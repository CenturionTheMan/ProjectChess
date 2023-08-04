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

        private static Vec2 windowSize;
        private static Vec2 windowOffset;
        private static Vec2 cellSize;

        private static Vec2[,] cellsWorldPositions;

        private PieceUserControl[] pieces = new PieceUserControl[Board.BOARD_SIZE];

        public Game game;

        private Vec2 clickedMousePosFrom = null;

        public BoardUserControl()
        {
            cellsWorldPositions = new Vec2[Board.BOARD_SINGLE_ROW_SIZE, Board.BOARD_SINGLE_ROW_SIZE];
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
            SetupPieces();
        }

        private void SetupPieces()
        {
            for (int i = 0; i < Board.BOARD_SIZE; i++)
            {
                if (game.TryGetPieceAtPosition(i, out PieceClasses pieceClass, out ChessColors color))
                {
                    if (pieces[i] != null && pieces[i].IsEqual(pieceClass, color)) continue;

                    if (pieces[i] != null)
                    {
                        pieces[i].OnPiecePressed -= HandlePiecePressed;
                        pieces[i].OnPieceDrop -= TryChangePiecePos;
                        this.Controls.Remove(pieces[i]);
                    }
                    Vec2 pieceVecPos = new Vec2(i);
                    pieces[i] = new PieceUserControl(pieceClass, color, cellsWorldPositions[pieceVecPos.X, pieceVecPos.Y], pieceVecPos, cellSize);
                    
                    pieces[i].OnPiecePressed += HandlePiecePressed;
                    pieces[i].OnPieceDrop += TryChangePiecePos;

                    this.Controls.Add(pieces[i]);
                }
                else
                {
                    if (pieces[i] == null) continue;
                    pieces[i].OnPiecePressed -= HandlePiecePressed;
                    pieces[i].OnPieceDrop -= TryChangePiecePos;
                    this.Controls.Remove(pieces[i]);
                    pieces[i] = null;
                }
            }
        }

        private void TryChangePiecePos(Vec2 originalGridPos, Vec2 tryToGridPos)
        {
            Move? move = game.GetValidMoves().FirstOrDefault(m => originalGridPos.ToBoardPosition() == m.FromPos && m.ToPos == tryToGridPos.ToBoardPosition());
            if (move != null)
            {
                game.MakeMove(move);
                SetupPieces();
            }
            else
            {
                pieces[originalGridPos.ToBoardPosition()].ResetPiecePos();
            }

            clickedMousePosFrom = null;
            this.Invalidate();
            this.Update();
        }

        public void MakeMove(Move move)
        {
            if (move != null)
            {
                game.MakeMove(move);
                SetupPieces();
            }

            clickedMousePosFrom = null;
            this.Invalidate();
            this.Update();

        }

        public void UnMakeLastMove()
        {
            game.UnMakeLastMove();
            SetupPieces();

            this.Invalidate();
            this.Update();

        }

        private void HandlePiecePressed(Vec2 gridPos)
        {
            clickedMousePosFrom = gridPos;
            this.Invalidate();
        }

        private void DrawBoard(Graphics g)
        {
            Brush whiteBrush = new SolidBrush(WHITE_CELL_COLOR);
            Brush blackBrush = new SolidBrush(BLACK_CELL_COLOR);
            Brush move = new SolidBrush(MOVE_COLOR);
            Brush clickedBrush = new SolidBrush(CLICKED_COLOR);
            Brush attackZoneBrush = new SolidBrush(Color.Red);

            Move[] validMoves = null;
            if(game != null)
            {
                validMoves = game.GetValidMoves();
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
                    else if(validMoves != null && clickedMousePosFrom != null && Array.Exists(validMoves, m => m.FromPos == clickedMousePosFrom.ToBoardPosition() && m.ToPos == new Vec2(x,y).ToBoardPosition()))
                    {
                        brush = move;
                    }

                    int xWindowPos = windowOffset.X + x * cellSize.X;
                    int yWindowPos = windowSize.Y - windowOffset.Y - (y + 1) * cellSize.Y;

                    g.FillRectangle(brush, xWindowPos, yWindowPos, cellSize.X,cellSize.Y);
                    cellsWorldPositions[x , y] = new Vec2(xWindowPos, yWindowPos);
                }
            }

            this.BackColor = BORDER_COLOR;
        }

        private void BoardUserControl_Paint(object sender, PaintEventArgs e)
        {          
            DrawBoard(e.Graphics);
        }

        private void BoardUserControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            //this.Invalidate(false);
            //MessageBox.Show($"{e.Location} || x= {indexX}, y= {indexY}");
        }

        private void BoardUserControl_MouseUp(object sender, MouseEventArgs e)
        {
            //clickedMousePosFrom = null;
        }

        public static Vec2 WordPosToGridPos(Vec2 worldPos)
        {
            int xPos = worldPos.X;
            int yPos = worldPos.Y;

            float xBoard = xPos / (float)cellSize.X;
            float yBoard = (windowSize.Y - yPos) / (float)cellSize.Y;

            int indexX = (int)Math.Floor(xBoard);
            int indexY = (int)Math.Floor(yBoard);

            indexX = Math.Clamp(indexX, 0, 7);
            indexY = Math.Clamp(indexY, 0, 7);
            return new Vec2(indexX, indexY);
        }

        public static Vec2 GridPosToWorldPos(Vec2 gridPos)
        {
            return cellsWorldPositions[gridPos.X, gridPos.Y];
        }
    }
}
