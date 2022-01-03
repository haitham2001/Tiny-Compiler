using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public enum Token_Class
{
    END, ENDL, ELSE, ELSEIF, IF, INTEGER,main,
    READ, THEN, REPEAT, UNTIL, WRITE, RETURN, FLOAT, STRING,LessThan, GreaterThan, NotEqual,
    Plus, Minus, Multiply, Division, Equal,LeftBraces,RightBraces,
    And, Or, Semicolon, Identifier,Assign,Comment, Comma, Number, String
    , LeftParentheses, RightParentheses
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
            ReservedWords.Add("main", Token_Class.main);

            Operators.Add("=", Token_Class.Equal);
            Operators.Add("<", Token_Class.LessThan);
            Operators.Add(">", Token_Class.GreaterThan);
            Operators.Add("<>", Token_Class.NotEqual);
            Operators.Add("+", Token_Class.Plus);
            Operators.Add("-", Token_Class.Minus);
            Operators.Add("*", Token_Class.Multiply);
            Operators.Add("/", Token_Class.Division);
            Operators.Add("&&", Token_Class.And);
            Operators.Add("||", Token_Class.Or);
            Operators.Add(":=", Token_Class.Assign);           
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LeftParentheses);
            Operators.Add(")", Token_Class.RightParentheses);
            Operators.Add("{", Token_Class.LeftBraces);
            Operators.Add("}", Token_Class.RightBraces);

        }

        public void StartScanning(string SourceCode)
        {
            int num_braces_left = 0;
            int num_LParanthesis_left = 0;
            for (int i = 0; i < SourceCode.Length; i++)
            {
                
                char CurrentChar = SourceCode[i];
                string check = "";
               
                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n'||CurrentChar=='\t')
                    continue;
                if (char.IsLetter(CurrentChar))
                {
                    while (char.IsLetter(CurrentChar) || char.IsDigit(CurrentChar))
                    {
                        check += CurrentChar.ToString();
                        i++;
                        if (i >= SourceCode.Length)
                            break;
                        CurrentChar = SourceCode[i];
                    }
                    FindTokenClass(check);
                    i = i - 1;
                }

                //For Number
                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    while (char.IsNumber(CurrentChar) || CurrentChar == '.' || char.IsLetter(CurrentChar))
                    {
                        check += CurrentChar.ToString();
                        i++;
                        if (i >= SourceCode.Length)
                            break;
                        CurrentChar = SourceCode[i];
                    }
                    FindTokenClass(check);
                    i = i - 1;
                }

                //For Strings
                else if (CurrentChar == '\"')
                {
                    check += CurrentChar.ToString();
                    i++;
                    CurrentChar = SourceCode[i];
                    while (CurrentChar != '\"')
                    {
                        check += CurrentChar.ToString();
                        if (i + 1 >= SourceCode.Length || SourceCode[i + 1] == '\n'
                            || SourceCode[i + 1] == '\r')
                            break;
                        i++;
                        CurrentChar = SourceCode[i];
                    }
                    if (CurrentChar == '\"')
                        check += CurrentChar.ToString();

                    FindTokenClass(check);

                }

                else if (CurrentChar == '.' || CurrentChar == ',' || CurrentChar == ';' || CurrentChar == '=' || CurrentChar == '>')
                {
                    check += CurrentChar.ToString();
                    FindTokenClass(check);

                }
                else if (CurrentChar == '{')
                {

                    num_braces_left++;
                    check += CurrentChar.ToString();

                    CurrentChar = SourceCode[i];
                    FindTokenClass(check);
                }
                else if (CurrentChar == '}')
                {
                    if (num_braces_left > 0)
                    {
                        num_braces_left--;
                        check += CurrentChar.ToString();

                        FindTokenClass(check);

                    }
                    else
                    {
                        Errors.Error_List.Add(CurrentChar.ToString());
                    }

                }
                else if (CurrentChar == '(')
                {

                    num_LParanthesis_left++;
                    check += CurrentChar.ToString();
                    CurrentChar = SourceCode[i];
                    FindTokenClass(check);

                }
                else if (CurrentChar == ')')
                {
                    if (num_LParanthesis_left > 0)
                    {
                        num_LParanthesis_left--;
                        check += CurrentChar.ToString();

                        CurrentChar = SourceCode[i];
                        FindTokenClass(check);

                    }
                    else
                    {
                        Errors.Error_List.Add(CurrentChar.ToString());
                    }

                }
                else if (CurrentChar == ':')
                {
                    if (SourceCode[i + 1] == '=')
                    {
                        for (int m = 0; m <= 1; m++)
                        {
                            check += CurrentChar.ToString();
                            if (m != 1)
                            {
                                i++;
                                CurrentChar = SourceCode[i];
                            }
                        }

                    }

                    FindTokenClass(check);
                }
                else if (CurrentChar == '<')
                {

                    check += CurrentChar.ToString();
                    i++;
                    CurrentChar = SourceCode[i];
                    if (SourceCode[i] == '>')
                    {
                        check += CurrentChar.ToString();

                    }
                    FindTokenClass(check);

                }
                
                else if (CurrentChar == '/' && SourceCode[i + 1] == '*')
                {
                    while (true)
                    {
                        check += CurrentChar.ToString();
                        i++;

                        if (i >= SourceCode.Length)
                            break;
                        CurrentChar = SourceCode[i];
                        if (CurrentChar == '*' && SourceCode[i + 1] == '/')
                        {
                            check += CurrentChar.ToString();
                            check += SourceCode[i + 1].ToString();
                            break;
                        }
                    }
                    i++;
                    FindTokenClass(check);
                }
                else if (CurrentChar == '+' || CurrentChar == '-' || CurrentChar == '/' || CurrentChar == '*')
                {
                    check += CurrentChar.ToString();
                    FindTokenClass(check);
                }
                else if (CurrentChar == '&' && SourceCode[i + 1] == '&') 
                {
                    check += CurrentChar.ToString();
                    i++;
                    check += CurrentChar.ToString();
                    FindTokenClass(check);
                }
                else if (char.IsSymbol(CurrentChar))
                {
                    while (char.IsSymbol(CurrentChar))
                    {
                        check += CurrentChar.ToString();
                        i++;
                        if (i >= SourceCode.Length)
                            break;
                        CurrentChar = SourceCode[i];
                    }
                    FindTokenClass(check);
                    i = i - 1;
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

            //Is it an identifier
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Identifier;
                Tokens.Add(Tok);
            }

            //Is it a Comment
            else if (isComment(Lex))
            {
                Tok.token_type = Token_Class.Comment;
                Tokens.Add(Tok);
            }

            //Is it a Constant?
            else if (isNumber(Lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
            }

            //is it a String
            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.String;
                Tokens.Add(Tok);
            }

            //Is it an Operator?
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
        bool isString(string lex)
        {
            bool isValid = false;
            var str_regx = new Regex("^(\")(.*)(\")$", RegexOptions.Compiled);
            if (str_regx.IsMatch(lex))
            {
                isValid = true;
            }
            return isValid;
        }

        bool isNumber(string lex)
        {
            bool isValid = false;
            // Check if the lex is a constant (Number) or not.
            var regx = new Regex(@"^[0-9]+(\.[0-9]+)?$", RegexOptions.Compiled);
            if (regx.IsMatch(lex))
            {
                isValid = true;
            }
            return isValid;
        }

        bool isComment(string lex)
        {
            bool isValid = false;
            var word_reg = new Regex(@"^(/\\*)(.|\s)*(\\*/)$", RegexOptions.Compiled);
            if (word_reg.IsMatch(lex))
            {
                isValid = true;
            }
            return isValid;
        }
    }
}
