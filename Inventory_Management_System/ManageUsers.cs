using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Inventory_Management_System
{
    public partial class ManageUsers : Form
    {
        public ManageUsers()
        {
            InitializeComponent();
        }
        SqlConnection Connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\strat\OneDrive\Рабочий стол\Inventory_Management_System\Inventorydb.mdf;Integrated Security=True;Connect Timeout=30");

        private void UserNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (UserNameTextBox.Text.Length > 0) label3.Visible = false;
            else label3.Visible = true;
        }

        private void FullNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (FullNameTextBox.Text.Length > 0) label4.Visible = false;
            else label4.Visible = true;
        }

        private void PasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            if (PasswordTextBox.Text.Length > 0) label5.Visible = false;
            else label5.Visible = true;
        }

        private void TelephoneTextBox_TextChanged(object sender, EventArgs e)
        {
            if (TelephoneTextBox.Text.Length > 0) label6.Visible = false;
            else label6.Visible = true;
        }

        private void label7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        void populate ()
        {
            try
            {
                Connect.Open();
                string Myquery = "Select * from UserTbl";
                SqlDataAdapter da = new SqlDataAdapter(Myquery, Connect);
                SqlCommandBuilder builder = new SqlCommandBuilder(da);
                var ds = new DataSet();
                da.Fill(ds);
                UserGV.DataSource = ds.Tables[0];
                Connect.Close();
            }
            catch
            {

            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Connect.Open();
                SqlCommand cmd = new SqlCommand("insert into UserTbl values('" + UserNameTextBox.Text + "'," +
                "'" + FullNameTextBox.Text + "','" + PasswordTextBox.Text + "','" + TelephoneTextBox.Text + "')", Connect);
                cmd.ExecuteNonQuery();
                MessageBox.Show("User Successfully Added");
                Connect.Close();
                populate();
                UserNameTextBox.Clear();
                FullNameTextBox.Clear();
                PasswordTextBox.Clear();
                TelephoneTextBox.Clear();
            } 
            catch
            {

            }
        }

        private void UserGV_CellContentClick(object sender, DataGridViewCellEventArgs e) 
        {
            try
            {
                UserNameTextBox.Text = UserGV.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();
                FullNameTextBox.Text = UserGV.Rows[e.RowIndex].Cells[1].FormattedValue.ToString();
                PasswordTextBox.Text = UserGV.Rows[e.RowIndex].Cells[2].FormattedValue.ToString();
                TelephoneTextBox.Text = UserGV.Rows[e.RowIndex].Cells[3].FormattedValue.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("This table cannot be selected");
            }
        }

        private void ManageUsers_Load(object sender, EventArgs e)
        {
            populate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (TelephoneTextBox.Text == "")
            {
                MessageBox.Show("Enter the Users Phone Number");
            }
            else
            {
                Connect.Open();
                string myQuery = "delete from UserTbl where UPhone='" + TelephoneTextBox.Text + "';";
                SqlCommand cmd = new SqlCommand(myQuery, Connect);
                cmd.ExecuteNonQuery();
                MessageBox.Show("User Successfully Deleted");
                Connect.Close();
                populate();
                UserNameTextBox.Clear();
                FullNameTextBox.Clear();
                PasswordTextBox.Clear();
                TelephoneTextBox.Clear();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Connect.Open();
                SqlCommand cmd = new SqlCommand("update UserTbl set Uname='"+UserNameTextBox.Text+"', Ufullname='"+FullNameTextBox.Text+"', Upassword='"+PasswordTextBox.Text+"' where UPhone='"+TelephoneTextBox.Text+"'", Connect);
                cmd.ExecuteNonQuery();
                MessageBox.Show("User Successfully Updated");
                Connect.Close();
                populate();
                UserNameTextBox.Clear();
                FullNameTextBox.Clear();
                PasswordTextBox.Clear();
                TelephoneTextBox.Clear();
            }
            catch
            {

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            HomeForm home = new HomeForm();
            home.Show();
            this.Hide();
        }
    }
}
