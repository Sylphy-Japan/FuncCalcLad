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


    public class RpnTests
    {
        [Fact]
        public void ToRpn_Simple()
        {
            var tokens = ExpressionPipeline.Tokenize("1 + 2 * 3");
            var rpn = ExpressionPipeline.ToRpn(tokens);

            Assert.Equal(new[] { "1", "2", "3", "*", "+" }, rpn);
        }

        [Fact]
        public void ToRpn_WithParentheses()
        {
            var tokens = ExpressionPipeline.Tokenize("(1 + 2) * 3");
            var rpn = ExpressionPipeline.ToRpn(tokens);

            Assert.Equal(new[] { "1", "2", "+", "3", "*" }, rpn);
        }

        [Fact]
        public void ToRpn_WithFunctions()
        {
            var tokens = ExpressionPipeline.Tokenize("sin(0.5) + cos(1)");
            var rpn = ExpressionPipeline.ToRpn(tokens);

            Assert.Equal(new[] { "0.5", "sin", "1", "cos", "+" }, rpn);
        }
    }
}
