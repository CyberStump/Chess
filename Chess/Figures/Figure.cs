using System;



namespace Chess
{
    class Figure
    {       
        public char Symbol { get; private set; }       // Just the first letter in the name of the figure. 
        public char ColorSign { get; private set; }    // Storing color of figure in data file.
        public bool isMakeJump = false;                // Crunch. For pawns only.
        private ConsoleColor Color;                    // Black/white color of figure.
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
