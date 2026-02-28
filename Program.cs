using System;
using System.Threading;
using LLTC.Utils;

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
                }
            }
        }).Start();
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
                int coefficient = IntegerInput("Enter procent to change 'time passed' parameter: ");
                tm.CorrectTimePassed(coefficient);

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
}