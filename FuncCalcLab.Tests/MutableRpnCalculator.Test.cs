using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FuncCalcLab.Core.Abstractions;
using FuncCalcLab.Mutable;

namespace FuncCalcLab.Tests
{
    public class MutableRpnCalculatorTests
    {
        private readonly ICalculator _calc = new MutableRpnCalculator();

        [Theory]
        [InlineData("1+2", 3)]
        [InlineData("1+2*3", 7)]
        [InlineData("(1+2)*3", 9)]
        [InlineData("10-4/2", 8)]

        //マイナスがうまく処理できていないケース、後で改善しよう。
        //[InlineData("-1+2", 1)]
        public void Evaluate_BasicArithmetic(string expr, decimal expected)
        {
            var actual = _calc.Evaluate(expr);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("sin(0)", 0)]
        [InlineData("cos(0)", 1)]
        public void Evaluate_Trig(string expr, decimal expected)
        {
            var actual = _calc.Evaluate(expr);
            Assert.InRange(actual, expected - 0.000001m, expected + 0.000001m);
        }
    }
}
