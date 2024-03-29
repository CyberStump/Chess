﻿using Chess.Figures;
using System;

namespace Chess
{
    class Game : API
    {
        // Form for recording data about figures:
        // {sign of figure k - knight, Q - queen, etc.}{Color: Black - B, White - W}{i absolute coordinate}{j absolute coordinate}
        // kB01 - black knight at 0 1

        private Figure[,] Board;

        private char[] CellNames_Letters = new char[8] 
        { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

        private char[] CellNames_Numbers = new char[8] 
        { '1', '2', '3', '4', '5', '6', '7', '8' };


        private char Winner = ' ';

        private string TargetedCellSign = "A1";  // The cursor is hovered over this cell;

        private int TargetedCellX = 0;           // j
        private int TargetedCellY = 0;           // i

        private int BoardStartCoordX = 50;       // Board placement
        private int BoardStartCoordY = 10;

        private ConsoleColor DefaultBoardColor1 = ConsoleColor.DarkGray;
        private ConsoleColor DefaultBoardColor2 = ConsoleColor.Red;

        private DateTime FirstPlayerTime = DateTime.MinValue;
        private DataManager GamesDataManager = new DataManager("games.txt");
        //private DataManager LanguageDataManager = new DataManager("language.txt");

        private int MoveCount;       

        private char CurrentColorMove
        { // White always goes first.
            get
            {
                if (MoveCount % 2 == 0)
                    return 'W';
                else return 'B';
            }
        }




        public Game(bool newGame)
        {            
            Board = new Figure[8, 8];


            if (newGame) 
            {
                CreateNewBoard();
                WriteBoardToGameData();
            }
            else 
            {
                SynchronizeBoardFromGameData();
            }            
            DrawBoard();

            while (true)
                if(!MakeMove()) break;
        }


        private bool MakeMove()
        {
            int[] FigureCoord   = new int[2];  // Coordinates of selected figure.
            int[] MoveCellCoord = new int[2];  // Coordinates of selected cell to move figure.
            bool selected = false; // ?
            bool continueMove = true;
            string selectedFigure = "";


            // Select figure to move
            do
            {
                WriteAt(BoardStartCoordX + 22, BoardStartCoordY + 2, Program.dic_CurrentLanguageDic["selectfigure"]);
                
                FigureCoord = SelectCell();
                if (FigureCoord[0] != -1)
                {
                    selectedFigure = TargetedCellSign;
                    if (Board[FigureCoord[0], FigureCoord[1]] != null)
                    {
                        if (Board[FigureCoord[0], FigureCoord[1]].ColorSign == CurrentColorMove
                        && CheckAnyPossibleMoves(Board[FigureCoord[0], FigureCoord[1]], FigureCoord[0], FigureCoord[1]))
                            selected = true;
                    }
                }
                else
                {
                    continueMove = false;
                }
            }
            while (!selected && continueMove);


            selected = false; // crunch?
            if (continueMove)
            {
                WriteAtColored(BoardStartCoordX + TargetedCellX * 2,
                               BoardStartCoordY + 8,
                               "^", ConsoleColor.Green);
                WriteAtColored(BoardStartCoordX + 16,
                               BoardStartCoordY + TargetedCellY,
                               "<", ConsoleColor.Green);
                do // Select cell to move figure at.
                {
                    WriteAt(BoardStartCoordX + 22, BoardStartCoordY + 2, "Выберите клетку для хода");
                    WriteAt(BoardStartCoordX + 24, BoardStartCoordY + 4, " <- " + selectedFigure);

                    MoveCellCoord = SelectCell();
                    if (MoveCellCoord[0] != -1)
                    {
                        if (CheckMove(
                        Board[FigureCoord[0], FigureCoord[1]],
                        FigureCoord[0], FigureCoord[1],
                        MoveCellCoord[0], MoveCellCoord[1]))
                        {
                            Board[MoveCellCoord[0], MoveCellCoord[1]] = Board[FigureCoord[0], FigureCoord[1]];
                            Board[FigureCoord[0], FigureCoord[1]] = null;
                            selected = true;
                            Board[MoveCellCoord[0], MoveCellCoord[1]].MovesCount++;
                            MoveCount++;
                            WriteBoardToGameData();
                            DrawBoard();
                        }

                    }
                    else
                    {
                        continueMove = false;
                        selected = true;
                    }
                }
                while (!selected);
            }

            if (CheckWin())
            {
                CreateNewBoard();
                ShowWinScreen(Winner);
                WriteBoardToGameData();
            }

            return continueMove;
        }


        private void CreateNewBoard()
        {
            MoveCount = 0;

            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    Board[i, j] = null;
                }
            }

            for (int i = 0; i < 8; i++) {
                Board[1, i] = new Pawn('B');
                Board[6, i] = new Pawn('W');
            }

            Board[0, 0] = new Rook('B');
            Board[0, 7] = new Rook('B');
            Board[0, 1] = new Knight('B');
            Board[0, 6] = new Knight('B');
            Board[0, 2] = new Bishop('B');
            Board[0, 5] = new Bishop('B');
            Board[0, 3] = new King('B');
            Board[0, 4] = new Queen('B');

            Board[7, 0] = new Rook('W');
            Board[7, 7] = new Rook('W');
            Board[7, 1] = new Knight('W');
            Board[7, 6] = new Knight('W');
            Board[7, 2] = new Bishop('W');
            Board[7, 5] = new Bishop('W');
            Board[7, 3] = new King('W');
            Board[7, 4] = new Queen('W');            
        }


        private void WriteBoardToGameData()
        {
            string gameData = DateTime.Now + " c" + MoveCount + "/\n";


            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (Board[i, j] != null) {
                        gameData += $"{Board[i, j].Symbol}{Board[i, j].ColorSign}{i}{j}\n";
                    }
                }
            }

            GamesDataManager.Write(gameData + "/");
        }


        public void DrawBoard()
        {
            ClearScreen();
            WriteAtColored(1, 29, Program.AppVersion, ConsoleColor.DarkGray);

            for (int i = 0; i < 8; i++)
            {
                WriteAt(BoardStartCoordX - 2, BoardStartCoordY + i, CellNames_Letters[i].ToString());
                for (int j = 0; j < 8; j++)
                {
                    WriteAt(BoardStartCoordX + j * 2, BoardStartCoordY - 1, CellNames_Numbers[j].ToString());

                    if ((i + j + 1) % 2 == 0) Console.BackgroundColor = DefaultBoardColor1;
                    if ((i + j + 1) % 2 != 0) Console.BackgroundColor = DefaultBoardColor2;

                    Console.SetCursorPosition(BoardStartCoordX + j * 2, BoardStartCoordY + i);
                    if (Board[i, j] != null) 
                    {                        
                        Board[i, j].Draw();
                    }
                    else Console.Write("  ");
                    SetConsoleColors(Program.DefaultForegroundColor, Program.DefaultBackgroundColor);
                }
            }            
            //Program.WriteAt(40, 6, TargetedCell);
        }


        private void SynchronizeBoardFromGameData()
        {
            Figure figure = null;
            int i, j;
            string s_gameData = GamesDataManager.Read();
            int moveCountPosIndex = s_gameData.IndexOf('c');
            

            // crunch
            if(!int.TryParse(s_gameData.Substring(moveCountPosIndex + 1, 
               s_gameData.IndexOf('/') - moveCountPosIndex - 1), out MoveCount))
            {
                throw new Exception("/TryParse MoveCount from game data failure\\");
            }
            

            s_gameData = s_gameData.Substring(s_gameData.IndexOf('/') + 1).Replace("\n", "");
            while (s_gameData.Length > 2) 
            {                
                switch (s_gameData[0])
                {
                    case 'P':
                             figure = new Pawn(s_gameData[1]);
                             break;
                    case 'K':
                             figure = new King(s_gameData[1]);
                             break;
                    case 'Q':
                             figure = new Queen(s_gameData[1]);
                             break;
                    case 'R':
                             figure = new Rook(s_gameData[1]);
                             break;
                    case 'B':
                             figure = new Bishop(s_gameData[1]);
                             break;
                    case 'k':
                             figure = new Knight(s_gameData[1]);
                             break;
                }
                i = int.Parse(s_gameData[2].ToString());
                j = int.Parse(s_gameData[3].ToString());
                Board[i, j] = figure;

                s_gameData = s_gameData.Length > 4 ? s_gameData.Substring(4) : "";
            }
        }


        private bool CheckMove(Figure figure, int y, int x, int targetY, int targetX)
        {
            if(Board[targetY, targetX] != null) 
                if (Board[targetY, targetX].ColorSign == CurrentColorMove || (targetX == x && targetY == y))
                    return false;

            if(figure != null)
            {                
                switch (figure.Symbol)
                {
                    case 'P':
                        if (figure.ColorSign == 'W')
                        { // White always at bottom.
                            if (Board[targetY, targetX] != null) {
                                if (Board[targetY, targetX].ColorSign != CurrentColorMove) {
                                    if (y - targetY == 1 && (x - targetX == 1 || targetX - x == 1)) return true;
                                }
                            }
                            else
                            {
                                if (y == 6 && y - targetY == 2
                                    && CheckFigureWay(y, x, targetY, targetX) && x == targetX)
                                {
                                    Board[y, x].isMakeJump = true;
                                    return true;
                                }
                                if(y == 3 && y - targetY == 1)
                                {
                                    if(Board[2, targetX] == null) {
                                        if(Board[3, targetX] != null) {
                                            if(Board[3, targetX].isMakeJump) {
                                                Board[3, targetX] = null;
                                                return true;
                                            }
                                        }
                                    }
                                }
                                return y - targetY == 1 && x == targetX;
                            }                            
                        }

                        if (figure.ColorSign == 'B')
                        { // Black always at top.
                            if (Board[targetY, targetX] != null)
                            {
                                if (Board[targetY, targetX].ColorSign != CurrentColorMove)
                                {
                                    if (targetY - y == 1 && (x - targetX == 1 || targetX - x == 1)) return true;
                                }
                            }
                            else
                            {
                                if (y == 1 && targetY - y == 2 &&
                                    CheckFigureWay(y, x, targetY, targetX) && x == targetX)
                                {
                                    Board[y, x].isMakeJump = true;
                                    return true;
                                }
                                if (y == 4 && targetY - y == 1)
                                {                                    
                                    if (Board[5, targetX] == null) {
                                        if (Board[4, targetX] != null) {
                                            if (Board[4, targetX].isMakeJump) {
                                                Board[4, targetX] = null;
                                                return true;
                                            }
                                        }
                                    }                                    
                                }
                                return targetY - y == 1 && x == targetX;
                            }                            
                        }
                        break;


                    case 'K': 
                        if ( figure.MovesCount == 0 && (y == 0 || y == 7) && x == 3 && Math.Abs(targetX - x) > 1 )
                        {
                            if (targetX == 1 && CheckFigureWay(y, x, targetY, targetX) && Board[y, 0] != null)
                            {
                                if(Board[y, 0].MovesCount == 0)
                                {
                                    Board[y, 2] = Board[y, 0];
                                    Board[y, 0] = null;
                                    return true;
                                }
                                else goto DefaultWay;
                            }
                            else if (targetX == 5 && CheckFigureWay(y, x, targetY, targetX + 1) && Board[y, 7] != null)
                            {
                                if (Board[y, 0].MovesCount == 0)
                                {
                                    Board[y, 4] = Board[y, 7];
                                    Board[y, 7] = null;
                                    return true;
                                }
                                else goto DefaultWay;
                            }
                            else goto DefaultWay;
                        }
                        DefaultWay:
                        {
                            return ((targetX - x <= 1 && targetX - x >= -1) && (targetY - y <= 1 && targetY - y >= -1)) &&
                                   CheckFigureWay(y, x, targetY, targetX);
                        }

                    case 'Q':
                            return (Math.Abs(targetX - x) == Math.Abs(targetY - y) || targetX - x == 0 || targetY - y == 0) &&
                                   CheckFigureWay(y, x, targetY, targetX);
                    case 'R':
                            return (targetX - x == 0 || targetY - y == 0) && CheckFigureWay(y, x, targetY, targetX);
                    case 'B':
                            return (Math.Abs(targetX - x) == Math.Abs(targetY - y)) && CheckFigureWay(y, x, targetY, targetX);
                    case 'k':
                            return ((targetX - x == -2 || targetX - x == 2) && (targetY - y == -1 || targetY - y == 1)) ||
                                   ((targetX - x == -1 || targetX - x == 1) && (targetY - y == -2 || targetY - y == 2));
                }
            }            
            return false; 
        }



        private bool CheckFigureWay(int y, int x, int targetY, int targetX)
        {
            int xMod, yMod; // Modules of coord for loop.
                
            if (targetY - y == 0) 
                yMod = 0;
            else 
                yMod = (targetY - y) / Math.Abs(targetY - y);

            if (targetX - x == 0) 
                xMod = 0;
            else 
                xMod = (targetX - x) / Math.Abs(targetX - x);

            int i = y + yMod;
            int j = x + xMod;
            
            while(i != targetY && j != targetX)
            {
                if (Board[i, j] != null) return false;
                i += yMod;
                j += xMod;
            }

            return true;
        }

        
        private bool CheckAnyPossibleMoves(Figure figure, int y, int x) 
        {
            if (figure != null)
            {
                switch (figure.Symbol)
                {
                    case 'K':
                    case 'Q':
                    case 'R':
                        for (int j = -1; j < 2; j++)
                        {
                            for (int i = -1; i < 2; i++)
                            {
                                if (!(j == 0 && i == 0))
                                {
                                    if (x + j > -1 && x + j < 8 && y + i > -1 && y + i < 8)
                                    {
                                        if (Board[y + i, x + j] == null) return true;
                                        else if (Board[y + i, x + j].ColorSign != CurrentColorMove) return true;
                                    }
                                }
                            }
                        }
                        break;


                    case 'k':
                        for (int j = -1; j < 2; j += 2)
                        {
                            for (int i = -2; i < 3; i += 4)
                            {
                                if (x + j > -1 && x + j < 8 && y + i > -1 && y + i < 8)
                                {
                                    if (Board[y + i, x + j] == null) return true;
                                    else if (Board[y + i, x + j].ColorSign != CurrentColorMove) return true;
                                }
                            }
                        }
                        for (int j = -2; j < 3; j += 4)
                        {
                            for (int i = -1; i < 2; i += 2)
                            {
                                if (x + j > -1 && x + j < 8 && y + i > -1 && y + i < 8)
                                {
                                    if (Board[y + i, x + j] == null) return true;
                                    else if (Board[y + i, x + j].ColorSign != CurrentColorMove) return true;
                                }
                            }
                        }
                        break;


                    case 'B':
                        for (int j = -1; j < 2; j += 2)
                        {
                            for (int i = -1; i < 2; i += 2)
                            {
                                if (x + j > -1 && x + j < 8 && y + i > -1 && y + i < 8)
                                {
                                    if (Board[y + i, x + j] == null) return true;
                                    else if (Board[y + i, x + j].ColorSign != CurrentColorMove) return true;
                                }
                            }
                        }
                        break;


                    case 'P':
                        if (figure.ColorSign == 'W')
                        { // White always at bottom.
                            if (Board[y - 1, x] == null) return true;
                            if (Board[y - 1, x - 1] != null)
                                if (Board[y - 1, x - 1].ColorSign != CurrentColorMove) return true;
                            if (Board[y - 1, x + 1] != null)
                                if (Board[y - 1, x + 1].ColorSign != CurrentColorMove) return true;
                            // Check the possibility of taking on the pass.
                            if (y == 3)
                            {
                                if(x > 0)
                                {
                                    if (Board[3, x - 1] != null)
                                        if (Board[3, x - 1].ColorSign != CurrentColorMove) return true;
                                }
                                if (x < 7)
                                {
                                    if (Board[3, x + 1] != null)
                                        if (Board[3, x + 1].ColorSign != CurrentColorMove) return true;
                                }
                            }
                        }
                        else if (figure.ColorSign == 'B')
                        { // Black always at top.
                            if (Board[y + 1, x] == null) return true;
                            if (Board[y + 1, x - 1] != null)
                                if (Board[y + 1, x - 1].ColorSign != CurrentColorMove) return true;                            
                            if (Board[y + 1, x + 1] != null)
                                if (Board[y + 1, x + 1].ColorSign != CurrentColorMove) return true;                            
                            // Check the possibility of taking on the pass.
                            if (y == 4)
                            {
                                if (x > 0)
                                {
                                    if (Board[4, x - 1] != null)
                                        if (Board[4, x - 1].ColorSign != CurrentColorMove) return true;
                                }
                                if (x < 7)
                                {
                                    if (Board[4, x + 1] != null)
                                        if (Board[4, x + 1].ColorSign != CurrentColorMove) return true;
                                }
                            }
                        }
                        break;
                }                
            }
            else throw new NullReferenceException("CheckAnyPossibleMoves(): null figure on input! ");
                        
            return false;
        }


        private int[] SelectCell()
        {
            ConsoleKey pressedKey;
            do
            {
                UpdateBoardInfo();
                WriteAt(BoardStartCoordX + TargetedCellX * 2, BoardStartCoordY + 9, "^");
                WriteAt(BoardStartCoordX + 18, BoardStartCoordY + TargetedCellY, "<");

                switch (pressedKey = Console.ReadKey().Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (TargetedCellX > 0)
                        {
                            WriteAt(BoardStartCoordX + TargetedCellX * 2, BoardStartCoordY + 9, " ");
                            TargetedCellX--;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (TargetedCellX < 7)
                        {
                            WriteAt(BoardStartCoordX + TargetedCellX * 2, BoardStartCoordY + 9, " ");
                            TargetedCellX++;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if (TargetedCellY > 0)
                        {
                            WriteAt(BoardStartCoordX + 18, BoardStartCoordY + TargetedCellY, "  ");
                            TargetedCellY--;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (TargetedCellY < 7)
                        {
                            WriteAt(BoardStartCoordX + 18, BoardStartCoordY + TargetedCellY, "  ");
                            TargetedCellY++;
                        }
                        break;
                    case ConsoleKey.Escape:
                        return new int[] { -1, -1 };
                }                
            }
            while (pressedKey != ConsoleKey.Enter);

            return new int[] { TargetedCellY, TargetedCellX };
        }


        private bool CheckWin()
        {
            int kingsCount = 0;
            char winner = ' ';
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if(Board[i, j] != null) {
                        if (Board[i, j].Symbol == 'K') {
                            kingsCount++;
                            winner = Board[i, j].ColorSign;
                        }
                    }                    
                }
            }

            if (kingsCount == 1)
            {
                Winner = winner;
                return true;
            }
            else
            {
                return false;
            }
        }


        private void ShowWinScreen(char winner)
        {
            ClearScreen();
            WriteAt(4, 2, Program.dic_CurrentLanguageDic["winner"] + ":");
            
            switch (winner)
            {
                case 'W':
                    WriteAt(16, 2, Program.dic_CurrentLanguageDic["white"]);
                    break;
                case 'B':
                    WriteAt(16, 2, Program.dic_CurrentLanguageDic["black"]);
                    break;
            }

            WriteAt(4, 3, Program.dic_CurrentLanguageDic["movescount"] + ":");
            WriteAt(21, 3, MoveCount.ToString());

            Console.ReadKey();
        }


        private void UpdateBoardInfo()
        {
            TargetedCellSign = CellNames_Letters[TargetedCellY].ToString() + CellNames_Numbers[TargetedCellX].ToString();

            switch (CurrentColorMove)
            {
                case 'B': // Black move
                    WriteAt(BoardStartCoordX + 22, BoardStartCoordY, Program.dic_CurrentLanguageDic["move"] + ": " + Program.dic_CurrentLanguageDic["black"]);
                    break;
                case 'W': // White move
                    WriteAt(BoardStartCoordX + 22, BoardStartCoordY, Program.dic_CurrentLanguageDic["move"] + ": " + Program.dic_CurrentLanguageDic["white"]);
                    break;
            } 

            WriteAt(BoardStartCoordX + 22, BoardStartCoordY + 4, TargetedCellSign);
            WriteAt(BoardStartCoordX + 22, BoardStartCoordY + 6, Program.dic_CurrentLanguageDic["movescount"] + ": " + MoveCount.ToString());
        }



    }
}
