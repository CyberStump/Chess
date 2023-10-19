using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Chess //                                                                                                        ->                                                     try not to laugh
{
    class Program : API
    {
        private static Game MainGame;
        public static string AppVersion { get; private set; } = "v1.0.5(7)";

        readonly static string GamesFilePath    = "games.txt";
        readonly static string SettingsFilePath = "settings.txt";
        readonly static string LanguageFilePath = "lang_new.txt";

        private static DataManager GamesDataManager;
        private static DataManager SettingsDataManager;
        private static DataManager LanguageDataManager;

        private static int WindowHeight = Console.WindowHeight;
        private static int WindowWidth  = Console.WindowWidth;

        public static ConsoleColor DefaultBackgroundColor { get; private set; } = ConsoleColor.Black;
        public static ConsoleColor DefaultForegroundColor { get; private set; } = ConsoleColor.White;
        
        private static string[] s_words = new string[30]; // Keywords of buttons/fields.

        public static Dictionary<string, string> dic_CurrentLanguageDic { get; private set; } = new Dictionary<string, string>();
        public static Dictionary<string, string[]> dic_AllLanguagesDic { get; private set; } = new Dictionary<string, string[]>();
        //private static ConsoleColor[] ColorsPalette = new ConsoleColor[10];

        private static string CurrentLanguage
        {
            get
            {
                return LanguageDataManager.Read().Substring(LanguageDataManager.Read().IndexOf('>') + 1, 3);
            }
        }





        private static bool BIOS()
        {
            bool check = false;


            Console.WindowHeight = WindowHeight;
            Console.WindowWidth = WindowWidth;
            Console.WriteLine();

            CheckFileExistance(new FileInfo(GamesFilePath), ref check);
            CheckFileExistance(new FileInfo(LanguageFilePath), ref check);

            return check;
        }

        
        private static void CheckFileExistance(FileInfo file, ref bool check)
        {
            Console.Write("    " + file.FullName);
            if (file.Exists)
            {
                WriteColored("    EXIST\n", ConsoleColor.Green);
                check = true;
            }
            else
            {
                WriteColored("    NOT EXIST\n", ConsoleColor.Red);
            }
        }




        /////////////////////////////////////////////////////////////////////////////////////        
        /////////////////////////////////////////////////////////////////////////////////////        

        static void Main(string[] args)
        {            
            Console.Title = "Chess";
            Console.CursorVisible   = false;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            ClearScreen();

            if (BIOS())
            {
                GamesDataManager = new DataManager(GamesFilePath);
                SettingsDataManager = new DataManager(SettingsFilePath);
                LanguageDataManager = new DataManager(LanguageFilePath);

                string languageTextFromData = LanguageDataManager.Read();
                languageTextFromData = languageTextFromData.Substring(languageTextFromData.IndexOf('/') + 1);

                int i = 0;
                /*while (languageTextFromData != "") // Filling keywords from language data file.
                {
                    s_words[i] = languageTextFromData.Substring(0, languageTextFromData.IndexOf(','));
                    languageTextFromData = languageTextFromData.Substring(languageTextFromData.IndexOf(',') + 1);
                    i++;
                }*/
                SeetLanguage();

                SetLanguage();
                MainMenu();

            }
            else
            {
                Console.ReadLine();
            }
        }
       
                       
        static void MainMenu()
        {
            bool isExit = false;
                        
            do
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;              
                ClearScreen();

                switch 
                (
                    SelectionMenuGUI(   
                        51, 10, 2, new string[] 
                        { 
                            dic_CurrentLanguageDic["continue"],
                            dic_CurrentLanguageDic["newgame"],
                            dic_CurrentLanguageDic["settings"],
                            dic_CurrentLanguageDic["exit"] 
                        })
                )
                {
                    case 0:
                        MainGame = new Game(false);
                        //Console.Read();
                        break;
                    case 1:
                        MainGame = new Game(true);
                        //Console.Read();
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

            ClearScreen();
            Console.SetCursorPosition(0, 0);
        }
                        

        private static void Settings()
        {
            bool isExit = false;


            ClearScreen();
            do 
            {                              
                WriteAtColored(6, 2, dic_CurrentLanguageDic["settings"], ConsoleColor.DarkGray);

                switch
                (
                    SelectionMenuGUI( 
                        8, 4, 1, new string[] 
                        {    
                            dic_CurrentLanguageDic["language"]   + ": " + CurrentLanguage,
                            dic_CurrentLanguageDic["colorsheme"] + ": -",
                            dic_CurrentLanguageDic["back"] 
                        })
                ) 
                {                    
                    case 0:
                        // crunch!
                        if (CurrentLanguage == "RUS")
                            ChangeLanguage("ENG"); 
                        else 
                            ChangeLanguage("RUS");
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

       
        static void SeetLanguage()
        {
            string langFileText = LanguageDataManager.Read();
            


            langFileText = langFileText.Replace("\r", "");
            int.TryParse( langFileText.Substring(langFileText.IndexOf("<=") - 4, 1), out int langNumber);
            dic_AllLanguagesDic.Clear();

            {                
                //int.TryParse(langFileText.Substring(langFileText.IndexOf("========__"), 1), out int _langCount);
                //tmp_words = new string[_langCount];
            }

            langFileText = langFileText.Substring(langFileText.IndexOf("========__"));
            do
            {
                string[] tmp_words = new string[5];
                int i = 0;

                langFileText = langFileText.Substring(11); // Erase separator
                string keyWord = langFileText.Substring(0, langFileText.IndexOf('\n'));
                dic_AllLanguagesDic.Add(keyWord, null);
                langFileText = langFileText.Substring(langFileText.IndexOf('\n') + 1); // Erase keyword
                do
                {
                    tmp_words[i] = langFileText.Substring(0, langFileText.IndexOf('\n'));
                    langFileText = langFileText.Substring(langFileText.IndexOf('\n') + 1);
                    i++;
                    if (langFileText.Substring(0, 9).Contains("=EOF=EOF=")) break;
                } // crunch
                while (!langFileText.Substring(0, langFileText.IndexOf('\n')).Contains("========__"));
                dic_AllLanguagesDic[keyWord] = tmp_words;
            }
            while (langFileText.Contains("========__"));


        }



        // FIX FIX FIX FIX FIX FIX FIX FIX FIX FIX FIX FIX FIX FIX FIX FIX
        static void ChangeLanguage(string language) 
        {
            string s_LanguageFileText = LanguageDataManager.Read();
            int langIndex = s_LanguageFileText.IndexOf(language) + 3;


            // Example:  >ENG  converts to:   ENG
            //            RUS                >RUS 
            s_LanguageFileText = s_LanguageFileText.Replace('>', ' ');
            s_LanguageFileText = s_LanguageFileText.Replace(" " + language, ">" + language);

            LanguageDataManager.Write(s_LanguageFileText);
            SetLanguage();
        }

        // FIX FIX FIX FIX FIX FIX FIX FIX FIX FIX FIX FIX FIX FIX FIX FIX
        static void SetLanguage()
        {
            string s_LanguageFileText;      // Contains text from language .txt file.
            string s_CurrentLanguageText;   // Will be filled with current selected language. 
            int index;                      // Position of arrow that marks current language.
            char symbol = ' ';


            dic_CurrentLanguageDic.Clear();

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

            // Filling dictionary with current language keywords. crunch?
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
                    dic_CurrentLanguageDic.Add(key, word);
                }                
            }
        }

               
        public static int SelectionMenuGUI(int StartX, int StartY, int interval, string[] fields)
        {
            ConsoleKey pressedKey;
            int selectedOption = 0;
            int offset = 0;


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
                        Console.Beep(500, 40);
                        WriteAt(StartX - 2, StartY + selectedOption * interval, " ");
                        selectedOption = selectedOption > 0 ? selectedOption - 1 : selectedOption;                        
                        break;
                    case ConsoleKey.DownArrow:
                        Console.Beep(500, 40);
                        WriteAt(StartX - 2, StartY + selectedOption * interval, " ");
                        selectedOption = selectedOption < fields.Length - 1 ? selectedOption + 1 : selectedOption;
                        break;
                    case ConsoleKey.Escape:
                        return -1;
                }                
            }
            while (pressedKey != ConsoleKey.Enter);

            Console.Beep(1000, 40);
            return selectedOption;
        }


    }
}
