using System;

namespace Chess
{
    class Figure
    {       
        public char Symbol { get; private set; }       // Just the first letter in the name of the figure. 
        private ConsoleColor Color;                    // Black/white color of figure.
        public char ColorSign { get; private set; }    // To write in data file color of figure.
        public bool isMakeJump = false;                // Crunch. For pawns only.

        public int MovesCount = 0;

        public Figure(char symbol, char colorSign)
        {
            Symbol = symbol;

            if (colorSign == 'b') colorSign = 'B';
            if (colorSign == 'w') colorSign = 'W';
            ColorSign = colorSign;
            Color = ColorSign == 'B' ? ConsoleColor.Black : ConsoleColor.White;            
        }


        public void Draw()
        {            
            ConsoleColor tmpColor = Console.ForegroundColor;
            Console.ForegroundColor = Color;
            Console.Write(Symbol + " ");
            Console.ForegroundColor = tmpColor;
        }


        public void Draw(int posX, int posY)
        {
            ConsoleColor tmpColor = Console.ForegroundColor;
            Console.ForegroundColor = Color;
            Console.SetCursorPosition(posX, posY);
            Console.Write(Symbol + " ");
            Console.ForegroundColor = tmpColor;
        }



    }
}
