using System;
using System.Collections.Generic;

namespace CaroChessMVC.Models
{
    public class Move
    {
        public const char Empty = '\0';
        public const char X = 'x';
        public const char O = 'o';

        public int Row { get; set; }
        public int Column { get; set; }
        public char Player { get; set; }

        public void Update(char[,] data, char value)
        {
            data[Row, Column] = Player = value;
        }
    }

    class History : Stack<Move> { }


    class CaroEventArg : EventArgs
    {
        public Move Move { get; set; }
    }

    internal class Game
    {
        char[,] data;
        public Move CurrentMove { get; set; }
        public char Player { get; set; }

        public event EventHandler<CaroEventArg> Changed;
        public event EventHandler<CaroEventArg> GameOver;

        History backward = new History();
        History forward = new History();

        private string gameIdentifier; //
        private bool isGameOver = false; //

        public Game Start()
        {
            gameIdentifier = Guid.NewGuid().ToString(); // 
            data = new char[Size, Size];
            Player = Move.O;

            backward = new History();
            forward = new History();

            isGameOver = false; //

            return this;
        }

        public void Undo()
        {
            if (backward.Count > 0)
            {
                CurrentMove = backward.Pop();
                CurrentMove.Update(data, Move.Empty);

                OnChange();
                // Swap();
            }
        }

        public int Size { get; set; } = 13;
        // public void Swap() => Player = Player == Move.X ? Move.O : Move.X;

        int calc(char player, int row, int col, int dr, int dc, bool invert)
        {
            int r = row + dr;
            int c = col + dc;
            int s = 0;
            while (r >= 0 && r < Size && c >= 0 && c < Size && data[r, c] == player)
            {
                ++s;
                r += dr;
                c += dc;
            }

            if (invert) s += calc(player, row, col, -dr, -dc, false);

            return s;

        }

        bool isWin(char player, int row, int col)
        {
            Func<int, int, bool> over = (dr, dc) => calc(player, row, col, dr, dc, true) >= 4;
            return over(0, 1) || over(1, 0) || over(1, 1) || over(-1, 1);
        }



        public void OnChange() => Changed?.Invoke(this, new CaroEventArg { Move = CurrentMove });



        public bool PutAndCheckGameOver(int row, int column)
        {
            if (data[row, column] != 0) return false;

            CurrentMove = new Move { Row = row, Column = column };
            CurrentMove.Update(data, Player);
            OnChange();

            backward.Push(CurrentMove);

            if (isWin(Player, row, column))
            {
                GameOver?.Invoke(this, new CaroEventArg { Move = CurrentMove });
                return true;
            }

            // Swap();
            return false;
        }

        public Game() { }
        public Game(int size) { Size = size; }
    }
}