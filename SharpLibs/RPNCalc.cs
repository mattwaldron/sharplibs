using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SharpLibs.RPNCalc
{
    public enum Operator
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Push
    }

    public class CalcItem
    {
        public Operator op;
        public double value;

        public static CalcItem Parse(string s)
        {
            var item = new CalcItem();
            if (double.TryParse(s, out item.value))
            {
                item.op = Operator.Push;
            }
            else
            {
                if (s.Length > 1)
                {
                    throw new ArgumentException("If not a number, string must be a single character");
                }

                item.op = s switch
                {
                    "+" => Operator.Add,
                    "-" => Operator.Subtract,
                    "*" => Operator.Multiply,
                    "/" => Operator.Divide,
                    _ => throw new ArgumentException("Unrecognized operator")

                };
            }

            return item;
        }

        public static CalcItem Value(double v)
        {
            return new CalcItem()
            {
                op = Operator.Push,
                value = v
            };
        }

    }
    public class RPNCalc
    {
        public bool HasResult => _stack.Count == 1;
        private Stack<CalcItem> _stack = new Stack<CalcItem>();

        public void PerformSingle(CalcItem item)
        {
            if (item.op == Operator.Push)
            {
                _stack.Push(item);
            }
            else
            {
                if (_stack.Count < 2)
                {
                    throw new Exception("Not enough values to operate on");
                }

                var val2 = _stack.Pop().value;
                var val1 = _stack.Pop().value;
                PerformSingle(CalcItem.Value(item.op switch
                {
                    Operator.Add => val1 + val2,
                    Operator.Subtract => val1 - val2,
                    Operator.Multiply => val1 * val2,
                    Operator.Divide => val1 / val2
                }));
            }
        }

        public void PerformSingle(string item) => PerformSingle(CalcItem.Parse(item));

        public double SolveEquation(string input)
        {
            var items = input.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Where(s => !string.IsNullOrWhiteSpace(s));

            foreach (var c in items)
            {
                PerformSingle(c);
            }

            return HasResult ? _stack.Single().value : throw new Exception("Malformed equation");
        }
    }
}
