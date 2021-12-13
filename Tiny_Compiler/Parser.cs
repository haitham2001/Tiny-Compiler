using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tiny_Compiler
{
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(Header());
            program.Children.Add(DeclSec());
            program.Children.Add(Block());            
            return program;
        }

        Node Header()
        {
            Node header = new Node("Header");
            
            return header;
        }
        Node DeclSec()
        {
            Node declsec = new Node("DeclSec");
            
            return declsec;
        }
        Node Block()
        {
            Node block = new Node("block");
            
            return block;
        }        

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;
                }

                else
                {
                    Errors.Parser_Error_List.Add("Parsing Error: Expected " + ExpectedToken.ToString() 
                        + "\r\n\t      Found " + TokenStream[InputPointer].token_type.ToString() + "\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Parser_Error_List.Add("Parsing Error: Expected " + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
