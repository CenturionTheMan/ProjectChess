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
        private readonly Color MOVE_COLOR = Color.GreenYellow;
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

            foreach (var piecePos in game.GetPiecePositions())
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
            return;
            //if (board == null) return;
            //board.UnMakeLastMove();
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

            //List<Move> validMoves = null;
            //if (clickedMousePosFrom != null)
            //    validMoves = board.GetListOfValidMovesForCurrentSide().FindAll(m => m.FromPos == clickedMousePosFrom);



            for (int y = 0; y < Board.BOARD_SINGLE_ROW_SIZE; y++)
            {
                for (int x = 0; x < Board.BOARD_SINGLE_ROW_SIZE; x++)
                {
                    Brush brush = ((x+y)%2==0)? blackBrush : whiteBrush;

                    //if (validMoves != null && validMoves.Exists(m => m.GetTriggerPos().Equals(x, y)))
                    //{
                    //    brush = move;
                    //}
                    //else if (clickedMousePosFrom != null && clickedMousePosFrom.Equals(x, y))
                    //{
                    //    brush = clickedBrush;
                    //}

                    int xWindowPos = windowOffset.X + x * cellSize.X;
                    int yWindowPos = windowSize.Y - windowOffset.Y - (y + 1) * cellSize.Y;

                    g.FillRectangle(brush, xWindowPos, yWindowPos, cellSize.X,cellSize.Y);
                    cellsPositions[x , y] = new Vec2(xWindowPos, yWindowPos);
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
            //var moves = board.GetListOfValidMovesForCurrentSide();
            //var move = moves.Find(m => m.FromPos == clickedMousePosFrom && m.GetTriggerPos() == pos);
            //if (move != null)
            //{
            //    board.MakeMove(move);
            //    clickedMousePosFrom = null;
            //}
            //else
            //{
            //    clickedMousePosFrom = pos;
            //}
        }

        private void BoardUserControl_MouseUp(object sender, MouseEventArgs e)
        {
            //clickedMousePosFrom = null;
        }
    }
}
