using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sudoku
{
    public class SudokuSquare
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Block => ((Row % 3) + ((Column % 3) * 3));
        public int? Value { get; private set; }
        public bool HasValue => Value.HasValue;

        public void SetValue(int value, IEnumerable<SudokuSquare> board = null)
        {
            Value = value;
            Exclusions = SudokuIterators.OneToNine.Where(v => v != Value).ToList();
            if (board != null)
            {
                foreach (var s in board.AllRelated(this))
                {
                    if (!s.HasValue)
                    {
                        s.Exclusions.Add(value);
                    }
                }
            }
        }

        public List<int> Exclusions = new List<int>();

        public bool IsRelatedTo(SudokuSquare s) =>
               Row == s.Row
            || Column == s.Column
            || Block == s.Block;

    }

    public static class SudokuIterators
    {
        public static IEnumerable<int> ZeroToEight => Enumerable.Range(0, 9);
        public static IEnumerable<int> OneToNine => Enumerable.Range(1, 9);
        public static IEnumerable<SudokuSquare> GetRow(this IEnumerable<SudokuSquare> board, int r) => board.Where(s => s.Row == r);
        public static IEnumerable<SudokuSquare> GetColumn(this IEnumerable<SudokuSquare> board, int c) => board.Where(s => s.Column == c);
        public static IEnumerable<SudokuSquare> GetBlock(this IEnumerable<SudokuSquare> board, int b) => board.Where(s => s.Block == b);

        
        public static IEnumerable<(int, IEnumerable<SudokuSquare>)> EnumerateRows(this IEnumerable<SudokuSquare> board) =>
            ZeroToEight.Select(i => (i, board.GetRow(i)));
        public static IEnumerable<(int, IEnumerable<SudokuSquare>)> EnumerateColumns(this IEnumerable<SudokuSquare> board) =>
            ZeroToEight.Select(i => (i, board.GetColumn(i)));
        public static IEnumerable<(int, IEnumerable<SudokuSquare>)> EnumerateBlocks(this IEnumerable<SudokuSquare> board) =>
            ZeroToEight.Select(i => (i, board.GetBlock(i)));
        public static IEnumerable<int> FoundValues(this IEnumerable<SudokuSquare> board) =>
            board.Where(s => s.HasValue).Select(s => s.Value.Value);
        public static IEnumerable<SudokuSquare> CreateBoard() =>
            ZeroToEight.SelectMany(r => ZeroToEight, (r, c) => new SudokuSquare{Row = r, Column = c}).ToList();

        public static IEnumerable<SudokuSquare> AllRelated(this IEnumerable<SudokuSquare> board, SudokuSquare square) =>
            board.Where(square.IsRelatedTo);
    }

    public static class SudokuAlgorithms
    {
        public static bool TrySquareExclusions(this SudokuSquare s, out int value)
        {
            value = 0;
            if (s.HasValue)
            {
                return false;
            }

            if (s.Exclusions.Count == 8)
            {
                value = SudokuIterators.OneToNine.Except(s.Exclusions).Single();
                return true;
            }

            return false;
        }
    }
}
