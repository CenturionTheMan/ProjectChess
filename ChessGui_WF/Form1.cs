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

        private void HandleSideShowing(ChessColors color)
        {
            string text = (color == ChessColors.WHITE) ? "CURRENT SIDE: WHITE" : "CURRENT SIDE: BLACK";
            currentSideLabel.Text = text;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (!ChessStringsHandler.GetSetupFromFen(initPosTextBox.Text, out uint[] board, out ChessColors? currentSide, out bool[] castling, out int enPassantPosition, out int halfmoves, out int fullmove)) return;


            Game game = new Game(board,currentSide, castling, enPassantPosition, halfmoves, fullmove);
            game.OnCurrentPlayerChanged += HandleSideShowing;
            HandleSideShowing(currentSide.Value);
            boardUserControl.InitGame(game);
        }

        private void unmakeLastMove_Click(object sender, EventArgs e)
        {
            boardUserControl.UnMakeLastMove();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void boardUserControl_MouseDown(object sender, MouseEventArgs e)
        {
            //MessageBox.Show(e.X.ToString(), e.Y.ToString());
        }
    }
}