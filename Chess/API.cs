﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class API
    {


        public static void Write(string text)
        {
            Console.Write(text);
        }


        public static void Writeln(string text)
        {
            Console.WriteLine(text);
        }


        public static void WriteAt(int x, int y, string text)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(text);
        }


        public static void WriteAtColored(int x, int y, string text, ConsoleColor color)
        {
            Console.SetCursorPosition(x, y);
            WriteColored(text, color);
        }


        public static void WriteColored(string text, ConsoleColor color)
        {
            ConsoleColor currentTextColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = currentTextColor;
        }


        public static void WriteColored(string text, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            ConsoleColor currentForegroundColor = Console.ForegroundColor;
            ConsoleColor currentBackgroundColor = Console.BackgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.Write(text);
            Console.ForegroundColor = currentForegroundColor;
            Console.BackgroundColor = currentBackgroundColor;
        }
              

        public static void SetConsoleColors(ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
        }


        public static void ClearScreen()
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < 29; i++) {
                Console.WriteLine("                                                                                                                       ");
            }
            Console.SetCursorPosition(0, 0);
        }


        public static void ClearScreen(int startY, int count)
        {
            Console.SetCursorPosition(0, startY);
            for (int i = 0; i < count; i++) {
                Console.WriteLine("                                                                                                                       ");
            }
            Console.SetCursorPosition(0, 0);
        }



    }
}