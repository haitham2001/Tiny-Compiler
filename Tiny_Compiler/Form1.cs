using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tiny_Compiler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void PrintTokens()
        {
            for (int i = 0; i < Tiny_Compiler.Tiny_Scanner.Tokens.Count; i++)
            {
                if(Tiny_Compiler.Tiny_Scanner.Tokens.ElementAt(i).token_type.ToString().Equals("INTEGER")|| Tiny_Compiler.Tiny_Scanner.Tokens.ElementAt(i).token_type.ToString().Equals("FLOAT")||
                    Tiny_Compiler.Tiny_Scanner.Tokens.ElementAt(i).token_type.ToString().Equals("STRING"))
                    dataGridView1.Rows.Add(Tiny_Compiler.Tiny_Scanner.Tokens.ElementAt(i).lex, "Datatype ("+Tiny_Compiler.Tiny_Scanner.Tokens.ElementAt(i).token_type+")");
                else
                    dataGridView1.Rows.Add(Tiny_Compiler.Tiny_Scanner.Tokens.ElementAt(i).lex, Tiny_Compiler.Tiny_Scanner.Tokens.ElementAt(i).token_type);
            }
        }

        void PrintErrors()
        {
            for (int i = 0; i < Errors.Error_List.Count; i++)
            {
                textBox2.Text += "Unrecognized Token";
                textBox2.Text += "\t\t";
                textBox2.Text += Errors.Error_List[i];
                textBox2.Text += "\r\n";
            }

            for (int i = 0; i < Errors.Parser_Error_List.Count; i++)
            {                
                textBox3.Text += Errors.Parser_Error_List[i];               
            }
        }

        private void button_Click_1(object sender, EventArgs e)
        {
            string code = textBox1.Text;

            Tiny_Compiler.TokenStream.Clear();
            treeView1.Nodes.Clear();
            Errors.Error_List.Clear();
            Errors.Parser_Error_List.Clear();
            Tiny_Compiler.Start_Compiling(code);

            PrintTokens();
            treeView1.Nodes.Add(Parser.PrintParseTree(Tiny_Compiler.tree_Root));
            PrintErrors();
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            Errors.Error_List.Clear();
            Errors.Parser_Error_List.Clear();
           // Tiny_Compiler.TokenStream.Clear();
            Tiny_Compiler.tokenStream.Clear();
            dataGridView1.Rows.Clear();
            treeView1.Nodes.Clear();
            textBox2.Clear();
            textBox3.Clear();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
