using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncCalcLab.Core.Parsing;

namespace FuncCalcLab.Tests
{
    public class TokenizeTests
    {
        [Theory]
        [InlineData("1+2", new[] { "1", "+", "2" })]
        [InlineData("1 + 2 * 3", new[] { "1", "+", "2", "*", "3" })]
        [InlineData("(1+2)*3", new[] { "(", "1", "+", "2", ")", "*", "3" })]
        [InlineData("sin(0.5)+cos(1)", new[] { "sin", "(", "0.5", ")", "+", "cos", "(", "1", ")" })]
        public void Tokenize_Works(string expression, string[] expected)
        {
            var tokens = ExpressionPipeline.Tokenize(expression);

            Assert.Equal(expected, tokens);
        }
    }
}
