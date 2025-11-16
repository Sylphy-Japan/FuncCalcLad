using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncCalcLab.Core.Parsing
{
    public static class ExpressionPipeline
    {

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
                if(IsOperator(c) || c == '(' || c == ')')
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

        private enum FragmentKind
        {
            None,
            Number,
            Identifier
        }
        

    }
}
