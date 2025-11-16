using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalcLab.Core.Parsing
{
    public static class ExpressionPipeline
    {
        /// <summary>
        /// 文字列をトークンに変換する。
        /// </summary>
        /// <param name="expression">入力された文字列</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static List<string> Tokenize(string expression)
        {
            var tokens = new List<string>();
            var currentToken = new StringBuilder();

            // 現在のフラグメントの種類を追跡するための変数
            FragmentKind currentKind = FragmentKind.None;

            foreach (var c in expression)
            {
                //1,空白はスキップ
                if (char.IsWhiteSpace(c))
                {
                    continue;
                }

                //2,数字か小数点を数値トークンに変換
                if (char.IsDigit(c) || c == '.')
                {
                    if (currentKind == FragmentKind.Identifier)
                    {
                        FlashCurrent();
                    }
                    currentToken.Append(c);
                    currentKind = FragmentKind.Number;
                    continue;
                }

                //3,英字を関数名トークンに変換
                if (char.IsLetter(c))
                {
                    if (currentKind == FragmentKind.Number)
                    {
                        FlashCurrent();
                    }

                    currentToken.Append(c);
                    currentKind = FragmentKind.Identifier;
                    continue;
                }

                //4, 演算子 or 括弧
                if (IsOperator(c) || c == '(' || c == ')')
                {
                    FlashCurrent();

                    tokens.Add(c.ToString());
                    continue;
                }

                //5, それ以外はエラー扱い
                throw new ArgumentException($"Invalid character in expression: {c}");
            }


            //最後に残ってるトークンを確定する。
            FlashCurrent();
            return tokens;

            void FlashCurrent()
            {
                if (currentToken.Length == 0) return;

                tokens.Add(currentToken.ToString());
                currentToken.Clear();
                currentKind = FragmentKind.None;
            }

            static bool IsOperator(char c)
            {
                return c switch
                {
                    '+' or '-' or '*' or '/' or '^' => true,
                    _ => false,
                };
            }

        }

        /// <summary>
        /// 状態を保持するための列挙型
        /// </summary>
        private enum FragmentKind
        {
            None,
            Number,
            Identifier
        }

        /// <summary>
        /// 逆ポーランド記法（RPN）のトークン列 に変換する
        /// </summary>
        public static List<string> ToRpn(IReadOnlyList<string> tokens)
        {
            var output = new List<string>();// 出力キュー
            var opStack = new Stack<string>();// 演算子スタック・関数スタック

            foreach (var token in tokens)
            {
                // 1. 数値トークン → そのまま出力へ
                if (decimal.TryParse(token, out _))
                {
                    output.Add(token);
                    continue;
                }

                // 2. 関数トークン（sin, cos など）→ スタックへ
                if (IsFunction(token))
                {
                    opStack.Push(token);
                    continue;
                }

                // 3. 演算子トークン（+ - * / ^）
                if (IsOperatorToken(token))
                {
                    while (opStack.Count > 0 &&
                           IsOperatorToken(opStack.Peek()) &&
                           (Precedence(opStack.Peek()) > Precedence(token) ||
                            (Precedence(opStack.Peek()) == Precedence(token) &&
                             !IsRightAssociative(token))))
                    {
                        // スタック上位の演算子の優先度が高い/同じなら先に出力
                        output.Add(opStack.Pop());
                    }

                    opStack.Push(token);
                    continue;
                }

                // 4. 左カッコ "(" → スタックへ
                if (token == "(")
                {
                    opStack.Push(token);
                    continue;
                }

                // 5. 右カッコ ")" → "(" までスタックから出力へ
                if (token == ")")
                {
                    while (opStack.Count > 0 && opStack.Peek() != "(")
                    {
                        output.Add(opStack.Pop());
                    }

                    if (opStack.Count == 0)
                        throw new ArgumentException("Mismatched parentheses.");

                    // "(" を捨てる
                    opStack.Pop();

                    // 直前が関数だった場合は、それも出力へ（sin x → x sin）
                    if (opStack.Count > 0 && IsFunction(opStack.Peek()))
                    {
                        output.Add(opStack.Pop());
                    }

                    continue;
                }

                // 6. それ以外はエラー
                throw new ArgumentException($"Unknown token '{token}'");
            }

            // 7. 残った演算子・関数を全部出力へ
            while (opStack.Count > 0)
            {
                var top = opStack.Pop();
                if (top == "(" || top == ")")
                    throw new ArgumentException("Mismatched parentheses.");

                output.Add(top);
            }

            return output;


        }
        

        static bool IsOperatorToken(string t)
        => t is "+" or "-" or "*" or "/" or "^";


        static bool IsFunction(string t)
    => !IsOperatorToken(t)
       && t != "("
       && t != ")"
       && !decimal.TryParse(t, out _);


        static int Precedence(string op) => op switch
        {
            "+" or "-" => 1,
            "*" or "/" => 2,
            "^" => 3,
            _ => 0,
        };

        // 右結合演算子（a ^ b ^ c = a ^ (b ^ c)）
        static bool IsRightAssociative(string op) => op == "^";
    }
}
