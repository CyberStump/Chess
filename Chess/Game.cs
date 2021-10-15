﻿using Chess.Figures;
using System;
using System.Threading;

namespace Chess
{
    class Game
    {
        // Form for recording data about figures:
        // {sign of figure k - knight, Q - queen, etc.}{Color: Black - B, White - W}{i absolute coordinate}{j absolute coordinate}
        // kB01 - black knight at 0 1

        private Figure[,] Board;

        private char[] CellNames_Letters = new char[8] 
        { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

        private char[] CellNames_Numbers = new char[8] 
        { '1', '2', '3', '4', '5', '6', '7', '8' };

        private int MoveCount;
        private char CurrentColorMove { // White always goes first.
            get {
                if (MoveCount % 2 == 0)
                    return 'W';
                else return 'B';
            }
        }

        private string TargetedCellSign = "A1";
        private int TargetedCellX       = 0;     // j
        private int TargetedCellY       = 0;     // i

        private int BoardStartCoordX   = 50;
        private int BoardStartCoordY   = 10;
        //private int BoardVisibleSize = 1;

        private DateTime FirstPlayerTime = DateTime.MinValue;
        private DataManager GamesDataManager = new DataManager("games.txt");
        //private DataManager LanguageDataManager = new DataManager("language.txt");



        public Game(bool newGame)
        {            
            Board = new Figure[8, 8];

            if (newGame) {
                CreateNewBoard();
                SynchronizeBoardFromGameData();
            }
            else {
                SynchronizeBoardFromGameData();
            }            
            DrawBoard();

            while (!CheckWin())
            {
                MakeMove();
            }
        }


        private void MakeMove()
        {
            DateTime startMoveTime = DateTime.Now; // Time of start a move.
            ConsoleKey pressedKey;  

            int[] FigureCoord   = new int[2];  // Coordinates of selected figure.
            int[] MoveCellCoord = new int[2];  // Coordinates of selected cell to move figure.
            int yBoardMin = 0,
                yBoardMax = 7;
            bool selected;
            bool continueMove = true;
            string selectedFigure;

            switch (CurrentColorMove)
            {
                case 'W': // White move
                    TargetedCellY = 6;
                    break;
                case 'B': // Black move
                    TargetedCellY = 1;
                    break;
            }

            selected = false; // crunch?
            do // Select figure to move
            {
                Program.WriteAt(BoardStartCoordX + 25, BoardStartCoordY, "Выберите фигуру");
                
                FigureCoord = SelectCell();
                selectedFigure = TargetedCellSign;
                if (Board[FigureCoord[0], FigureCoord[1]] != null) 
                {
                    if(Board[FigureCoord[0], FigureCoord[1]].ColorSign == CurrentColorMove
                    && CheckAnyPossibleMoves(Board[FigureCoord[0], FigureCoord[1]], FigureCoord[0], FigureCoord[1]))
                        selected = true;
                }
                else if(FigureCoord[0] == -1) {
                    continueMove = false;
                }
            }
            while (!selected);


            selected = false; // crunch?
            if (continueMove)
            {
                do // Select cell to move figure at
                {
                    Program.WriteAt(BoardStartCoordX + 25, BoardStartCoordY, "<- " + selectedFigure + " Выберите клетку для хода");
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
                            MoveCount++;
                            DrawBoard();
                        }
                            
                    }
                    else
                    {
                        continueMove = false;
                    }
                }
                while (!selected);
            }
            

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
            Board[7, 4] = new King('W');
            Board[7, 3] = new Queen('W');            
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
            Program.ClearScreen();
            for (int i = 0; i < 8; i++)
            {
                Program.WriteAt(BoardStartCoordX - 2, BoardStartCoordY + i, CellNames_Letters[i].ToString());
                for (int j = 0; j < 8; j++)
                {
                    Program.WriteAt(BoardStartCoordX + j * 2, BoardStartCoordY - 1, CellNames_Numbers[j].ToString());
                    if ((i + j + 1) % 2 == 0) Console.BackgroundColor = ConsoleColor.DarkGray;
                    if ((i + j + 1) % 2 != 0) Console.BackgroundColor = ConsoleColor.Red;

                    Console.SetCursorPosition(BoardStartCoordX + j * 2, BoardStartCoordY + i);
                    if (Board[i, j] != null) {                        
                        Board[i, j].Draw();
                    }
                    else Console.Write("  ");
                    Program.SetDefaultConsoleColors();
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
            
            int.TryParse( s_gameData.Substring(moveCountPosIndex + 1, s_gameData.IndexOf('/') - moveCountPosIndex - 1),
                          out MoveCount );

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
            switch (figure.Symbol)
            {
                case 'P':
                    if(figure.ColorSign == 'W') { // White always at bottom.
                        return y - targetY == 1 && x == targetX;
                    }
                    if(figure.ColorSign == 'B') { // Black always at top.
                        return targetY - y == 1 && x == targetX;
                    }
                    break;
                case 'K':
                    return (targetX - x <= 1 && targetX - x >= -1) || (targetY - y <= 1 && targetY - y >= -1);
                case 'Q':
                    return targetX - x == targetY - y || targetX - x == 0 || targetY - y == 0;
                case 'R':
                    return targetX - x == 0 || targetY - y == 0;
                case 'B':
                    return targetX - x == targetY - y;
                case 'k':
                    return ((targetX - x == -2 || targetX - x == 2) && (targetY - y == -1 || targetY - y == 1))
                        || ((targetX - x == -1 || targetX - x == 1) && (targetY - y == -2 || targetY - y == 2));
            }
            return false; 
        }

        // TODO:
        private bool CheckAnyPossibleMoves(Figure figure, int y, int x) 
        {
            if (figure != null)
            {
                if (figure.Symbol != 'k') // If figure isn't knight (horse).
                {
                    for (int i = -1; i < 2; i++)
                    {
                        for (int k = -1; k < 2; k+=2)
                        {
                            if( x + i > -1 && x + i < 8 && y + k > -1 && y + k < 8 )
                            {
                                if (Board[y + k, x + i] == null) return true;
                                else if (Board[y + k, x + i].Symbol != CurrentColorMove) return true;
                            }
                        }
                    }
                    for (int i = -1; i < 2; i += 2)
                    {
                        if (x + i > -1 && x + i < 8)
                        {
                            if (Board[y, x + i] == null) return true;
                            else if (Board[y, x + i].Symbol != CurrentColorMove) return true;
                        }
                    }
                }

                else if (figure.Symbol == 'k')
                {
                    if (y > 1 || y < 6 || x > 1 || x < 6) // ????
                    {
                        for (int i = -1; i < 2; i += 2)
                        {
                            for (int k = -2; k < 3; k += 4)
                            {
                                if (Board[y + k, x + i] == null) {
                                    return true;
                                }
                                else if (Board[y + k, x + i].Symbol != CurrentColorMove) {
                                    return true;
                                }
                            }
                        }
                    }
                    else if (y - 1 > -1 && y + 1 < 8 && x - 2 > -1 && x + 2 < 8)
                    {
                        for (int i = -1; i < 2; i += 2)
                        {
                            for (int k = -2; k < 3; k += 4)
                            {
                                if (Board[y + i, x + k] == null) {
                                    return true;
                                }
                                else if (Board[y + i, x + k].Symbol != CurrentColorMove) {
                                    return true;
                                }
                            }
                        }
                    }
                    else return false;
                    
                }
            }
            else throw new Exception(" Null object in CheckAnyMovePosition() ");
            
            
            return false;
        }


        private int[] SelectCell()
        {
            ConsoleKey pressedKey;      
            do
            {
                UpdateBoardInfo();
                Program.WriteAt(BoardStartCoordX + TargetedCellX * 2, BoardStartCoordY + 9, "^");
                Program.WriteAt(BoardStartCoordX + 18, BoardStartCoordY + TargetedCellY, "<");
                switch (pressedKey = Console.ReadKey().Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (TargetedCellX > 0)
                        {
                            Program.WriteAt(BoardStartCoordX + TargetedCellX * 2, BoardStartCoordY + 9, " ");
                            TargetedCellX--;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (TargetedCellX < 7)
                        {
                            Program.WriteAt(BoardStartCoordX + TargetedCellX * 2, BoardStartCoordY + 9, " ");
                            TargetedCellX++;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if (TargetedCellY > 0)
                        {
                            Program.WriteAt(BoardStartCoordX + 18, BoardStartCoordY + TargetedCellY, "  ");
                            TargetedCellY--;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (TargetedCellY < 7)
                        {
                            Program.WriteAt(BoardStartCoordX + 18, BoardStartCoordY + TargetedCellY, "  ");
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
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if(Board[i, j] != null) {
                        if (Board[i, j].Symbol == 'K') {
                            kingsCount++;
                        }
                    }                    
                }
            }
            if (kingsCount < 1) return true;
            else return false;
        }


        private void UpdateBoardInfo()
        {
            TargetedCellSign = CellNames_Letters[TargetedCellY].ToString() + CellNames_Numbers[TargetedCellX].ToString();
            Program.WriteAt(BoardStartCoordX + 22, BoardStartCoordY, TargetedCellSign);             
            Program.WriteAt(BoardStartCoordX + 22, BoardStartCoordY + 1, "Ход: " + CurrentColorMove.ToString());             
        }



    }
}