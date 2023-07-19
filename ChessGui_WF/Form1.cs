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
            if (!FenInterpreter.GetSetupFromFen(initPosTextBox.Text, out var currentSide, out var PieceClasses)) return;

            GameManager board = new GameManager(currentSide.Value, PieceClasses);
            boardUserControl.InitGame(board);
        }

        private void unmakeLastMove_Click(object sender, EventArgs e)
        {
            boardUserControl.UnmakeLastMove();
        }
    }
}