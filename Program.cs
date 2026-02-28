using System;
using System.IO;
using System.Threading;

using LLTC.Utils;
using Microsoft.VisualBasic;
using static LLTC.Utils.EState;

namespace LLTC;

public static class Program
{ 
    private static EState state = Pause;

    private readonly static TimeHandle tm = new();
    private readonly static Timer timer = new (_ => { Tick(); }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

    public static void Main()
    {
        Init();

        ChangeState(Pause);
    }

    private static string GetReport()
    {
        string output = tm.Info();

        output += "\n\n\n\n" + 
            "spacebar - switch timer state (if not in edit mode)\n" +
            "e - edit mode (when paused)\n" +
            "c - add time to LLSA's today speaking time field (when paused)\n" +
            "r - reset timer (when paused)\n" +
            "q - exit (if not in edit mode)\n";
        
        return output;
    }

    private static void Tick()
    {
        tm.Pass(1);

        Console.Clear();
        Console.WriteLine(GetReport());
        Console.SetCursorPosition(0, 2);
    }

    private static void Init()
    {
        new Thread(() =>                                             // input handle thread
        {
            while (state != Exit)
            {
                var input = Console.ReadKey();

                switch (input.Key)
                {
                    case ConsoleKey.Spacebar:
                        tm.SwitchState();

                        if (state == Run)
                            ChangeState(Pause);
                        else
                            ChangeState(Run);
                        break;
                    
                    case ConsoleKey.Q:
                        if (state == Run || state == Pause)
                        {
                            ChangeState(Exit);
                            break;
                        }

                        ChangeState(Pause);
                        break;

                    case ConsoleKey.E:
                        if (state == Pause)
                            ChangeState(Edit);
                        break;

                    case ConsoleKey.C:
                        if (state == Pause)
                            ChangeState(Shout);
                        break;
                    
                    case ConsoleKey.R:
                        if (state == Pause)
                        {
                            tm.Reset();
                            ChangeState(Pause);
                        }
                        break;
                }
            }
        }).Start();
    
        ConfigManager.Init();
    }

    private static void ChangeState(EState newState)
    {
        state = newState;

        switch (state)
        {
            case Run:
                timer.Change(0, 1000);
                break;

            case Pause:
                timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

                Console.Clear();
                Console.WriteLine(GetReport());
                Console.SetCursorPosition(0, 2);
                break;
            
            case Edit:
                int coefficient = IntegerInput("Enter procent to change 'time passed' parameter (enter '100' to cancel): ");
                tm.CorrectTimePassed(coefficient);

                ChangeState(Pause);
                break;
            
            case Shout:
                string configPath = PathInput("Enter path to the LLSA compatible config file .\n" +
                    "If input is empty program will use last used path: ");

                var config = ConfigManager.Read(configPath);
                
                if (config.ContainsKey("TodayLanguageUseSecondCount"))
                {
                    int newValue = int.Parse(config["TodayLanguageUseSecondCount"]) + tm.TimePassed;

                    config["TodayLanguageUseSecondCount"] = newValue.ToString();

                    ConfigManager.Write(config, configPath);
                }

                ChangeState(Pause);
                break;
            
            case Exit:
                timer.Dispose();
                Console.Clear();
                break;
        }
    }

    private static int IntegerInput(string showTextOnConsole = "")
    {
        while (true)
        {
            Console.Clear();
            Console.Write(showTextOnConsole);

            if (int.TryParse(Console.ReadLine(), out int input))
                if (input >= 0)
                    return input;
        }
    }

    private static string PathInput(string showTextOnConsole = "")
    {
        string? lastUsedPath = null;
        try { lastUsedPath = ConfigManager.Read()["lastUsedPath"]; } catch { }
    
        while (true)
        {
            Console.Clear();
            Console.Write(showTextOnConsole);

            string input = Console.ReadLine() ?? "";

            if (Path.Exists(input))
            {
                var config = ConfigManager.Read();
                config["lastUsedPath"] = input;

                ConfigManager.Write(config);

                return input;
            }
            else if (lastUsedPath != null && Path.Exists(lastUsedPath))
                return lastUsedPath;
        }
    }
}