using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Token_Class
{
    END, ENDL, ELSE, ELSEIF, IF, INTEGER,
    READ, THEN, REPEAT, UNTIL, WRITE, RETURN, FLOAT, STRING,
    Equal, LessThan, GreaterThan, NotEqual
    //Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    //GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    //Idenifier, Constant
}
namespace Tiny_Compiler
{


    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("if", Token_Class.IF);
            ReservedWords.Add("end", Token_Class.END);
            ReservedWords.Add("endl", Token_Class.ENDL);
            ReservedWords.Add("else", Token_Class.ELSE);
            ReservedWords.Add("elseif", Token_Class.ELSEIF);
            ReservedWords.Add("int", Token_Class.INTEGER);
            ReservedWords.Add("read", Token_Class.READ);
            ReservedWords.Add("then", Token_Class.THEN);
            ReservedWords.Add("repeat", Token_Class.REPEAT);
            ReservedWords.Add("until", Token_Class.UNTIL);
            ReservedWords.Add("write", Token_Class.WRITE);
            ReservedWords.Add("return", Token_Class.RETURN);
            ReservedWords.Add("float", Token_Class.FLOAT);
            ReservedWords.Add("string", Token_Class.STRING);

            Operators.Add("=", Token_Class.Equal);
            Operators.Add("<", Token_Class.LessThan);
            Operators.Add(">", Token_Class.GreaterThan);
            Operators.Add("<>", Token_Class.NotEqual);
            //Operators.Add(".", Token_Class.Dot);
            //Operators.Add(";", Token_Class.Semicolon);
            //Operators.Add(",", Token_Class.Comma);
            //Operators.Add("(", Token_Class.LParanthesis);
            //Operators.Add(")", Token_Class.RParanthesis);
            //Operators.Add("=", Token_Class.EqualOp);
            //Operators.Add("<", Token_Class.LessThanOp);
            //Operators.Add(">", Token_Class.GreaterThanOp);
            //Operators.Add("!", Token_Class.NotEqualOp);
            //Operators.Add("+", Token_Class.PlusOp);
            //Operators.Add("-", Token_Class.MinusOp);
            //Operators.Add("*", Token_Class.MultiplyOp);
            //Operators.Add("/", Token_Class.DivideOp);



        }

        public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();
                string check = "";
                
                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;
                if (char.IsLetter(CurrentChar))
                {
                    while (char.IsLetter(CurrentChar) || char.IsDigit(CurrentChar))
                    {
                        check += CurrentChar.ToString();
                        j++;
                        if (j >= SourceCode.Length)
                            break;
                        CurrentChar = SourceCode[j];

                    }
                    FindTokenClass(check);
                    i = j - 1;
                }
                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {

                }
                else if (CurrentChar == '{')
                {

                }
                else if (CurrentChar == '<')
                {
                    while (true)
                    {
                        check += CurrentChar.ToString();
                        j++;
                        if (j >= SourceCode.Length)
                            break;
                        CurrentChar = SourceCode[j];

                    }
                    FindTokenClass(check);
                    i = j - 1;
                }
                else if (CurrentChar == '=' || CurrentChar == '>')
                {

                    check += CurrentChar.ToString();
                    FindTokenClass(check);
                }
                else
                {

                }

            }

            Tiny_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }

            //Is it an identifier?


            //Is it a Constant?

            //Is it an operator?
            if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            //Is it an undefined?
        }



        bool isIdentifier(string lex)
        {
            bool isValid = true;
            // Check if the lex is an identifier or not.

            return isValid;
        }
        bool isConstant(string lex)
        {
            bool isValid = true;
            // Check if the lex is a constant (Number) or not.

            return isValid;
        }
    }
}
