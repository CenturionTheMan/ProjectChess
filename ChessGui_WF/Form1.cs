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

        private void OnPlayerWon(ChessColors wonColor)
        {
            string text = (wonColor == ChessColors.WHITE) ? "WHITE WON!" : "BLACK WON!";
            currentSideLabel.Text = text;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (!ChessStringsHandler.GetSetupFromFen(initPosTextBox.Text, out uint[] board, out ChessColors? currentSide, out bool[] castling, out int enPassantPosition, out int halfmoves, out int fullmove)) return;


            Game game = new Game(board,currentSide, castling, enPassantPosition, halfmoves, fullmove);
            game.OnCurrentPlayerChanged += HandleSideShowing;
            HandleSideShowing(currentSide.Value);

            game.OnPlayerWon += OnPlayerWon;

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
        
        private void allPositionsButton_Click(object sender, EventArgs e)
        {
            _ = Task.Run(() => MoveGeneration(3));
        }

        private async Task<int> MoveGeneration(int depth)
        {
            if (depth == 0)
            {
                return 1;
            }
            var currentMoves = boardUserControl.game.GetValidMoves();
            int movesAmount = 0;

            foreach (var move in currentMoves)
            {
                this.Invoke(new Action(() =>
                {
                    boardUserControl.MakeMove(move);
                }));

                while (wasKeyDown == false)
                {
                    await Task.Delay(2);
                }
                wasKeyDown = false;

                movesAmount += await MoveGeneration(depth - 1);

                this.Invoke(new Action(() =>
                {
                    boardUserControl.UnMakeLastMove();
                }));

                //while (wasKeyDown == false)
                //{
                //    await Task.Delay(10);
                //}
                //wasKeyDown = false;
            }

            return movesAmount;
        }

        bool wasKeyDown = false;
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            wasKeyDown = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            boardUserControl.EnableBlackBot(blackBotCheckBox.Checked);
        }
    }
}