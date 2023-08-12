﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessTests
{
    //https://www.chessprogramming.org/Perft_Results
    public class PositionsTest
    {
        [Fact]
        public void Position2TestMovesAmountDepth()
        {
            Assert.Equal(48, TestAllMoveGenerator.TestPositionMoveAmount("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - ", 1));
            Assert.Equal(2039, TestAllMoveGenerator.TestPositionMoveAmount("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - ", 2));
            Assert.Equal(97862, TestAllMoveGenerator.TestPositionMoveAmount("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - ", 3));
            Assert.Equal(4085603, TestAllMoveGenerator.TestPositionMoveAmount("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - ", 4));
            //Assert.Equal(, TestAllMoveGenerator.TestPositionMoveAmount("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - ", 5));
            //Assert.Equal(, TestAllMoveGenerator.TestPositionMoveAmount("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - ", 6));
        }

        [Fact]
        public void Position3TestMovesAmountDepth()
        {
            Assert.Equal(14, TestAllMoveGenerator.TestPositionMoveAmount("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - ", 1));
            Assert.Equal(191, TestAllMoveGenerator.TestPositionMoveAmount("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - ", 2));
            Assert.Equal(2812, TestAllMoveGenerator.TestPositionMoveAmount("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - ", 3));
            Assert.Equal(43238, TestAllMoveGenerator.TestPositionMoveAmount("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - ", 4));
            Assert.Equal(674624, TestAllMoveGenerator.TestPositionMoveAmount("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - ", 5));
            Assert.Equal(11030083, TestAllMoveGenerator.TestPositionMoveAmount("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - ", 6));
        }

        [Fact]
        public void Position4TestMovesAmountDepth()
        {
            Assert.Equal(6, TestAllMoveGenerator.TestPositionMoveAmount("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 1));
            Assert.Equal(264, TestAllMoveGenerator.TestPositionMoveAmount("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 2));
            Assert.Equal(9467, TestAllMoveGenerator.TestPositionMoveAmount("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 3));
            Assert.Equal(422333, TestAllMoveGenerator.TestPositionMoveAmount("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 4));
            Assert.Equal(15833292, TestAllMoveGenerator.TestPositionMoveAmount("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 5));
            Assert.Equal(706045033, TestAllMoveGenerator.TestPositionMoveAmount("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 6));
        }

        [Fact]
        public void Position5TestMovesAmountDepth()
        {
            Assert.Equal(44, TestAllMoveGenerator.TestPositionMoveAmount("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 1));
            Assert.Equal(1486, TestAllMoveGenerator.TestPositionMoveAmount("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 2));
            Assert.Equal(62379, TestAllMoveGenerator.TestPositionMoveAmount("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 3));
            Assert.Equal(2103487, TestAllMoveGenerator.TestPositionMoveAmount("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 4));
            Assert.Equal(89941194, TestAllMoveGenerator.TestPositionMoveAmount("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 5));
        }
    }
}
