using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Sudoku;

namespace SudokuTest
{
    public class SudokuTestHelp
    {
        public static IEnumerable<SudokuSquare> LoadBoardFromText(string text)
        {
            var board = SudokuIterators.CreateBoard();
            int row = 0;
            foreach (var line in text.Split('\n'))
            {
                int col = 0;
                foreach (var c in line.Trim('\r').ToCharArray())
                {
                    if (c != ' ')
                    {
                        board.Single(s => s.Row == row && s.Column == col).SetValue(int.Parse(c.ToString()), board);
                    }

                    col++;
                }

                row++;
            }

            return board;
        }

        public static string PrintBoard(IEnumerable<SudokuSquare> board)
        {
            var text = "";
            foreach (var r in SudokuIterators.ZeroToEight)
            {
                foreach (var c in SudokuIterators.ZeroToEight)
                { 
                    var square = board.Single(s => s.Row == r && s.Column == c);
                    text += square.HasValue ? square.Value.ToString() : " ";
                }

                text += "\n";
            }

            return text;
        }
    }

    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void EasyPuzzle()
        {
            var board = SudokuTestHelp.LoadBoardFromText(
@"     41  
71 8  3 5
957 83   
1   4   2
   16 758
 91  5  3
8 2  6 14
  57     ");
            var iterate = true;
            while (iterate)
            {
                Debug.Write(SudokuTestHelp.PrintBoard(board));
                Debug.WriteLine("----------");
                iterate = false;
                foreach (var s in board)
                {
                    if (s.TrySquareExclusions(out var v))
                    {
                        s.SetValue(v, board);
                        iterate = true;
                    }

                    break;
                }
            }
        }
    }
}