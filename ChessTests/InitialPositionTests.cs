using ChessLibrary.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessTests
{
    public class InitialPositionTests
    {
        private int capturesAmount = 0;
        private int checksAmount = 0;
        private int checksmatesAmount = 0;
        private int promotionsAmount = 0;
        private int castlesAmount = 0;
        private int enPassantCaptureAmount = 0;


        [Fact]
        public void TestMovesAmountDepth1()
        {
            Assert.Equal(20, TestDepth(1));
        }

        [Fact]
        public void TestMovesAmountDepth2()
        {
            Assert.Equal(400, TestDepth(2));
        }

        [Fact]
        public void TestMovesAmountDepth3()
        {
            Assert.Equal(8_902, TestDepth(3));
        }

        [Fact]
        public void TestMovesAmountDepth4()
        {
            Assert.Equal(197_281, TestDepth(4));
        }

        [Fact]
        public void TestMovesAmountDepth5()
        {
            Assert.Equal(4_865_609, TestDepth(5));
        }

        [Fact]
        public void TestMovesAmountDepth6()
        {
            Assert.True(false);
            Assert.Equal(119_060_324, TestDepth(6));
        }


        private int TestDepth(int depth)
        {
            ChessBoard board = new ChessBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w");

            int movesAmount = MoveGeneration(board, depth);
            return movesAmount;
        }

        private void ResetVariables()
        {
            capturesAmount = 0;
            checksAmount = 0;
            checksmatesAmount = 0;
            promotionsAmount = 0;
            castlesAmount = 0;
            enPassantCaptureAmount = 0;
        }

        private int MoveGeneration(ChessBoard board, int depth)
        {
            if (depth == 0)
            {
                return 1;
            }
            var currentMoves = board.GetListOfValidMovesForCurrentSide();
            int movesAmount = 0;

            foreach (var move in currentMoves)
            {
                board.MakeMove(move);
                movesAmount += MoveGeneration(board, depth - 1);
                board.UnMakeLastMove();
            }

            return movesAmount;
        }

        private void Castling()
        {
            castlesAmount++;
        }

        private void EnPassant()
        {
            enPassantCaptureAmount++;
        }

        private void Check()
        {
            checksAmount++;
        }

        private void Checkmate()
        {
            checksmatesAmount++;
        }

        private void PieceCaptured(ChessPiece piece)
        {
            capturesAmount++;
        }

        private void PawnPromotion(ChessPiece piece)
        {
            promotionsAmount++;
        }
    }
}
