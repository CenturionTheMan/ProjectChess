using ChessLibrary.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChessLibrary.Engine.EngineTester;

namespace ChessTests
{
    public class InitialPositionTests
    {
        //private int capturesAmount = 0;
        //private int checksAmount = 0;
        //private int checksmatesAmount = 0;
        //private int promotionsAmount = 0;
        //private int castlesAmount = 0;
        //private int enPassantCaptureAmount = 0;


        [Fact]
        public void TestMovesAmountDepth1()
        {
            Assert.Equal(20, GetNodesAmount("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 1));
        }

        [Fact]
        public void TestMovesAmountDepth2()
        {
            Assert.Equal(400, GetNodesAmount("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 2));
        }

        [Fact]
        public void TestMovesAmountDepth3()
        {
            Assert.Equal(8_902, GetNodesAmount("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 3));
        }

        [Fact]
        public void TestMovesAmountDepth4()
        {
            Assert.Equal(197_281, GetNodesAmount("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",4));
        }

        [Fact]
        public void TestMovesAmountDepth5()
        {
            Assert.Equal(4_865_609, GetNodesAmount("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",5));
        }

        [Fact]
        public void TestMovesAmountDepth6()
        {
            //Assert.Equal(119_060_324, GetNodesAmount("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 6));
        }


        

        
    }
}
