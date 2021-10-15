using Chess.Figures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Chess //                                                                                                        ->                                                     try not to laugh
{
    class Program
    {
        private static string AppVersion = "v0.0.1(1) Beta";
        private static int _WindowHeight = Console.WindowHeight;
        private static int _WindowWidth  = Console.WindowWidth;

        private static ConsoleColor DefaultBackgroundColor = ConsoleColor.Black;
        private static ConsoleColor DefaultForegroundColor = ConsoleColor.White;

        private static Game MainGame;

        readonly static string s_GamesPath    = "games.txt";
        readonly static string s_SettingsPath = "settings.txt";
        readonly static string s_LanguagePath = "language.txt";

        private static DataManager GamesDataManager;
        private static DataManager SettingsDataManager;
        private static DataManager LanguageDataManager;

        private static string CurrentLanguage 
        {
            get {
                return LanguageDataManager.Read().Substring(LanguageDataManager.Read().IndexOf('>') + 1, 3);
            }
        }

        // Temporary
        private static string[] Languages = new string[]
        {
            "RUS", "ENG"
        };

        // Keywords of buttons/fields.
        private static string[] s_words = new string[30];

        public static Dictionary<string, string> dic_LanguageDic { get; private set; } = new Dictionary<string, string>();
        //private static ConsoleColor[] ColorsPalette = new ConsoleColor[10];
        
        


        // MAIN
        static void Main(string[] args)
        {            
            Console.Title = "Chess";
            Console.CursorVisible = false;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            ClearScreen();

            if (BIOS())
            {
                GamesDataManager = new DataManager(s_GamesPath);
                SettingsDataManager = new DataManager(s_SettingsPath);
                LanguageDataManager = new DataManager(s_LanguagePath);

                string languageTextFromData = LanguageDataManager.Read();
                languageTextFromData = languageTextFromData.Substring(languageTextFromData.IndexOf('/') + 1);
                
                int i = 0;
                while(languageTextFromData != "") // Filling keywords from language data file.
                {
                    s_words[i] = languageTextFromData.Substring(0, languageTextFromData.IndexOf(',')); 
                    languageTextFromData = languageTextFromData.Substring(languageTextFromData.IndexOf(',') + 1);
                    i++;
                }

                SetLanguage();
                MainMenu();                
            }
            ClearScreen();
        }


        private static bool BIOS() // crunch
        {
            int waitingTime = 0; // Milliseconds.
            bool check = false;
            int delay = 400;
            FileInfo dataFile = new FileInfo(s_GamesPath);
            FileInfo languageFile = new FileInfo(s_LanguagePath);

            Console.WindowHeight = _WindowHeight; 
            Console.WindowWidth = _WindowWidth;

            Console.WriteLine();
            Console.Write("    " + dataFile.FullName);
            if (!dataFile.Exists) 
            {
                Thread.Sleep(delay);
                WriteAt("    NOT EXIST\n", ConsoleColor.Red);
                waitingTime += delay;
                dataFile.Create();
            }
            else
            {
                WriteAt("    EXIST\n", ConsoleColor.Green);
                Thread.Sleep(100);
            } 

            Console.Write("    " + languageFile.FullName);
            if (!languageFile.Exists)
            {
                Thread.Sleep(delay);
                WriteAt("    NOT EXIST\n", ConsoleColor.Red);
                waitingTime += delay;
                languageFile.Create();
            }
            else
            {
                WriteAt("    EXIST\n", ConsoleColor.Green);
                Thread.Sleep(100);
            }
            Thread.Sleep(waitingTime);
            return true;
        }

                       
        static void MainMenu()
        {
            bool isExit = false;
            do
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;              
                ClearScreen();

                switch (SelectionMenuGUI( 51, 10, 2, new string[] { 
                        dic_LanguageDic["continue"],
                        dic_LanguageDic["newgame"],
                        dic_LanguageDic["settings"],
                        dic_LanguageDic["exit"] } ))
                {
                    case 0:
                        MainGame = new Game(false);
                        Console.Read();
                        break;
                    case 1:
                        MainGame = new Game(true);
                        Console.Read();
                        break;
                    case 2:
                        Settings();
                        break;
                    case 3:
                        isExit = true;
                        break;
                }
            }
            while(!isExit);            
        }

                 
        

        private static void Settings()
        {
            bool isExit = false;

            do 
            {                
                ClearScreen();
                WriteAt(6, 2, dic_LanguageDic["settings"], ConsoleColor.DarkGray);
                WriteAt(1, 29, AppVersion, ConsoleColor.DarkGray);

                switch (SelectionMenuGUI( 8, 4, 1, new string[] {    
                        dic_LanguageDic["language"]   + ": " + CurrentLanguage,
                        dic_LanguageDic["colorsheme"] + ": -",
                        dic_LanguageDic["back"] } ) ) 
                {                    
                    case 0:
                        // crunch!
                        if (CurrentLanguage == "RUS") {
                            ChangeLanguage("ENG"); 
                        }
                        else ChangeLanguage("RUS");
                        break;
                    case 1:
                        break;
                    case 2:
                        isExit = true;
                        break;
                    case -1: // ESC button.
                        isExit = true;
                        break;
                }                
            }
            while (!isExit);
        }


        static void ChangeLanguage(string language)
        {
            string s_LanguageFileText = LanguageDataManager.Read();
            int langIndex = s_LanguageFileText.IndexOf(language) + 3;

            // Example: >ENG converts to >RUS .
            s_LanguageFileText = s_LanguageFileText.Replace('>', ' ');
            s_LanguageFileText = s_LanguageFileText.Replace(" " + language, ">" + language);

            LanguageDataManager.Write(s_LanguageFileText);
            SetLanguage();
        }


        static void SetLanguage()
        {
            string s_LanguageFileText,      // Contains text from language .txt file.
                   s_CurrentLanguageText;   // Will be filled with current selected language. 
            int index;                      // Position of arrow that marks current language.
            char symbol = ' ';

            dic_LanguageDic.Clear();

            s_LanguageFileText    = LanguageDataManager.Read();
            s_CurrentLanguageText = "";

            index = s_LanguageFileText.IndexOf('>') + 4;

            // Gathering current language dictionary.
            while (symbol != '}')
            {
                s_CurrentLanguageText += s_LanguageFileText[index];
                symbol = s_LanguageFileText[index];
                index++;
            }
            // Filling dictionary with current language keywords.
            // crunch?
            foreach (string key in s_words) 
            {
                string word = "";
                if (key != null)
                {
                    int index2 = s_CurrentLanguageText.IndexOf(key);

                    while (s_CurrentLanguageText[index2] != '-')
                    {
                        index2++;
                    }
                    index2++;
                    while (s_CurrentLanguageText[index2] != ';')
                    {
                        word += s_CurrentLanguageText[index2];
                        index2++;
                    }
                    dic_LanguageDic.Add(key, word);
                }                
            }
            ClearScreen();
        }




        private static void Exit()
        {
            Console.Clear();
            Console.WriteLine("\n    ");
        }




        public static void WriteAt(int x, int y, string text)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(text);         
        }


        public static void WriteAt(string text, ConsoleColor color)
        {
            ConsoleColor currentTextColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = currentTextColor;
        }


        public static void WriteAt(int x, int y, string text, ConsoleColor color)
        {
            ConsoleColor currentTextColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.SetCursorPosition(x, y);
            Console.Write(text);
            Console.ForegroundColor = currentTextColor;
        }


        public static void SetDefaultConsoleColors()
        {
            Console.BackgroundColor = DefaultBackgroundColor;
            Console.ForegroundColor = DefaultForegroundColor;
        }


        public static void ClearScreen()
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < 29; i++)
            {
                Console.WriteLine("                                                                                                                       ");
            }
            WriteAt(1, 29, AppVersion, ConsoleColor.DarkGray);
            Console.SetCursorPosition(0, 0);
        }


        public static void ClearScreen(int startY, int count)
        {
            Console.SetCursorPosition(0, startY);
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine("                                                                                                                       ");
            }
            WriteAt(1, 29, AppVersion, ConsoleColor.DarkGray);
            Console.SetCursorPosition(0, 0);
        }


        public static int SelectionMenuGUI(int StartX, int StartY, int interval, string[] fields)
        {
            ConsoleKey pressedKey;
            int selectedOption = 0,
                offset = 0;

            ClearScreen(3, 6);
            foreach (string field in fields) 
            {
                WriteAt(StartX, StartY + offset, field);
                offset += interval;
            }
            
            do
            {
                WriteAt(StartX - 2, StartY + selectedOption * interval, ">");
                switch (pressedKey = Console.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow:
                        WriteAt(StartX - 2, StartY + selectedOption * interval, " ");
                        selectedOption = selectedOption > 0 ? selectedOption - 1 : selectedOption;                        
                        break;
                    case ConsoleKey.DownArrow:
                        WriteAt(StartX - 2, StartY + selectedOption * interval, " ");
                        selectedOption = selectedOption < fields.Length - 1 ? selectedOption + 1 : selectedOption;
                        break;
                    case ConsoleKey.Escape:
                        return -1;
                }                
            }
            while (pressedKey != ConsoleKey.Enter);            
            return selectedOption;
        }   


        // EOF :)
    }
}
