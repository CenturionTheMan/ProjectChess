using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChessLibrary.Engine;
using ChessLibrary.Utilities;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ChessGui_WF
{
    public partial class BoardUserControl : UserControl
    {
        private readonly Color BLACK_CELL_COLOR = Color.RosyBrown;
        private readonly Color WHITE_CELL_COLOR = Color.AntiqueWhite;
        private readonly Color BORDER_COLOR = Color.SaddleBrown;
        private readonly Color MOVE_COLOR = Color.Gray;
        private readonly Color CLICKED_COLOR = Color.DarkSeaGreen;

        private readonly float BOARD_OFFSET_PERC = 0.01f;
        private readonly string IMAGES_FOLDER_PATH = "Images/";

        private Vec2 windowSize;
        private Vec2 windowOffset;
        private Vec2 cellSize;

        private Vec2[,] cellsPositions;

        private Game game;

        private Dictionary<uint, Image> imagesDic;

        private Vec2? clickedMousePosFrom = null;
        private Vec2? clickedMousePosFromPrev = null;


        public BoardUserControl()
        {
            imagesDic = new();
            cellsPositions = new Vec2[Board.BOARD_SINGLE_ROW_SIZE, Board.BOARD_SINGLE_ROW_SIZE];
            InitializeComponent();

            windowSize = new Vec2(this.Size.Width, this.Size.Height);
            windowOffset = new Vec2((int)(windowSize.X * BOARD_OFFSET_PERC) / 2, (int)(windowSize.Y * BOARD_OFFSET_PERC) / 2);
            int cellWidth = (windowSize.X - windowOffset.X * 2) / Board.BOARD_SINGLE_ROW_SIZE;
            int cellHeight = (windowSize.Y - windowOffset.Y * 2) / Board.BOARD_SINGLE_ROW_SIZE;
            cellSize = new Vec2(cellWidth, cellHeight);
        }

        public void InitGame(Game game)
        {
            this.game = game;
            var piecePositions = game.GetPiecePositions();
            foreach (var piecePos in piecePositions)
            {
                _ = game.TryGetPieceAtPosition(piecePos, out PieceClasses pieceClass, out ChessColors color);
                string imagePath = IMAGES_FOLDER_PATH + PieceToImagePath(pieceClass, color);
                Image image = Image.FromFile(imagePath);
                imagesDic.TryAdd(BoardEntityFactory.CreatePiece(pieceClass, color), image);
            }


            this.Invalidate();
        }

        public void UnmakeLastMove()
        {
            if (game == null) return;
            game.UnMakeLastMove();
            this.Invalidate();
        }

        private void ChangePieceImage(uint piece)
        {
            if(imagesDic.ContainsKey(piece))
            {
                imagesDic.Remove(piece);
            }
            _ = BoardEntityFactory.CheckIfPiece(piece, out PieceClasses pieceClass, out ChessColors color);
            string imagePath = IMAGES_FOLDER_PATH + PieceToImagePath(pieceClass, color);
            Image image = Image.FromFile(imagePath);
            imagesDic.Add(piece, image);
            this.Invalidate();
        }

        //private void DrawSpecyficSquare(Graphics g, Vec2 pos, Color color)
        //{
        //    Brush brush = new SolidBrush(color);
        //    g.FillRectangle(brush, pos.X, pos.Y, cellSize.X, cellSize.Y);

        //    if (board == null) return;

        //    if (board.GetCell(pos).HasPiece(out var piece))
        //    {
        //        if (!imagesDic.TryGetValue(piece, out Image img)) return;
        //        g.DrawImage(img, pos.X, pos.Y, cellSize.X, cellSize.Y);
        //    }
        //}

        private void DrawBoard(Graphics g)
        {
            Brush whiteBrush = new SolidBrush(WHITE_CELL_COLOR);
            Brush blackBrush = new SolidBrush(BLACK_CELL_COLOR);
            Brush move = new SolidBrush(MOVE_COLOR);
            Brush clickedBrush = new SolidBrush(CLICKED_COLOR);
            Brush attackZoneBrush = new SolidBrush(Color.Red);

            Move[] validMoves = null;
            if (clickedMousePosFrom != null)
            {
                int singleDimPos = clickedMousePosFrom.ToBoardPosition();
                validMoves = game.GetValidMoves();
                validMoves = Array.FindAll(validMoves, m => m.FromPos == singleDimPos);
            }


            for (int y = 0; y < Board.BOARD_SINGLE_ROW_SIZE; y++)
            {
                for (int x = 0; x < Board.BOARD_SINGLE_ROW_SIZE; x++)
                {
                    Brush brush = ((x+y)%2==0)? blackBrush : whiteBrush;

                    
                    if (clickedMousePosFrom != null && clickedMousePosFrom.Equals(x, y))
                    {
                        brush = clickedBrush;
                    }

                    int xWindowPos = windowOffset.X + x * cellSize.X;
                    int yWindowPos = windowSize.Y - windowOffset.Y - (y + 1) * cellSize.Y;

                    g.FillRectangle(brush, xWindowPos, yWindowPos, cellSize.X,cellSize.Y);
                    cellsPositions[x , y] = new Vec2(xWindowPos, yWindowPos);

                    //if (game != null && game.GetEnemyAttackZone()[x + 8 * y])
                    //{
                    //    g.FillEllipse(attackZoneBrush, xWindowPos + cellSize.X / 4, yWindowPos + cellSize.Y / 4, cellSize.X/2, cellSize.Y/2);
                    //}
                    if (validMoves != null && Array.Exists(validMoves, m => m.ToPos == x + 8 * y))
                    {
                        g.FillEllipse(move, xWindowPos + cellSize.X / 4, yWindowPos + cellSize.Y / 4, cellSize.X / 2, cellSize.Y / 2);
                    }
                }
            }

            this.BackColor = BORDER_COLOR;
        }


        private void DrawPieceClasses(Graphics g)
        {
            if (game == null) return;

            foreach (var piecePos in game.GetPiecePositions())
            {
                var piece = game.GetCellCode(piecePos);
                if (!imagesDic.TryGetValue(piece, out Image img)) continue;
                int y = (int)Math.Floor(piecePos / (float)8);
                int x = piecePos - y*8;
                g.DrawImage(img, cellsPositions[x, y].X, cellsPositions[x, y].Y, cellSize.X, cellSize.Y);
            }
        }

        private string PieceToImagePath(PieceClasses pClass, ChessColors color)
        {
            string path = "";
            switch (pClass)
            {
                case PieceClasses.KING:
                    path = "King";
                    break;
                case PieceClasses.QUEEN:
                    path = "Queen";
                    break;
                case PieceClasses.ROOK:
                    path = "Rook";
                    break;
                case PieceClasses.BISHOP:
                    path = "Bishop";
                    break;
                case PieceClasses.PAWN:
                    path = "Pawn";
                    break;
                case PieceClasses.KNIGHT:
                    path = "Knight";
                    break;
                default:
                    throw new Exception();
            }

            path += (color == ChessColors.WHITE) ? "W" : "B";
            path += ".png";
            return path;
        }

        private void BoardUserControl_Paint(object sender, PaintEventArgs e)
        {
            

            DrawBoard(e.Graphics);
            DrawPieceClasses(e.Graphics);
        }

        private void BoardUserControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            int xPos = e.Location.X;
            int yPos = e.Location.Y;

            float xBoard = xPos / (float)cellSize.X;
            float yBoard = (windowSize.Y - yPos) / (float)cellSize.Y;

            int indexX = (int)Math.Floor(xBoard);
            int indexY = (int)Math.Floor(yBoard);

            indexX = Math.Clamp(indexX, 0, 7);
            indexY = Math.Clamp(indexY, 0, 7);

            if (clickedMousePosFrom != null)
            {
                clickedMousePosFromPrev = clickedMousePosFrom;
                TryMakeMove(new Vec2(indexX, indexY));
            }
            else
                clickedMousePosFrom = new Vec2(indexX, indexY);

            this.Invalidate();
        }

        private void TryMakeMove(Vec2 pos)
        {
            var moves = game.GetValidMoves();
            var move = Array.Find(moves, m => m.FromPos == clickedMousePosFrom.ToBoardPosition() && m.ToPos == pos.ToBoardPosition());
            if (move != null)
            {
                game.MakeMove(move);
                clickedMousePosFrom = null;
            }
            else
            {
                clickedMousePosFrom = pos;
            }
        }

        private void BoardUserControl_MouseUp(object sender, MouseEventArgs e)
        {
            //clickedMousePosFrom = null;
        }
    }
}
