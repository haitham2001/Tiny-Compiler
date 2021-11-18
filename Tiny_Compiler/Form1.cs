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

        private void button1_Click(object sender, EventArgs e)
        {

        }
        void PrintTokens()
        {
            for (int i = 0; i < Tiny_Compiler.Tiny_Scanner.Tokens.Count; i++)
            {
                if(Tiny_Compiler.Tiny_Scanner.Tokens.ElementAt(i).token_type.ToString().Equals("INTEGER")|| Tiny_Compiler.Tiny_Scanner.Tokens.ElementAt(i).token_type.ToString().Equals("FLOAT"))
                    dataGridView1.Rows.Add(Tiny_Compiler.Tiny_Scanner.Tokens.ElementAt(i).lex, "Datatype ("+Tiny_Compiler.Tiny_Scanner.Tokens.ElementAt(i).token_type+")");
                else
                    dataGridView1.Rows.Add(Tiny_Compiler.Tiny_Scanner.Tokens.ElementAt(i).lex, Tiny_Compiler.Tiny_Scanner.Tokens.ElementAt(i).token_type);
            }
        }

        void PrintErrors()
        {
            for (int i = 0; i < Errors.Error_List.Count; i++)
            {
                //if(!(Errors.Error_List[i].ToString().Equals(" ")))
                //{
                    textBox2.Text += "Unrecognized Token";
                    textBox2.Text += "\t\t";
                //}
                
                textBox2.Text += Errors.Error_List[i];
                textBox2.Text += "\r\n";
            }
        }

        private void button_Click_1(object sender, EventArgs e)
        {
            string code = textBox1.Text; 
            Tiny_Compiler.Start_Compiling(code);
            PrintTokens();
            PrintErrors();
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            textBox2.Clear();
            //foreach (DataGridViewRow row in dataGridView1.Rows)
            //{
            //    foreach (DataGridViewCell cell in row.Cells)
            //    {
            //        cell.Value = "";
            //    }
            //}

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }


        
    }
}
