using System;
using System.Threading;
using LLTC.Utils;

using static LLTC.Utils.EState;

namespace LLTC;

public static class Program
{ 
    private static EState state = Pause;
    private readonly static TimeHandle tm = new();

    private static ThreadStart? TInput;

    public static void Main()
    {
        InitHandlers();

        StartHandle(TInput);

        while (true)
        {
            if (state == Run || state == Pause)
            {
                Console.Clear();
                Console.WriteLine(tm.Info());

                Thread.Sleep(1000);

                tm.Pass(1);
            }
            else if (state == Edit)
            {
                int coefficient = IntegerInput("Enter procent to change 'time passed' parameter: ");
                tm.CorrectTimePassed(coefficient);

                state = Pause;

                StartHandle(TInput);
            }
            else if (state == Exit)
            {
                Console.Clear();
                break;
            }
        }
    }

    private static void InitHandlers()
    {
        TInput = () =>                                             // input handle thread
        {
            while (state != Exit && state != Edit)
            {
                var input = Console.ReadKey();

                switch (input.Key)
                {
                    case ConsoleKey.Spacebar:
                        tm.SwitchState();

                        state = state == Pause ? Run : Pause;
                        break;
                    
                    case ConsoleKey.Q:
                        if (state == Run || state == Pause)
                        {
                            state = Exit;
                            break;
                        }

                        state = Pause;
                        break;

                    case ConsoleKey.E:
                        if (state == Pause)
                            state = Edit;
                        break;
                }
            }
        };
    }
    private static void StartHandle(ThreadStart? threadInfo)
    {
        if (threadInfo != null)
            new Thread(threadInfo).Start();
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