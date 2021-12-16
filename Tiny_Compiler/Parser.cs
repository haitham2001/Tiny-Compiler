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
            program.Children.Add(Function_state());
            program.Children.Add(Main_Function());          
            return program;
        }

        Node Function_state()
        {
            Node Function_stat = new Node("Function_state");
            if (TokenStream[InputPointer].token_type == Token_Class.INTEGER)
            {
                Function_stat.Children.Add(Function_Statement());
                Function_stat.Children.Add(Function_state());
                return Function_stat;
            }
                return null;
        }
        Node Function_Statement()
        {
            Node Function_Statement_node = new Node("Function_Statement");
           
                Function_Statement_node.Children.Add(Function_Declaration());
                Function_Statement_node.Children.Add(Function_Body());
                return Function_Statement_node;
        }

        Node Main_Function()
        {
            Node Main_Function_node = new Node("Main_Function");
            Main_Function_node.Children.Add(Datatype());
            Main_Function_node.Children.Add(match(Token_Class.main));
            Main_Function_node.Children.Add(match(Token_Class.LeftBraces));
            Main_Function_node.Children.Add(match(Token_Class.RightBraces));
            Main_Function_node.Children.Add(Function_Body());
            return Main_Function_node;

        }

        Node Function_Body()
        {
            Node Function_Body_node = new Node("Function_Body");
            Function_Body_node.Children.Add(match(Token_Class.LeftParentheses));
            Function_Body_node.Children.Add(States());
            Function_Body_node.Children.Add(match(Token_Class.RightParentheses));
            Function_Body_node.Children.Add(Return_Statement());
            return Function_Body_node;

        }
        Node States()
        {
            Node States_node = new Node("States");
            States_node.Children.Add(Statements());
            States_node.Children.Add(States_second());
            
            return States_node;
        }

        Node States_second()
        {
            Node States_node = new Node("States_second");
            if (TokenStream[InputPointer].token_type == Token_Class.IF ||
                TokenStream[InputPointer].token_type == Token_Class.ELSEIF ||
                TokenStream[InputPointer].token_type == Token_Class.READ ||
                TokenStream[InputPointer].token_type == Token_Class.WRITE ||
                TokenStream[InputPointer].token_type == Token_Class.REPEAT ||
                TokenStream[InputPointer].token_type == Token_Class.Identifier ||
                TokenStream[InputPointer].token_type == Token_Class.FLOAT ||
                TokenStream[InputPointer].token_type == Token_Class.Division ||
                TokenStream[InputPointer].token_type == Token_Class.RETURN)
            {
                States_node.Children.Add(Statements());
                States_node.Children.Add(States_second());
                return States_node;
            }
            return null;
        }

        Node Function_Declaration()
        {
            Node Function_Declaration_node = new Node("Function_Declaration");
            Function_Declaration_node.Children.Add(Datatype());
            Function_Declaration_node.Children.Add(match(Token_Class.Identifier));
            Function_Declaration_node.Children.Add(Function_Declaration_second());
            return Function_Declaration_node;

        }
        Node Function_Declaration_second()
        {
            Node Function_Declaration_second_node = new Node("Function_Declaration_second");
            if (TokenStream[InputPointer].token_type == Token_Class.LeftParentheses)
            {
                Function_Declaration_second_node.Children.Add(match(Token_Class.LeftParentheses));
                Function_Declaration_second_node.Children.Add(Function_Declaration_third());
                Function_Declaration_second_node.Children.Add(match(Token_Class.RightParentheses));
                return Function_Declaration_second_node;
            }
            return null;

        }
        Node Function_Declaration_third()
        {
            Node Function_Declaration_third_node = new Node("Function_Declaration_third");
            Function_Declaration_third_node.Children.Add(Parameter());
            Function_Declaration_third_node.Children.Add(Function_Declaration_fourth());
            Function_Declaration_third_node.Children.Add(match(Token_Class.RightParentheses));
            return Function_Declaration_third_node;

        }
        Node Function_Declaration_fourth()
        {
            Node Function_Declaration_fourth_node = new Node("Function_Declaration_fourth");
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                Function_Declaration_fourth_node.Children.Add(match(Token_Class.Comma));
                Function_Declaration_fourth_node.Children.Add(Function_Declaration_third());
                return Function_Declaration_fourth_node;
            }
            return null;

        }

        Node Function_Name()
        {
            Node Function_Name_node = new Node("Function_Name");
            Function_Name_node.Children.Add(match(Token_Class.Identifier));
            return Function_Name_node;
        }
        Node Parameter()
        {
            Node Parameter_node = new Node("Parameter");
            Parameter_node.Children.Add(Datatype());
            Parameter_node.Children.Add(match(Token_Class.Identifier));
            return Parameter_node;

        }
        Node Repeat_Statements()
        {
            Node Repeat_Statements_node = new Node("Repeat_Statements");
            Repeat_Statements_node.Children.Add(match(Token_Class.REPEAT));
            Repeat_Statements_node.Children.Add(Statements());
            Repeat_Statements_node.Children.Add(match(Token_Class.UNTIL));
            Repeat_Statements_node.Children.Add(Condition_Statement());
            return Repeat_Statements_node;
        }
        Node Else_Statement()
        {
            Node Else_Statement_node = new Node("Else_Statement");
            Else_Statement_node.Children.Add(match(Token_Class.ELSE));
            Else_Statement_node.Children.Add(Statements());
            Else_Statement_node.Children.Add(match(Token_Class.END));
            return Else_Statement_node;
        }
       

        Node Condition_Statement()
        {
            Node Condition_Statement_node = new Node("Condition_Statement");
            return Condition_Statement_node;

        }

        Node Statements()
        {
            Node statement = new Node("statement");
            if(TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                statement.Children.Add(Assignment_Statement());
                return statement;
            }
            return null;
        }
        Node Term()
        {
            Node term = new Node("term");
            if (Token_Class.Number == TokenStream[InputPointer].token_type)
            {
                term.Children.Add(match(Token_Class.Number));
            }
            else if(Token_Class.Identifier == TokenStream[InputPointer].token_type)
            {
                term.Children.Add(match(Token_Class.Identifier));
            }
            else
            {
                term.Children.Add(Function_Call());
            }
            return term;
        }
        Node Function_Call()
        {
            Node fn_call = new Node("function call");
            fn_call.Children.Add(match(Token_Class.Identifier));
            fn_call.Children.Add(match(Token_Class.LeftParentheses));
            fn_call.Children.Add(ArgList());
            fn_call.Children.Add(match(Token_Class.RightParentheses));
            return fn_call;
        }
        Node ArgList()
        {
            Node arglist = new Node("arglist");
            if(TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                arglist.Children.Add(match(Token_Class.Identifier));
                arglist.Children.Add(Arguments());
                return arglist;
            }
            return null;
        }
        Node Arguments()
        {
            Node arguments = new Node("arguments");
            if(Token_Class.Comma == TokenStream[InputPointer].token_type)
            {
                arguments.Children.Add(match(Token_Class.Comma));
                arguments.Children.Add(ArgList());
                return arguments;
            }
            
            return null;
        }
        Node Expression()
        {
            Node expression = new Node("expression");
            if(Token_Class.Comma == TokenStream[InputPointer].token_type)
            {
                expression.Children.Add(match(Token_Class.String));
            }
            
            //expression.Children.Add(Term());
            //expression.Children.Add(Equation());
            return expression;
        }
       Node Assignment_Statement()
        {
            Node assign = new Node("assignment statement");
            assign.Children.Add(match(Token_Class.Identifier));
            assign.Children.Add(match(Token_Class.Assign));
            assign.Children.Add(Expression());
            return assign;
        }
        Node Datatype()
        {
            Node dt = new Node("datatype");
            if (Token_Class.INTEGER == TokenStream[InputPointer].token_type)
            {
                dt.Children.Add(match(Token_Class.INTEGER));
            }
            else if (Token_Class.FLOAT == TokenStream[InputPointer].token_type)
            {
                dt.Children.Add(match(Token_Class.FLOAT));
            }
            else if (Token_Class.STRING == TokenStream[InputPointer].token_type)
            {
                dt.Children.Add(match(Token_Class.STRING));
            }
            return dt;
        }

        Node Write_Statement()
        {
            Node write_statement = new Node("write statement");
            write_statement.Children.Add(match(Token_Class.WRITE));
            write_statement.Children.Add(Expression());
            write_statement.Children.Add(Endline());
            return write_statement;
        }
        Node Endline()
        {
            Node el = new Node("endline");
            if(Token_Class.ENDL == TokenStream[InputPointer].token_type)
            {
                el.Children.Add(match(Token_Class.ENDL));
                return el;
            }
            return null;           
        }
        Node Read_Statement()
        {
            Node read_statement = new Node("read statement");
            read_statement.Children.Add(match(Token_Class.READ));
            read_statement.Children.Add(match(Token_Class.Identifier));
            read_statement.Children.Add(match(Token_Class.Semicolon));
            return read_statement;
        }
        
        Node Return_Statement()
        {
            Node return_statement = new Node("return statement");
            return_statement.Children.Add(match(Token_Class.RETURN));
            return_statement.Children.Add(Expression());
            return_statement.Children.Add(match(Token_Class.Semicolon));
            return return_statement;
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
