using FuncCalcLab.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FuncCalcLab.Core.Parsing;

namespace FuncCalcLab.Mutable
{
    /// <summary>
    /// 電卓（ミュータブル版）
    /// </summary>
    public sealed class MutableRpnCalculator : ICalculator
    {
        private readonly Stack<decimal> _stack = new();

        public decimal Evaluate(string expression)
        {
            _stack.Clear();

            // 1. 文字列 → トークン列
            var tokens = ExpressionPipeline.Tokenize(expression);

            // 2. トークン列 → RPN
            var rpn = ExpressionPipeline.ToRpn(tokens);

            // 3. RPN をスタックで評価
            foreach (var token in rpn)
            {
                if (decimal.TryParse(token, out var number))
                {
                    // 数値トークン → スタックにプッシュ
                    _stack.Push(number);
                    continue;
                }

                // 数値以外（演算子・関数）はここで処理
                ApplyToken(token);

            }

            if (_stack.Count != 1)
                throw new InvalidOperationException("Invalid expression (stack count is not 1 at the end).");

            return _stack.Pop();
        }

        /// <summary>
        /// 数値以外のトークン（演算子・関数）を処理する
        /// </summary>
        private void ApplyToken(string token)
        {
            switch (token)
            {
                case "+":
                    {
                        var b = Pop(); // 右辺
                        var a = Pop(); // 左辺
                        _stack.Push(a + b);
                        break;
                    }

                case "-":
                    {
                        var b = Pop();
                        var a = Pop();
                        _stack.Push(a - b);
                        break;
                    }

                case "*":
                    {
                        var b = Pop();
                        var a = Pop();
                        _stack.Push(a * b);
                        break;
                    }

                case "/":
                    {
                        var b = Pop();
                        var a = Pop();
                        _stack.Push(a / b);
                        break;
                    }

                case "^":
                    {
                        var b = Pop();
                        var a = Pop();
                        _stack.Push((decimal)Math.Pow((double)a, (double)b));
                        break;
                    }

                // 関数：sin
                case "sin":
                    {
                        var x = Pop();
                        _stack.Push((decimal)Math.Sin((double)x));
                        break;
                    }

                // 関数：cos
                case "cos":
                    {
                        var x = Pop();
                        _stack.Push((decimal)Math.Cos((double)x));
                        break;
                    }

                default:
                    throw new ArgumentException($"Unknown token '{token}'");
            }
        }

        /// <summary>
        /// 安全に Pop するためのヘルパー（スタックが空なら例外）
        /// </summary>
        private decimal Pop()
        {
            if (_stack.Count == 0)
                throw new InvalidOperationException("Stack underflow while evaluating expression.");

            return _stack.Pop();
        }
    }
}
