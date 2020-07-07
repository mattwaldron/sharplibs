using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Sudoku;

namespace SudokuTest
{
    public class SudokuTestHelp
    {
        public static SudokuBoard LoadBoardFromText(string text)
        {
            var board = new SudokuBoard();
            int row = 0;
            foreach (var line in text.Split('\n'))
            {
                int col = 0;
                foreach (var c in line.Trim('\r').ToCharArray())
                {
                    if (c != ' ')
                    {
                        board.SetValue(board[row,col], int.Parse(c.ToString()));
                    }

                    col++;
                }

                row++;
            }

            return board;
        }

        
    }

    public class Tests
    {

        [Test]
        public void SquareExclusions_EasyPuzzle()
        {
            var board = SudokuTestHelp.LoadBoardFromText(
@" 98  54  
  3697  8
7 1  4   
  91    2
 72   13 
4    25  
   2  3 5
9  4568  
  59  74");
            while (true)
            {
                Debug.Write(board.ToString());
                Debug.WriteLine("");
                SudokuSquare s;
                if (board.TrySquareExclusions(out s))
                {
                    continue;
                }
                break;
            }
            Assert.IsTrue(board.IsValidComplete);
        }

        [Test]
        public void OnlyWithoutExclusion_EasyPuzzle()
        {
            var board = SudokuTestHelp.LoadBoardFromText(
@" 98  54  
  3697  8
7 1  4   
  91    2
 72   13 
4    25  
   2  3 5
9  4568  
  59  74");
            while (true)
            {
                Debug.Write(board.ToString());
                Debug.WriteLine("");
                SudokuSquare s;
                if (board.TryOnlyWithoutExclusion(out s))
                {
                    continue;
                }

                break;
            }
            Assert.IsTrue(board.IsValidComplete);
        }
    }
}