using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessTests
{
    public class Position3Test
    {
        [Fact]
        public void TestMovesAmountDepth()
        {
            Assert.Equal(14, TestAllMoveGenerator.TestPositionMoveAmount("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - ", 1));
            Assert.Equal(191, TestAllMoveGenerator.TestPositionMoveAmount("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - ", 2));
            Assert.Equal(2812, TestAllMoveGenerator.TestPositionMoveAmount("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - ", 3));
            Assert.Equal(43228, TestAllMoveGenerator.TestPositionMoveAmount("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - ", 4));
            Assert.Equal(674624, TestAllMoveGenerator.TestPositionMoveAmount("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - ", 5));
            Assert.Equal(11030083, TestAllMoveGenerator.TestPositionMoveAmount("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - ", 6));
        }


    }
}
