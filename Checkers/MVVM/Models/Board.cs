﻿using Caliburn.Micro;
using Checkers.MVVM.Models;
using Checkers.MVVM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Models
{
    public class Board
    {
        private readonly Piece[,] pieces= new Piece[8, 8];

        public Piece this[int row, int col]
        {
            get
            {
                return pieces[row, col];
            }
            set
            {
                pieces[row, col] = value;
            }
        }
        public Piece this[Position position]
        {
            get
            {
                return this[position.Row, position.Column];
            }
            set
            {
                this[position.Row, position.Column] = value;
            }
        }
        public static Board Initial()
        {
            Board board = new Board();
            board.AddStartPieces();
            return board;
        }

        private void AddStartPieces()
        {
            this[0,1]= new Piece(Player.White);
            this[0,3]= new Piece(Player.White);
            this[0,5]= new Piece(Player.White);
            this[0,7]= new Piece(Player.White);
            this[1,0]= new Piece(Player.White);
            this[1,2]= new Piece(Player.White);
            this[1,4]= new Piece(Player.White);
            this[1,6]= new Piece(Player.White);
            this[2,1]= new Piece(Player.White);
            this[2,3]= new Piece(Player.White);
            this[2,5]= new Piece(Player.White);
            this[2,7]= new Piece(Player.White);

            this[5,0]= new Piece(Player.Red);
            this[5,2]= new Piece(Player.Red);
            this[5,4]= new Piece(Player.Red);
            this[5,6]= new Piece(Player.Red);
            this[6,1]= new Piece(Player.Red);
            this[6,3]= new Piece(Player.Red);
            this[6,5]= new Piece(Player.Red);
            this[6,7]= new Piece(Player.Red);
            this[7,0]= new Piece(Player.Red);
            this[7,2]= new Piece(Player.Red);
            this[7,4]= new Piece(Player.Red);
            this[7,6]= new Piece(Player.Red);

        }
        public static bool IsInsideBoard(Position position)
        {
            return position.Row >= 0 && position.Row < 8 && position.Column >= 0 && position.Column < 8;
        }
        public bool IsEmpty(Position position)
        {
            return this[position] == null;
        }

        public static BindableCollection<BindableCollection<Cell>> GetInitialCells()
        {
            BindableCollection<BindableCollection<Cell>> cells = new BindableCollection<BindableCollection<Cell>>();

            //White pieces
            for (int row = 0; row < 3; row++)
            {
                BindableCollection<Cell> rowCells = new BindableCollection<Cell>();
                for (int col = 0; col < 8; col++)
                {
                    if ((row + col) % 2 == 1) 
                        rowCells.Add(new Cell { Row = row, Column = col, ImagePath = "../../Resources/whitePiece.png"});
                    else 
                        rowCells.Add(new Cell { Row = row, Column = col, ImagePath = "../../Resources/transparent.png" });
                }
               cells.Add(rowCells);
            }

            //Empty cells
            for (int row = 3; row < 5; row++)
            {
                BindableCollection<Cell> rowCells = new BindableCollection<Cell>();
                for (int col = 0; col < 8; col++)
                {
                    rowCells.Add(new Cell { Row = row, Column = col, ImagePath = "../../Resources/transparent.png" });
                }
                cells.Add(rowCells);
            }

            //Red pieces
            for (int row = 5; row < 8; row++)
            {
                BindableCollection<Cell> rowCells = new BindableCollection<Cell>();
                for (int col = 0; col < 8; col++)
                {
                    if ((row + col) % 2 == 1)
                        rowCells.Add(new Cell { Row = row, Column = col, ImagePath = "../../Resources/redPiece.png" });
                    else
                        rowCells.Add(new Cell { Row = row, Column = col, ImagePath = "../../Resources/transparent.png" });
                }
                cells.Add(rowCells);
            }

            return cells;
        }
    }
}