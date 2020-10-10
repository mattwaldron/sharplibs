using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SharpLibs.RPNCalc;


namespace SharpLibsTests
{
    [TestFixture]
    public class RPNCalcTests
    {
        [Test]
        [TestCase("4 3 -", 1)]
        [TestCase("4 3 + 2 -", 5)]
        [TestCase("20 2 2 * /", 5)]
        [TestCase("362880 9 8 7 6 5 4 3 * * * * * * /", 2)]
        [TestCase("362880 9 / 8 / 7 / 6 / 5 / 4 / 3 /", 2)]
        public void EquationSolver_KnownCases(string equation, double expected)
        {
            var rpn = new RPNCalc();
            var calculated = rpn.SolveEquation(equation);
            Assert.AreEqual(expected, calculated);
        }

        [Test]
        [TestCase("3 -")]
        [TestCase("3 + 2 -")]
        [TestCase("2 2 * /")]
        [TestCase("9 8 7 6 5 4 3 * * * * * * /")]
        [TestCase("9 / 8 / 7 / 6 / 5 / 4 / 3 /")]
        public void IncorrectNumberOfOperations_TooManyOperations_ThrowsException(string equation)
        {
            var rpn = new RPNCalc();
            Assert.Throws<Exception>(() => rpn.SolveEquation(equation));
        }

        [Test]
        [TestCase("4 3")]
        [TestCase("4 3 + 2")]
        [TestCase("20 2 2 *")]
        [TestCase("362880 9 8 7 6 5 4 3 * * * * * *")]
        [TestCase("362880 9 / 8 / 7 / 6 / 5 / 4 / 3")]
        public void IncorrectNumberOfOperations_TooManyValues_ThrowsException(string equation)
        {
            var rpn = new RPNCalc();
            Assert.Throws<Exception>(() => rpn.SolveEquation(equation));
        }
    }
}
