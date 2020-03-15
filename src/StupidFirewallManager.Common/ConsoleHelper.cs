using System;
using System.Collections.Generic;
using System.Text;

namespace StupidFirewallManager.Common
{
    public static class ConsoleHelper
    {
        public static String AskPassword(String question)
        {
            Console.Write(AddDefaultValueToQuestion(question, null));
            StringBuilder password = new StringBuilder();

            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);
                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        if (password.Length > 0)
                            password.Length--;

                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return password.ToString();
        }

        private static string AddDefaultValueToQuestion(string question, string defaultFile)
        {
            question = question.TrimEnd(' ', ':');
            if (string.IsNullOrWhiteSpace(defaultFile))
            {
                return $"{question}:";
            }
            return $"{question}: [{defaultFile}]";
        }
    }
}
