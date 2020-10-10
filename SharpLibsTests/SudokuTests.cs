using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using SharpLibs.Sudoku;

namespace SharpLibsTests
{
    public class SudokuTestHelp
    {

        public static SudokuBoard LoadBoardFromText(string text)
        {
            var board = new SudokuBoard();
            int row = 0;
            foreach (var line in text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
            {
                int col = 0;
                foreach (var c in line.Trim('\r').ToCharArray())
                {
                    if (c != ' ' && c != '.')
                    {
                        board.SetValue(board[row, col], int.Parse(c.ToString()));
                    }

                    col++;
                }

                row++;
            }

            return board;
        }
    }

    [TestFixture]
    public class SudokuTests
    {
        [Test]
        public void OneSquareWithOption_SingleValue_SquareFilled()
        {
            var board = SudokuTestHelp.LoadBoardFromText(
                @"
.........
...1.....
......1..
.1.......
....1....
.......1.
..1......
.....1...
........1
");
            board.GroupHasOneSquareWithOption(out var s);
            Assert.AreEqual(9, board.NumFound);
            Assert.AreEqual(0, s.Row);
            Assert.AreEqual(0, s.Column);
            Assert.AreEqual(1, s.Value);
        }

        [Test]
        public void SquareHasOneOption_SingleRow_SquareFilled()
        {
            var board = SudokuTestHelp.LoadBoardFromText(
                @"
.23456789
.........
.........
.........
.........
.........
.........
.........
.........
");
            board.SquareHasOneOption(out var s);
            Assert.AreEqual(9, board.NumFound);
            Assert.AreEqual(0, s.Row);
            Assert.AreEqual(0, s.Column);
            Assert.AreEqual(1, s.Value);
        }

        [Test]
        public void ReduceOptions_ExamplePuzzle()
        {
            var board = SudokuTestHelp.LoadBoardFromText(
                @"
5...76.8.
.78.156..
...83275.
7.15.3.26
..562....
.8.741935
.531...6.
..936.5..
.6725.3.4
");
            var set = board.GetBlock(5);
            SudokuAlgorithms.RemoveUnboundOptions(set, 2);
            Assert.AreEqual(2, board[6, 5].NumOptions);
        }

        [Test]
        public void ReduceExtraOptionsFromBoundSquares_ExamplePuzzle()
        {
            var board = SudokuTestHelp.LoadBoardFromText(
                @"
.3978.1.5
1..95.3..
...13.8.9
...82..3.
3..46...8
.2.371..4
7.3648...
..62974.3
2.45137.6
");
            board.RemoveExtraOptionsFromBoundSquares();
            Assert.AreEqual(2, board[4, 7].NumOptions);

        }

        [Test]
        public void Solve_EasyPuzzle()
        {
            var board = SudokuTestHelp.LoadBoardFromText(
@"
61..4.5.2
.54...8..
..75...4.
8.62.93.1
...3.8...
7.14.52.9
.7...69..
..3...46.
5.2.8..13
");
            foreach (var b in board.IterateAllAlgorithms())
            {
                Debug.Write(board.ToString());
                Debug.WriteLine("");
            }
            Assert.IsTrue(board.IsValidComplete);
        }

        [Test]
        public void Solve_MediumPuzzle()
        {
            var board = SudokuTestHelp.LoadBoardFromText(
                @"
.9.1.....
4.39...85
.82......
9...7.8..
32.859.17
..6.4...3
......42.
54...87.1
.....4.6.
");
            foreach (var b in board.IterateAllAlgorithms())
            {
                Debug.Write(board.ToString());
                Debug.WriteLine("");
            }
            Assert.IsTrue(board.IsValidComplete);
        }

        [Test]
        public void Solve_HardPuzzle()
        {
            var board = SudokuTestHelp.LoadBoardFromText(
                @"
.1.2.....
9..6..37.
....5...8
16.....3.
3..975..2
.2.....85
6...9....
.81..3..4
.....8.5.
");
            foreach (var b in board.IterateAllAlgorithms())
            {
                Debug.Write(board.ToString());
                Debug.WriteLine("");
            }
            Assert.IsTrue(board.IsValidComplete);
        }

        [Test]
        [TestCase(@"
1..6.8.3.
....7....
2.....64.
9.27.1.5.
.........
.3.4.59.8
.14.....7
....4....
.2.8.9..6
"
        )]
        [TestCase(@"
..3.4...5
5....9...
.6.2....8
.91...6.7
.5.....9.
3.4...85.
4....7.2.
...3....1
1...6.3..")]
        [TestCase(@"
..8..3...
3..9....8
.....4.6.
65.2.7.9.
.7.....1.
.9.3.5.27
.1.7.....
4....1..2
...6..7..
            ")]
        public void Solve_EvilPuzzle(string puzzle)
        {
            var board = SudokuTestHelp.LoadBoardFromText(puzzle);
            foreach (var b in board.IterateAllAlgorithms())
            {
                Debug.Write(board.ToString());
                Debug.WriteLine("");
            }
            Assert.IsTrue(board.IsValidComplete);
        }

        [Test]
        public void Solve_NewspaperHardPuzzle()
        {
            var board = SudokuTestHelp.LoadBoardFromText(
@"
5....6.8.
..8.156..
......7..
7.1..3.2.
....2....
.8.7..9.5
..3......
..936.5..
.6.2....4
");
            foreach (var b in board.IterateAllAlgorithms())
            {
                Debug.Write(board.ToString());
                Debug.WriteLine("");
            }

            Assert.IsTrue(board.IsValidComplete);
        }

        [Test]
        public void Solve_NewspaperSixStarPuzzle()
        {
            var board = SudokuTestHelp.LoadBoardFromText(
@"
..9.8.1.5
1..95.3..
......8.9
...82..3.
3...6...8
.2..71...
7.3......
..6.97..3
2.4.1.7..
");
            foreach (var b in board.IterateAllAlgorithms())
            {
                Debug.Write(board.ToString());
                Debug.WriteLine("");
            }

            Assert.IsTrue(board.IsValidComplete);
        }

        [Test]
        public void Solve_FiveStar20190510()
        {
            var board = SudokuTestHelp.LoadBoardFromText(
@"
.....9.43
....3.6..
5...6.78.
.1.....52
...6.8...
84.....1.
.56.7...8
..1.9....
78.5.....
");
            foreach (var b in board.IterateAllAlgorithms())
            {
                Debug.Write(board.ToString());
                Debug.WriteLine("");
            }

            Assert.IsTrue(board.IsValidComplete);
        }

        [Test]
        public void Solve_SixStar20200802()
        {
            var board = SudokuTestHelp.LoadBoardFromText(
                @"
.....563.
.6.413..7
..7.6.1..
2.5......
.1..5..7.
......4.8
..6.8.2..
4..726.8.
.215.....
");
            foreach (var b in board.IterateAllAlgorithms())
            {
                Debug.Write(board.ToString());
                Debug.WriteLine("");
            }

            Assert.IsTrue(board.IsValidComplete);
        }
    }
}