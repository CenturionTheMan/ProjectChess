using ChessLibrary.Engine;
using System.Drawing;

namespace ChessGui_WF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (!ChessStringsHandler.GetSetupFromFen(initPosTextBox.Text, out uint[] board, out ChessColors? currentSide, out bool[] castling, out int enPassantPosition, out int halfmoves, out int fullmove)) return;


            Game game = new Game(board,currentSide, castling, enPassantPosition, halfmoves, fullmove);
            boardUserControl.InitGame(game);
        }

        private void unmakeLastMove_Click(object sender, EventArgs e)
        {
            boardUserControl.UnmakeLastMove();
        }
    }
}