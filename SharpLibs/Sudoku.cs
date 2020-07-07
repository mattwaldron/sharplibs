using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Sudoku
{
    public class SudokuSquare
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Block => ((Row / 3) + ((Column / 3) * 3));
        private int? _value;
        public int Value => _value.Value;
        public bool HasValue => _value.HasValue;
        public void SetValue(int value, IEnumerable<SudokuSquare> board = null)
        {
            _value = value;
            Exclusions = SudokuIterators.OneToNine.Where(v => v != _value).ToList();
        }

        public void Exclude(int value)
        {
            if (!Exclusions.Contains(value))
            {
                Exclusions.Add(value);
            }
        }

        public List<int> Exclusions = new List<int>();

        public bool IsRelatedTo(SudokuSquare s) =>
               Row == s.Row
            || Column == s.Column
            || Block == s.Block;
    }

    public class SudokuBoard
    {
        SudokuSquare[,] board;

        public SudokuBoard()
        {
            board = new SudokuSquare[9, 9];

            foreach (var r in SudokuIterators.ZeroToEight)
            {
                foreach (var c in SudokuIterators.ZeroToEight)
                {
                    board[r, c] = new SudokuSquare { Row = r, Column = c };
                }
            }
        }

        public SudokuSquare this[int r, int c] => board[r, c];

        public void SetValue(SudokuSquare s, int value)
        {
            s.SetValue(value);
            foreach (var ss in AllRelated(s))
            {
                ss.Exclude(value);
            }
        }

        public IEnumerable<SudokuSquare> Squares
        {
            get
            {
                foreach (var r in SudokuIterators.ZeroToEight)
                {
                    foreach (var c in SudokuIterators.ZeroToEight)
                    {
                        yield return board[r, c];
                    }
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("-------------");
            foreach (var enumerator in new[] {Enumerable.Range(0, 3), Enumerable.Range(3, 3), Enumerable.Range(6, 3)})
            {
                foreach (var r in enumerator)
                {
                    sb.Append("|");
                    foreach (var enumerator2 in new[] { Enumerable.Range(0, 3), Enumerable.Range(3, 3), Enumerable.Range(6, 3) })
                    {
                        foreach (var c in enumerator2)
                        {
                            sb.Append(board[r, c].HasValue ? board[r, c].Value.ToString() : " ");
                        }
                        sb.Append("|");
                    }

                    sb.AppendLine("");
                }
                sb.AppendLine("-------------");
            }

            return sb.ToString();
        }

        public IEnumerable<SudokuSquare> GetRow(int r)
        {
            foreach (var c in SudokuIterators.ZeroToEight)
            {
                yield return board[r, c];
            }
        }

        public IEnumerable<SudokuSquare> GetColumn(int c)
        {
            foreach (var r in SudokuIterators.ZeroToEight)
            {
                yield return board[r, c];
            }
        }

        public IEnumerable<SudokuSquare> GetBlock(int b)
        {
            foreach (var r in SudokuIterators.ZeroToEight)
            {
                foreach (var c in SudokuIterators.ZeroToEight)
                {
                    var s = board[r, c];
                    if (s.Block == b)
                    {
                        yield return s;
                    }
                }
            }
        }
        

        public int NumFound => Squares.Count(s => s.HasValue);
        public int NumEmpty => Squares.Count(s => !s.HasValue);

        public bool IsValidComplete
        {
            get
            {
                if (NumEmpty != 0)
                {
                    return false;
                }
                foreach (var enumerator in new[] { EnumerateRows(), EnumerateColumns(), EnumerateBlocks() })
                {
                    foreach (var set in enumerator)
                    {
                        if (set.Select(v => v.Value).Distinct().Count() != 9)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }


        public IEnumerable<SudokuSquare> AllRelated(SudokuSquare square) =>
            Squares.Where(square.IsRelatedTo);
        public IEnumerable<IEnumerable<SudokuSquare>> EnumerateRows() =>
            SudokuIterators.ZeroToEight.Select(GetRow);
        public IEnumerable<IEnumerable<SudokuSquare>> EnumerateColumns() =>
            SudokuIterators.ZeroToEight.Select(GetColumn);
        public IEnumerable<IEnumerable<SudokuSquare>> EnumerateBlocks() =>
            SudokuIterators.ZeroToEight.Select(GetBlock);
    }

    public static class SudokuIterators
    {
        public static IEnumerable<int> ZeroToEight => Enumerable.Range(0, 9);
        public static IEnumerable<int> OneToNine => Enumerable.Range(1, 9);
       
    }

    public static class SudokuAlgorithms
    {
        public static bool TrySquareExclusions(this SudokuBoard b, out SudokuSquare newlySolved)
        {
            newlySolved = null;
            foreach (var s in b.Squares)
            {
                if (s.HasValue)
                {
                    continue;
                }

                if (s.Exclusions.Count == 8)
                {
                    var value = SudokuIterators.OneToNine.Except(s.Exclusions).Single();
                    b.SetValue(s, value);
                    newlySolved = s;
                    return true;
                }
            }
            return false;
        }

        private static SudokuSquare OnlyWithoutExclusion(IEnumerable<SudokuSquare> squares, SudokuBoard board)
        {
            var squareList = squares.ToList();
            var emptyList = squares.Where(s => !s.HasValue).ToList();
            var remaining =
                SudokuIterators.OneToNine.Except(squareList.Where(s => s.HasValue).Select(s => s.Value));
            foreach (var r in remaining)
            {
                if (emptyList.Count(e => e.Exclusions.Contains(r)) == emptyList.Count - 1)
                {
                    var sq = emptyList.Single(e => !e.Exclusions.Contains(r));
                    board.SetValue(sq, r);
                    return sq;
                }
            }

            return null;
        }

        public static bool TryOnlyWithoutExclusion(this SudokuBoard b, out SudokuSquare newlyFound)
        {
            newlyFound = null;

            foreach (var enumerator in new[] {b.EnumerateRows(), b.EnumerateColumns(), b.EnumerateBlocks()})
            {
                foreach (var set in enumerator)
                {
                    var sq = OnlyWithoutExclusion(set, b);
                    if (sq != null)
                    {
                        newlyFound = sq;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
