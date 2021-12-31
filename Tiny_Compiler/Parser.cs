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
        Node Statements()
        {
            Node statement = new Node("statement");
            if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                if (TokenStream[InputPointer + 1].token_type == Token_Class.Assign)
                {
                    statement.Children.Add(Assignment_Statement());
                }
                else if (Is_Boolean(InputPointer + 1))
                {
                    statement.Children.Add(Condition_Statement());
                }
                else if (TokenStream[InputPointer + 1].token_type == Token_Class.LeftParentheses)
                {
                    statement.Children.Add(Function_Call());
                }
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.WRITE)
            {
                statement.Children.Add(Write_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.RETURN)
            {
                statement.Children.Add(Return_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.READ)
            {
                statement.Children.Add(Read_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.IF)
            {
                statement.Children.Add(if_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.ELSE)
            {
                statement.Children.Add(Else_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.ELSEIF)
            {
                statement.Children.Add(Else_if_statement());
            }
            else if (Is_Datatype(InputPointer))
            {
                statement.Children.Add(Datatype());
            }
            return statement;
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
            if (InputPointer + 1 < TokenStream.Count)
            {
                if (TokenStream[InputPointer + 1].token_type == Token_Class.INTEGER)
                {
                    Function_stat.Children.Add(Function_Statement());
                    Function_stat.Children.Add(Function_state());
                    return Function_stat;
                }
                else
                    return null;
            }
            return Function_stat;
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
            if (InputPointer < TokenStream.Count)
            {
                Node Main_Function_node = new Node("Main_Function");
                Main_Function_node.Children.Add(Datatype());
                Main_Function_node.Children.Add(match(Token_Class.main));
                Main_Function_node.Children.Add(match(Token_Class.LeftParentheses));
                Main_Function_node.Children.Add(match(Token_Class.RightParentheses));
                Main_Function_node.Children.Add(Function_Body());
                return Main_Function_node;
            }
            return null;
        }

        Node Function_Body()
        {
            Node Function_Body_node = new Node("Function_Body");
            Function_Body_node.Children.Add(match(Token_Class.LeftBraces));   
            
            Function_Body_node.Children.Add(States());

            //if (!(InputPointer + 1 == TokenStream.Count))
            //    Function_Body_node.Children.Add(Return_Statement());
            
            Function_Body_node.Children.Add(match(Token_Class.RightBraces));          
            return Function_Body_node;
        }
        Node States()
        {
            Node States_node = new Node("States");
            States_node.Children.Add(Statements());
            States_node.Children.Add(States_Repeatition());
            return States_node;
        }
        Node States_Repeatition()
        {
            Node states_repeat = new Node("state_repeatition");
            if (InputPointer < TokenStream.Count && Is_Statement(InputPointer))
            {
                states_repeat.Children.Add(Statements());
                states_repeat.Children.Add(States_Repeatition());
                return states_repeat;
            }
            return null;            
        }
        Node Function_Declaration()
        {
            Node Function_Declaration_node = new Node("Function_Declaration");
            Function_Declaration_node.Children.Add(Datatype());
            Function_Declaration_node.Children.Add(match(Token_Class.Identifier));           
            Function_Declaration_node.Children.Add(Parameters());                        
            return Function_Declaration_node;
        }
        Node Parameters()
        {
            Node parameters = new Node("Function_Declaration_second");
            if (InputPointer < TokenStream.Count)
            {                             
                parameters.Children.Add(match(Token_Class.LeftParentheses));                
                parameters.Children.Add(Parameter_List());
                parameters.Children.Add(match(Token_Class.RightParentheses));
                return parameters;               
            }
            Errors.Error_List.Add("Parsing Error: You Must Enter the Parameters/Arguments\r\n");
            return null;

        }
        Node Parameter_List()
        {
            Node parameter_list = new Node("Function_Declaration_third");
            if(InputPointer < TokenStream.Count)
            {
                if (!(TokenStream[InputPointer].token_type == Token_Class.RightParentheses))
                {
                    parameter_list.Children.Add(Parameter());
                    parameter_list.Children.Add(Parameter_Repeatition());
                    return parameter_list;
                }
            }           
            return null;
        }
        Node Parameter_Repeatition()
        {
            Node parameter_repeatition = new Node("Parameter Repeatition");
            if(InputPointer<TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    parameter_repeatition.Children.Add(match(Token_Class.Comma));
                    parameter_repeatition.Children.Add(Parameter());
                    parameter_repeatition.Children.Add(Parameter_Repeatition());
                    return parameter_repeatition;
                }
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
        Node Declaration_statements()
        {
            Node declare = new Node("declaration statements");
            declare.Children.Add(Datatype());
            declare.Children.Add(Declare_A());
            declare.Children.Add(match(Token_Class.Semicolon));
            return declare;
        }
        Node Declare_A()
        {
            Node declare_a = new Node("declare_a");
            if(TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                declare_a.Children.Add(match(Token_Class.Identifier));
            }
            else
            {
                declare_a.Children.Add(Assignment_Statement());
            }
            declare_a.Children.Add(Declare_B());
            return declare_a;
        }
        Node Declare_B()
        {
            Node declare_b = new Node("declare_b");
            if(InputPointer<TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    declare_b.Children.Add(match(Token_Class.Comma));
                    declare_b.Children.Add(Declare_A());
                    return declare_b;
                }
            }            
            return null;
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
        Node if_Statement()
        {
            Node if_Statement_node = new Node("if_Statement");
            if (TokenStream[InputPointer].token_type == Token_Class.IF)
            {
                if_Statement_node.Children.Add(match(Token_Class.IF));
                if_Statement_node.Children.Add(match(Token_Class.LeftParentheses));
                if_Statement_node.Children.Add(Condition_Statement());
                if_Statement_node.Children.Add(match(Token_Class.RightParentheses));
                if_Statement_node.Children.Add(match(Token_Class.THEN));
                if_Statement_node.Children.Add(Statements());
                if_Statement_node.Children.Add(Else_if_statement());
                if_Statement_node.Children.Add(Else_Statement());
                if_Statement_node.Children.Add(match(Token_Class.END));
            }
            return if_Statement_node;
        }
        Node Else_if_statement()
        {
            Node else_if_statement = new Node("else_if_condition");
            if (TokenStream[InputPointer].token_type == Token_Class.ELSEIF)
            {
                else_if_statement.Children.Add(match(Token_Class.ELSEIF));
                else_if_statement.Children.Add(match(Token_Class.LeftParentheses));
                else_if_statement.Children.Add(Condition_Statement());
                else_if_statement.Children.Add(match(Token_Class.RightParentheses));
                else_if_statement.Children.Add(match(Token_Class.THEN));
                else_if_statement.Children.Add(Statements());
                else_if_statement.Children.Add(Else_if_statement());
                else_if_statement.Children.Add(Else_Statement());
                else_if_statement.Children.Add(match(Token_Class.END));
            }
                return else_if_statement;
        }
        Node Condition()
        {
            Node condition = new Node("condition");
            condition.Children.Add(match(Token_Class.Identifier));
            if(Is_Condition(InputPointer))
            {
                condition.Children.Add(match(TokenStream[InputPointer].token_type));
            }
            condition.Children.Add(Term());
            return condition;
        }
        Node Condition_Statement()
        {
            Node Condition_Statement_node = new Node("Condition_Statement");
            Condition_Statement_node.Children.Add(Condition());           
            Condition_Statement_node.Children.Add(Condition_State());
            return Condition_Statement_node;
        }

        Node Condition_State()
        {
            Node condition_state = new Node("condition_state");
            if(Is_Boolean(InputPointer))
            {
                condition_state.Children.Add(match(TokenStream[InputPointer].token_type));
                condition_state.Children.Add(Condition());
                condition_state.Children.Add(Condition_State());
                return condition_state;
            }
            return null;
        }
       
        Node Arithmatic_OP()
        {
            Node arith = new Node("arithmatic_op");
            if(TokenStream[InputPointer].token_type == Token_Class.Plus ||
                TokenStream[InputPointer].token_type == Token_Class.Minus ||
                TokenStream[InputPointer].token_type == Token_Class.Multiply ||
                TokenStream[InputPointer].token_type == Token_Class.Division)
            {
                arith.Children.Add(match(TokenStream[InputPointer].token_type));
            }
            return arith;
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
            if(InputPointer<TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
                {
                    arglist.Children.Add(match(Token_Class.Identifier));
                    arglist.Children.Add(Arguments());
                    return arglist;
                }
            }            
            return null;
        }
        Node Arguments()
        {
            Node arguments = new Node("arguments");
            if(InputPointer<TokenStream.Count)
            {
                if (Token_Class.Comma == TokenStream[InputPointer].token_type)
                {
                    arguments.Children.Add(match(Token_Class.Comma));
                    arguments.Children.Add(ArgList());
                    return arguments;
                }
            }                       
            return null;
        }
        Node Equation()
        {
            Node equation = new Node("equation");

            Node term_tmp = Term();
            int term_input_pointer = InputPointer;
            InputPointer -= term_tmp.Children.Count;

            if (term_tmp.Children[0].Name == Token_to_node(TokenStream[InputPointer].token_type).Name)
            {
                InputPointer = term_input_pointer;
                equation.Children.Add(term_tmp);
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.LeftParentheses)
            {
                equation.Children.Add(match(Token_Class.LeftParentheses));
                equation.Children.Add(Equation_A());
                equation.Children.Add(match(Token_Class.RightParentheses));
                equation.Children.Add(Equation_B());
            }
            return equation;
        }
        Node Equation_A()
        {
            Node eq_a = new Node("equation_a");
            eq_a.Children.Add(Term());
            eq_a.Children.Add(Equation_B());
            return eq_a;
        }
        Node Equation_B()
        {
            Node eq_b = new Node("equation_b");
            if(InputPointer < TokenStream.Count)
            {
                if (Arithmatic_OP() == Token_to_node(TokenStream[InputPointer].token_type))
                {
                    eq_b.Children.Add(Arithmatic_OP());
                    eq_b.Children.Add(Term());
                    eq_b.Children.Add(Equation_B());
                }
            }            
            return null;
        }
        Node Expression()
        {
            Node expression = new Node("expression");

            Node term_tmp = Term();
            int term_input_pointer = InputPointer;
            InputPointer -= term_tmp.Children.Count;

            Node equation_tmp = Equation();
            int equation_input_pointer = InputPointer;
            InputPointer -= equation_tmp.Children.Count;
           
            if (Token_Class.STRING == TokenStream[InputPointer].token_type)
            {
                expression.Children.Add(match(Token_Class.String));
            }           
            else if (term_tmp.Children[0].Name == Token_to_node(TokenStream[InputPointer].token_type).Name)
            {
                InputPointer = term_input_pointer;
                expression.Children.Add(term_tmp);
            }           
            else if (equation_tmp.Children[0].Name == Token_to_node(TokenStream[InputPointer].token_type).Name)
            {
                InputPointer = equation_input_pointer;
                expression.Children.Add(equation_tmp);
            }            
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
            Node write_state = new Node("write statement");
            write_state.Children.Add(match(Token_Class.WRITE));
            write_state.Children.Add(Expression());
            write_state.Children.Add(match(Token_Class.Semicolon));            
            return write_state;
        }
        Node Endline()
        {
            Node el = new Node("endline");
            if(InputPointer < TokenStream.Count)
            {
                if (Token_Class.ENDL == TokenStream[InputPointer].token_type)
                {
                    el.Children.Add(match(Token_Class.ENDL));
                    return el;
                }
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
        public Node Token_to_node(Token_Class tc)
        {
            Node converted_node = new Node(tc.ToString());
            return converted_node;
        }
        bool Is_Boolean(int InputPointer)
        {
            bool is_Or = TokenStream[InputPointer].token_type == Token_Class.Or;
            bool is_And = TokenStream[InputPointer].token_type == Token_Class.And;
            return is_And || is_Or;
        }
        bool Is_Condition(int InputPointer)
        {
            bool is_LessThan = TokenStream[InputPointer].token_type == Token_Class.LessThan;
            bool is_GreaterThan = TokenStream[InputPointer].token_type == Token_Class.GreaterThan;
            bool is_Equal = TokenStream[InputPointer].token_type == Token_Class.Equal;
            bool is_NotEqual = TokenStream[InputPointer].token_type == Token_Class.NotEqual;
            return is_Equal || is_GreaterThan || is_LessThan || is_NotEqual;
        }
        bool Is_Datatype(int InputPointer)
        {
            bool is_Int = TokenStream[InputPointer].token_type == Token_Class.INTEGER;
            bool is_Float = TokenStream[InputPointer].token_type == Token_Class.FLOAT;
            bool is_String = TokenStream[InputPointer].token_type == Token_Class.String;
            return is_Int || is_Float || is_String;
        }
        bool Is_Statement(int InputPointer)
        {
            bool is_decleration = Is_Datatype(InputPointer);
            bool is_write = TokenStream[InputPointer].token_type == Token_Class.WRITE;
            bool is_read = TokenStream[InputPointer].token_type == Token_Class.READ;
            bool is_return = TokenStream[InputPointer].token_type == Token_Class.RETURN;
            bool is_identifier = TokenStream[InputPointer].token_type == Token_Class.Identifier;
            bool is_if = TokenStream[InputPointer].token_type == Token_Class.IF;
            bool is_repeat = TokenStream[InputPointer].token_type == Token_Class.REPEAT;
            return is_decleration || is_write || is_read || is_return || is_identifier || is_if || is_repeat;
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
