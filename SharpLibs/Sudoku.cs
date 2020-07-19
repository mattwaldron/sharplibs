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
        public int Value => _value ?? 0;

        public bool HasValue => _value.HasValue;
        public void SetValue(int value)
        {
            _value = value;
            _options.Clear();
        }

        public void Forbid(int value)
        {
            _options.Remove(value);
        }

        private readonly List<int> _options = SudokuIterators.PossibleValues.ToList();
        public int NumOptions => _options.Count();
        public IReadOnlyList<int> Options => _options;

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

            foreach (var r in SudokuIterators.Indexes)
            {
                foreach (var c in SudokuIterators.Indexes)
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
                ss.Forbid(value);
            }
        }

        public IEnumerable<SudokuSquare> Squares
        {
            get
            {
                foreach (var r in SudokuIterators.Indexes)
                {
                    foreach (var c in SudokuIterators.Indexes)
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
            foreach (var c in SudokuIterators.Indexes)
            {
                yield return board[r, c];
            }
        }

        public IEnumerable<SudokuSquare> GetColumn(int c)
        {
            foreach (var r in SudokuIterators.Indexes)
            {
                yield return board[r, c];
            }
        }

        public IEnumerable<SudokuSquare> GetBlock(int b)
        {
            foreach (var r in SudokuIterators.Indexes)
            {
                foreach (var c in SudokuIterators.Indexes)
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
            SudokuIterators.Indexes.Select(GetRow);
        public IEnumerable<IEnumerable<SudokuSquare>> EnumerateColumns() =>
            SudokuIterators.Indexes.Select(GetColumn);
        public IEnumerable<IEnumerable<SudokuSquare>> EnumerateBlocks() =>
            SudokuIterators.Indexes.Select(GetBlock);
    }

    public static class SudokuIterators
    {
        public static IEnumerable<int> Indexes => Enumerable.Range(0, 9);
        public static IEnumerable<int> PossibleValues => Enumerable.Range(1, 9);

        public static IEnumerable<int> FoundValues(this IEnumerable<SudokuSquare> squares) =>
            squares.Where(s => s.HasValue).Select(s => s.Value).ToList();

        public static IEnumerable<int> UnfoundOptions(this IEnumerable<SudokuSquare> squares) =>
            PossibleValues.Except(squares.FoundValues()).ToList();

    }

    public static class SudokuAlgorithms
    {
        public static IEnumerable<SudokuBoard> IterateAllAlgorithms(this SudokuBoard b)
        {
            while (true)
            {
                SudokuSquare s;
                if (b.SquareHasOneOption(out s))
                {
                    yield return b;
                    continue;
                }
                if (b.GroupHasOneSquareWithOption(out s))
                {
                    yield return b;
                    continue;
                }

                if (b.RestrictOptionsInGroupCommonToOtherGroup())
                {
                    yield return b;
                    continue;
                }
                if (b.RemoveOptionsFromUnboundSquares())
                {
                    yield return b;
                    continue;
                }

                break;
            }
        }
        
        /// <summary>
        /// Set the values for the first square that has its options reduced to 1.
        /// </summary>
        public static bool SquareHasOneOption(this SudokuBoard board, out SudokuSquare newlySolved)
        {
            newlySolved = null;
            foreach (var s in board.Squares)
            {
                if (s.HasValue)
                {
                    continue;
                }

                if (s.NumOptions == 1)
                {
                    board.SetValue(s, s.Options.Single());
                    newlySolved = s;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// If all squares in a group (row, column, block) but one exclude the same value, then the square without the
        /// exclusion must be that value. 
        /// </summary>
        public static bool GroupHasOneSquareWithOption(this SudokuBoard board, out SudokuSquare newlyFound)
        {
            newlyFound = null;

            foreach (var enumerator in new[] {board.EnumerateRows(), board.EnumerateColumns(), board.EnumerateBlocks()})
            {
                foreach (var group in enumerator)
                {
                    var unsetSquares = group.Where(s => !s.HasValue).ToList();
                    var unsetValues = group.UnfoundOptions();
                    foreach (var v in unsetValues)
                    {
                        if (unsetSquares.Count(e => e.Options.Contains(v)) == 1)
                        {
                            newlyFound = unsetSquares.Single(e => e.Options.Contains(v));
                            board.SetValue(newlyFound, v);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// In this check, we search for options in a group that are also common to another group, and remove options
        /// from the second group not in the first.  For example, given a row, if all options for a value are in the same
        /// block, then options for that value in the block not in the given row can be removed.
        /// </summary>
        public static bool RestrictOptionsInGroupCommonToOtherGroup(this SudokuBoard board)
        {
            // There are four cases to check:
            // - rows with options in the same block
            // - columns with options in the same block
            // - blocks with options in the same row
            // - blocks with options in the same rows
            // Rows with options in the same column and vice versa should be caught as the only option
            // in a square, since the overlap between a row and a column is a single square.

            // TODO: Make the procedure here common with variable enumerators.
            // rows with options in the same block:
            foreach (var group in board.EnumerateRows())
            {
                var row = group.First().Row;
                foreach (var option in group.UnfoundOptions())
                {
                    var squaresWithOption = group.Where(s => s.Options.Contains(option));
                    if (squaresWithOption.Select(s => s.Block).Distinct().Count() == 1)
                    {
                        var block = squaresWithOption.First().Block;
                        foreach (var s in board.GetBlock(block).Where(s => s.Row != row && !s.HasValue && s.Options.Contains(option)))
                        {
                            s.Forbid(option);
                            return true;
                        }
                    }
                }
            }

            // columns with options in the same block:
            foreach (var group in board.EnumerateColumns())
            {
                var column = group.First().Column;
                foreach (var option in group.UnfoundOptions())
                {
                    var squaresWithOption = group.Where(s => s.Options.Contains(option));
                    if (squaresWithOption.Select(s => s.Block).Distinct().Count() == 1)
                    {
                        var block = squaresWithOption.First().Block;
                        foreach (var s in board.GetBlock(block).Where(s => s.Column != column && !s.HasValue && s.Options.Contains(option)))
                        {
                            s.Forbid(option);
                            return true;
                        }
                    }
                }
            }

            // blocks with options in the same row
            foreach (var group in board.EnumerateBlocks())
            {
                var block = group.First().Block;
                foreach (var option in group.UnfoundOptions())
                {
                    var squaresWithOption = group.Where(s => s.Options.Contains(option));
                    if (squaresWithOption.Select(s => s.Row).Distinct().Count() == 1)
                    {
                        var row = squaresWithOption.First().Row;
                        foreach (var s in board.GetRow(row).Where(s => s.Block != block && !s.HasValue && s.Options.Contains(option)))
                        {
                            s.Forbid(option);
                            return true;
                        }
                    }
                }
            }

            // blocks with options in the same row
            foreach (var group in board.EnumerateBlocks())
            {
                var block = group.First().Block;
                foreach (var option in group.UnfoundOptions())
                {
                    var squaresWithOption = group.Where(s => s.Options.Contains(option));
                    if (squaresWithOption.Select(s => s.Column).Distinct().Count() == 1)
                    {
                        var column = squaresWithOption.First().Column;
                        foreach (var s in board.GetColumn(column).Where(s => s.Block != block && !s.HasValue && s.Options.Contains(option)))
                        {
                            s.Forbid(option);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool RemoveUnboundOptions(IEnumerable<SudokuSquare> squares, int nvals)
        {
            var optionsRemoved = false;
            var possibleBinds = squares.Where(s => !s.HasValue && s.NumOptions == nvals).ToList();
            if (possibleBinds.Count >= nvals)
            {
                for (var i = 0; i < possibleBinds.Count; i++)
                {
                    var unboundSquares = squares.Where(s => !s.HasValue).ToList(); // un-set squares with the constraint squares removed
                    var boundSquares = new List<SudokuSquare> {possibleBinds[i]};
                    var bindingOptions = boundSquares.First().Options;
                    unboundSquares.Remove(possibleBinds[i]);
                    for (var j = i + 1; j < possibleBinds.Count; j++)
                    {
                        if (bindingOptions.Count == possibleBinds[j].Options.Count && bindingOptions.All(possibleBinds[j].Options.Contains))
                        {
                            boundSquares.Add(possibleBinds[j]);
                            unboundSquares.Remove(possibleBinds[j]);
                        }
                    }

                    if (boundSquares.Count == nvals)
                    {
                        foreach (var s in unboundSquares)
                        {
                            if (bindingOptions.Intersect(s.Options).Any())
                            {
                                foreach (var e in bindingOptions)
                                {
                                    s.Forbid(e);
                                }

                                optionsRemoved = true;
                            }
                        }
                    }
                }
            }

            return optionsRemoved;
        }

        /// <summary>
        /// Here we define 'bound' squares as those with the same options.  They are bound because (in the case of two) defining
        /// one value defines the value of the other.  Therefore, other squares in the group cannot be either of the options in
        /// the binding; e.g.: if squares have options {1, 5}, {1, 5}, {2, 4, 5}, {2, 4}.  The 5 in {2, 4, 5} can be removed
        /// because the 5 must be in either of the {1, 5} squares.
        /// </summary>
        public static bool RemoveOptionsFromUnboundSquares(this SudokuBoard b)
        {
            foreach (var enumerator in new[] {b.EnumerateRows(), b.EnumerateColumns(), b.EnumerateBlocks()})
            {
                foreach (var set in enumerator)
                {
                    for (var n = 2; n <= set.Count(s => !s.HasValue); n++)
                    {
                        if (RemoveUnboundOptions(set, n))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

    }
}
