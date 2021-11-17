using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public enum Token_Class
{
    END, ENDL, ELSE, ELSEIF, IF, INTEGER,
    READ, THEN, REPEAT, UNTIL, WRITE, RETURN, FLOAT, STRING, LessThan, GreaterThan, NotEqual,
    Plus, Minus, Multiply, Divide, Equal,LeftBraces,RightBraces,
    And, Or, Semicolon, Identifier,Assign,Comment
    //Dot, Comma, LParanthesis, RParanthesis,
    //Constant
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
            Operators.Add("+", Token_Class.Plus);
            Operators.Add("-", Token_Class.Minus);
            Operators.Add("*", Token_Class.Multiply);
            Operators.Add("/", Token_Class.Divide);
            Operators.Add("&&", Token_Class.And);
            Operators.Add("||", Token_Class.Or);
            Operators.Add(":=", Token_Class.Assign);
            //Operators.Add(".", Token_Class.Dot);
            Operators.Add(";", Token_Class.Semicolon);
            //Operators.Add(",", Token_Class.Comma);
            //Operators.Add("(", Token_Class.LParanthesis);
            //Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("{", Token_Class.LeftBraces);
            Operators.Add("}", Token_Class.RightBraces);

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
                else if (CurrentChar == ':')
                {

                    if (SourceCode[j + 1] == '=')
                    {

                        for (int m = 0; m <= 1; m++)
                        {
                            check += CurrentChar.ToString();
                            if (m != 1)
                            {
                                j++;
                                CurrentChar = SourceCode[j];
                            }
                        }
                        i = j;
                    }

                    FindTokenClass(check);
                }
                else if (CurrentChar == '<')
                {

                    check += CurrentChar.ToString();
                    j++;
                    CurrentChar = SourceCode[j];
                    if (SourceCode[j] == '>')
                    {
                        check += CurrentChar.ToString();
                        i = j;
                    }

                    FindTokenClass(check);

                }
                else if (CurrentChar == '=' || CurrentChar == '>')
                {
                    check += CurrentChar.ToString();
                    FindTokenClass(check);
                }
                else if (CurrentChar == '/' && SourceCode[j + 1] == '*')
                {

                    while (true)
                    {
                        check += CurrentChar.ToString();
                        j++;
                        
                        if (j >= SourceCode.Length)
                            break;
                        CurrentChar = SourceCode[j];
                        if (CurrentChar == '*' && SourceCode[j + 1] == '/')
                        {
                            check += CurrentChar.ToString();
                            check += SourceCode[j + 1].ToString();
                            break;
                        }
                        

                    }


                    i = j+1;
                    FindTokenClass(check);
                }
                else if (CurrentChar == '+' || CurrentChar == '-' || CurrentChar == '/' || CurrentChar == '*')
                {
                    check += CurrentChar.ToString();
                    FindTokenClass(check);
                }
                else if (char.IsSymbol(CurrentChar))
                {
                    while (char.IsSymbol(CurrentChar))
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
                else
                {
                    Errors.Error_List.Add(CurrentChar.ToString());
                }

            }

            Tiny_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);

            }
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Identifier;
                Tokens.Add(Tok);
            }
            else if (isComment(Lex))
            {
                Tok.token_type = Token_Class.Comment;
                Tokens.Add(Tok);
            }

            //Is it a Constant?

            //Is it an operator?
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            else
            {
                Errors.Error_List.Add(Lex);
            }

        }



        bool isIdentifier(string lex)
        {
            bool isValid = false;
            var iden_reg = new Regex("^[A-Za-z]([A-Za-z0-9])*$", RegexOptions.Compiled);
            if (iden_reg.IsMatch(lex))
            {
                isValid = true;
            }
            return isValid;
        }
        bool isConstant(string lex)
        {
            bool isValid = true;
            // Check if the lex is a constant (Number) or not.

            return isValid;
        }

        bool isComment(string lex)
        {
            bool isValid = false;
            var word_reg = new Regex("^(/\\*).*(\\*/)$", RegexOptions.Compiled);
            if (word_reg.IsMatch(lex))
            {
                isValid = true;
            }
            return isValid;
        }
    }
}
