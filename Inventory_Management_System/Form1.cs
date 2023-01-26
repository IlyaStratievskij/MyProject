using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Inventory_Management_System
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection Connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\strat\OneDrive\Рабочий стол\Inventory_Management_System\Inventorydb.mdf;Integrated Security=True;Connect Timeout=30");

        private void label2_Click(object sender, EventArgs e){}
        private void panel1_Paint(object sender, PaintEventArgs e){}
        private void label4_Click(object sender, EventArgs e){}
        private void label5_Click(object sender, EventArgs e){}

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (UserNameTextBox.Text.Length > 0) label4.Visible = false;
            else label4.Visible = true;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (PasswordTextBox.Text.Length > 0) label5.Visible = false;
            else label5.Visible = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false) PasswordTextBox.UseSystemPasswordChar = true;
            else PasswordTextBox.UseSystemPasswordChar = false;
        }

        private void label3_Click(object sender, EventArgs e)
        {
            UserNameTextBox.Clear();
            PasswordTextBox.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Connect.Open();
            SqlDataAdapter sda = new SqlDataAdapter("Select Count(*) from UserTbl where Uname = '"+UserNameTextBox.Text+"' and Upassword = '"+PasswordTextBox.Text+"'", Connect);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows[0][0].ToString() == "1")
            {
                HomeForm home = new HomeForm();
                home.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Wrong UserName Or Password");
            }
            Connect.Close();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
