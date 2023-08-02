using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChessLibrary.Engine;
using ChessLibrary.Utilities;

namespace ChessGui_WF
{
    public partial class PieceUserControl : UserControl
    {
        public Action<Vec2, Vec2> OnPieceDrop; //gridOriginalPos, gridCurrentPos
        public Action<Vec2> OnPiecePressed;

        private readonly string IMAGES_FOLDER_PATH = "Images/";

        private PieceClasses pieceClass;
        private ChessColors color;
        private Vec2 pieceWorldPos;
        private Vec2 pieceGridPos;

        private static Vec2 mouseDownPos = null;

        public PieceUserControl(PieceClasses pieceClass, ChessColors color, Vec2 pieceWorldPos, Vec2 pieceGridPos, Vec2 pieceSize)
        {
            this.pieceClass = pieceClass;
            this.color = color;
            this.pieceWorldPos = pieceWorldPos;
            this.pieceGridPos = pieceGridPos;

            string imagePath = IMAGES_FOLDER_PATH + PieceToImagePath(pieceClass, color);
            Image image = Image.FromFile(imagePath);

            InitializeComponent();

            this.BackgroundImage = image;
            this.Size = new Size(pieceSize.X, pieceSize.Y);
            this.Location = new Point(pieceWorldPos.X, pieceWorldPos.Y);
        }

        public void ResetPiecePos()
        {
            this.Location = new Point(pieceWorldPos.X, pieceWorldPos.Y);
        }

        public bool IsEqual(PieceClasses pieceClass, ChessColors color)
        {
            return this.pieceClass == pieceClass && this.color == color;
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

        private void PieceUserControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                mouseDownPos = new Vec2(e.Location.X, e.Location.Y);
                this.BringToFront();
                OnPiecePressed?.Invoke(pieceGridPos);
            }
        }

        private void PieceUserControl_MouseUp(object sender, MouseEventArgs e)
        {
            OnPieceDrop?.Invoke(pieceGridPos, BoardUserControl.WordPosToGridPos(new Vec2(this.Location.X + this.Size.Width / 2, this.Location.Y + this.Size.Height / 2)));
            mouseDownPos = null;
        }

        private void PieceUserControl_MouseLeave(object sender, EventArgs e)
        {
            //if(mouseDownPos != null)
            //OnPieceDrop?.Invoke(pieceGridPos, BoardUserControl.WordPosToGridPos(new Vec2(this.Location.X, this.Location.Y)));
            //mouseDownPos = null;
        }

        private void PieceUserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && mouseDownPos != null)
            {
                this.Left = e.X + this.Left - mouseDownPos.X;
                this.Top = e.Y + this.Top - mouseDownPos.Y;
            }
        }
    }
}
