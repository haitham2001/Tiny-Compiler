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
        bool return_is_found = false;
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
                return_is_found = true;
                statement.Children.Add(Return_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.REPEAT)
            {
                statement.Children.Add(Repeat_Statements());
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
                statement.Children.Add(Declaration_statements());
            }
          
            return statement;
        }
        Node Program()
        {
            Node program = new Node("Program");
            if (TokenStream[InputPointer + 1].token_type != Token_Class.main)
            {
                program.Children.Add(User_Function());
            }           
            if (InputPointer + 1 < TokenStream.Count)
            {
                if (TokenStream[InputPointer + 1].token_type == Token_Class.main)
                {
                    program.Children.Add(Main_Function());
                }               
            }
            else
            {
                Errors.Parser_Error_List.Add("you must write main function");
            }

            return program;
        }

        Node User_Function()
        {
            Node user_function = new Node("User_Function");
            if (InputPointer + 1 < TokenStream.Count)
            {
                while (TokenStream[InputPointer + 1].token_type == Token_Class.Identifier) {
                    user_function.Children.Add(Function_Statement());
                    if (InputPointer + 1 > TokenStream.Count)
                    {
                        break;
                    }
                }                
                return user_function;
            }
            return user_function;
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
            Main_Function_node.Children.Add(match(Token_Class.LeftParentheses));
            Main_Function_node.Children.Add(match(Token_Class.RightParentheses));
            Main_Function_node.Children.Add(Function_Body());          
            return Main_Function_node;
            
        }

        Node Function_Body()
        {
            Node Function_Body_node = new Node("Function_Body");
            Function_Body_node.Children.Add(match(Token_Class.LeftBraces));   
            
            Function_Body_node.Children.Add(States());

            if (!return_is_found)
                Errors.Parser_Error_List.Add("You must have at least one return statement" + "\r\n");
            return_is_found = false;

            Function_Body_node.Children.Add(match(Token_Class.RightBraces));          
            return Function_Body_node;
        }
        Node States()
        {
            Node States_node = new Node("States");
            States_node.Children.Add(Statements());
            if (Is_Statement(InputPointer))
            {
                States_node.Children.Add(States_Repetition());
            }
            return States_node;
        }
        Node States_Repetition()
        {
            Node states_repeat = new Node("state_repetition");
            if (InputPointer < TokenStream.Count && Is_Statement(InputPointer))
            {
                int temp = InputPointer;
                states_repeat.Children.Add(Statements());
                if (temp != InputPointer)
                {
                    states_repeat.Children.Add(States_Repetition());
                }

                return states_repeat;
            }          
            return states_repeat;       
        }
        Node Function_Declaration()
        {
            Node Function_Declaration_node = new Node("Function_Declaration");
            Function_Declaration_node.Children.Add(Datatype());
            Function_Declaration_node.Children.Add(match(Token_Class.Identifier));
            Function_Declaration_node.Children.Add(match(Token_Class.LeftParentheses));
            Function_Declaration_node.Children.Add(Parameter_List());
            Function_Declaration_node.Children.Add(match(Token_Class.RightParentheses));
            
            return Function_Declaration_node;
        }
        Node Parameter_List()
        {
            Node parameter_list = new Node("Parameters");            
            if (!(TokenStream[InputPointer].token_type == Token_Class.RightParentheses))
            {
                parameter_list.Children.Add(Parameter());
                parameter_list.Children.Add(Parameter_Repeatition());
                return parameter_list;
            }                     
            return null;
        }
        Node Parameter_Repeatition()
        {
            Node parameter_repeatition = new Node("Parameter Repetition");
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
            Node Function_Name_node = new Node("Function Name");
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
            if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                if(TokenStream[InputPointer + 1].token_type == Token_Class.Assign)
                {
                    declare.Children.Add(Assignment_Statement());
                }
                else
                {
                    declare.Children.Add(match(Token_Class.Identifier));
                }               
            }           
            declare.Children.Add(More_Declaration());
            if (TokenStream[InputPointer - 1].token_type != Token_Class.Semicolon)
            {
                declare.Children.Add(match(Token_Class.Semicolon));
            }
            return declare;
        }
        Node More_Declaration()
        {
            Node more_declare = new Node("more_declartions");           
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                more_declare.Children.Add(match(Token_Class.Comma));
                if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
                {
                    if (TokenStream[InputPointer + 1].token_type == Token_Class.Assign)
                    {
                        more_declare.Children.Add(Assignment_Statement());
                    }
                    else
                    {
                        more_declare.Children.Add(match(Token_Class.Identifier));
                    }
                }
                more_declare.Children.Add(More_Declaration());
                return more_declare;
            }
            return null;
        }        
        Node Repeat_Statements()
        {
            Node Repeat_Statements_node = new Node("Repeat_Statements");
            Repeat_Statements_node.Children.Add(match(Token_Class.REPEAT));
            Repeat_Statements_node.Children.Add(States());
            Repeat_Statements_node.Children.Add(match(Token_Class.UNTIL));
            Repeat_Statements_node.Children.Add(Condition_Statement());
            return Repeat_Statements_node;
        }
        Node Else_Statement()
        {
            Node Else_Statement_node = new Node("Else_Statement");
            Else_Statement_node.Children.Add(match(Token_Class.ELSE));
            Else_Statement_node.Children.Add(States());
            Else_Statement_node.Children.Add(match(Token_Class.END));
            return Else_Statement_node;
        }
        Node if_Statement()
        {
            Node if_Statement_node = new Node("if_Statement");           
            if_Statement_node.Children.Add(match(Token_Class.IF));            
            if_Statement_node.Children.Add(Condition_Statement());
            if_Statement_node.Children.Add(match(Token_Class.THEN));
            if_Statement_node.Children.Add(States());
            if(TokenStream[InputPointer].token_type == Token_Class.ELSEIF)
            {
                if_Statement_node.Children.Add(Else_if_statement());
            }
            else if(TokenStream[InputPointer].token_type == Token_Class.ELSE)
            {
                if_Statement_node.Children.Add(Else_Statement());
            }
            else if(TokenStream[InputPointer].token_type == Token_Class.END)
            {
                if_Statement_node.Children.Add(match(Token_Class.END));
            }                     
            return if_Statement_node;
        }
        Node Else_if_statement()
        {
            Node else_if_statement = new Node("else_if_condition");            
            else_if_statement.Children.Add(match(Token_Class.ELSEIF));            
            else_if_statement.Children.Add(Condition_Statement());           
            else_if_statement.Children.Add(match(Token_Class.THEN));
            else_if_statement.Children.Add(States());
            if (TokenStream[InputPointer].token_type == Token_Class.ELSEIF)
            {
                else_if_statement.Children.Add(Else_if_statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.ELSE)
            {
                else_if_statement.Children.Add(Else_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.END)
            {
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
            if(is_arithamtic(InputPointer))
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
                if(Token_Class.LeftParentheses == TokenStream[InputPointer + 1].token_type)                   
                {
                    term.Children.Add(Function_Call());
                }
                else
                {
                    term.Children.Add(match(Token_Class.Identifier));
                }                
            }            
            return term;
        }       
        Node Function_Call()
        {
            Node fn_call = new Node("function call");
            fn_call.Children.Add(Function_Name());
            fn_call.Children.Add(match(Token_Class.LeftParentheses));
            fn_call.Children.Add(ArgList());
            fn_call.Children.Add(match(Token_Class.RightParentheses));
            return fn_call;
        }
        
        Node ArgList()
        {
            Node arglist = new Node("arglist");              
            if(TokenStream[InputPointer].token_type != Token_Class.RightParentheses)
            {
                arglist.Children.Add(Expression());
                arglist.Children.Add(Arguments());
                return arglist;
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

            if (Is_Term(InputPointer))
            {                
                equation.Children.Add(Equation_A());
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
            if (is_arithamtic(InputPointer))
            {
                eq_b.Children.Add(Arithmatic_OP());
                eq_b.Children.Add(Term());
                eq_b.Children.Add(Equation_B());
                return eq_b;
            }
            return null;
        }
        Node Expression()
        {
            Node expression = new Node("expression");           

            if (Token_Class.String == TokenStream[InputPointer].token_type)
            {
                expression.Children.Add(match(Token_Class.String));
            }           
            else if (Is_Term(InputPointer))
            {
                int tmp = InputPointer + 1;
                if(is_arithamtic(tmp))
                {
                    expression.Children.Add(Equation());
                }
                else
                {                    
                    expression.Children.Add(Term());
                }              
            }  
            else if(TokenStream[InputPointer].token_type == Token_Class.LeftParentheses)
            {
                expression.Children.Add(Equation());
            }
            return expression;
        }
       Node Assignment_Statement()
        {
            Node assign = new Node("assignment statement");
            assign.Children.Add(match(Token_Class.Identifier));
            assign.Children.Add(match(Token_Class.Assign));
            assign.Children.Add(Expression());
            if (TokenStream[InputPointer].token_type != Token_Class.Comma)
            {
                assign.Children.Add(match(Token_Class.Semicolon));
            }
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
            if(Token_Class.ENDL == TokenStream[InputPointer].token_type)
            {
                write_state.Children.Add(match(Token_Class.ENDL));
            }
            else
            {
                write_state.Children.Add(Expression());
            }            
            write_state.Children.Add(match(Token_Class.Semicolon));            
            return write_state;
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
            bool is_String = TokenStream[InputPointer].token_type == Token_Class.STRING;
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
        bool Is_Term(int inputpointer)
        {
            bool is_number = TokenStream[InputPointer].token_type == Token_Class.Number;
            bool is_identifier = TokenStream[InputPointer].token_type == Token_Class.Identifier;
            return is_number || is_identifier;
        }
        bool is_arithamtic(int inputpointer)
        {
            bool is_plus = TokenStream[inputpointer].token_type == Token_Class.Plus;
            bool is_minus = TokenStream[inputpointer].token_type == Token_Class.Minus;
            bool is_multi = TokenStream[inputpointer].token_type == Token_Class.Multiply;
            bool is_divide = TokenStream[inputpointer].token_type == Token_Class.Division;
            return is_plus || is_multi || is_minus || is_divide;
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
                        + "\r\n\t      Found " + TokenStream[InputPointer].token_type.ToString() + 
                        " Token Number: "+ InputPointer + ' ' + TokenStream[InputPointer].lex + "\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Parser_Error_List.Add("Parsing Error: Expected " + ExpectedToken.ToString()  + 
                    " Token Number: " + InputPointer + ' ' + TokenStream[InputPointer].lex + "\r\n");
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