using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tiny_Compiler
{
    public static class Tiny_Compiler
    {
        public static Scanner Tiny_Scanner = new Scanner();
        public static Parser Tiny_Parser = new Parser();       
        public static List<Token> TokenStream = new List<Token>();
        public static List<Token> tokenStream = new List<Token>();
        public static Node tree_Root;

        public static void Start_Compiling(string SourceCode) //character by character
        {
            //Scanner
            Tiny_Scanner.StartScanning(SourceCode);
            //Parser
            for (int i = 0; i < TokenStream.Count; i++)
            {
                if (!(TokenStream[i].token_type == Token_Class.Comment))
                {
                    tokenStream.Add(TokenStream[i]);

                }  
            }
            Tiny_Parser.StartParsing(tokenStream);
            tree_Root = Tiny_Parser.root;
            //Sematic Analysis
        }
    }
}
