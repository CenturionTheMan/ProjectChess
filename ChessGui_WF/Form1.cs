using ChessLibrary.Engine;

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
            if (!FenInterpreter.GetSetupFromFen(initPosTextBox.Text, out var currentSide, out var pieces)) return;

            ChessBoard board = new ChessBoard(currentSide.Value, pieces);
            boardUserControl.InitGame(board);
        }

        private void unmakeLastMove_Click(object sender, EventArgs e)
        {
            boardUserControl.UnmakeLastMove();
        }
    }
}