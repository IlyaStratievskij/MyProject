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
    public partial class ManageCategories : Form
    {
        public ManageCategories()
        {
            InitializeComponent();
        }
        SqlConnection Connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\strat\OneDrive\Рабочий стол\Inventory_Management_System\Inventorydb.mdf;Integrated Security=True;Connect Timeout=30");

        private void CategorieIdTextBox_TextChanged(object sender, EventArgs e)
        {
            if (CategorieIdTextBox.Text.Length > 0) label3.Visible = false;
            else label3.Visible = true;
        }

        private void CategorieNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (CategorieNameTextBox.Text.Length > 0) label4.Visible = false;
            else label4.Visible = true;
        }

        void populate()
        {
            try
            {
                Connect.Open();
                string Myquery = "Select * from CategorTbl";
                SqlDataAdapter da = new SqlDataAdapter(Myquery, Connect);
                SqlCommandBuilder builder = new SqlCommandBuilder(da);
                var ds = new DataSet();
                da.Fill(ds);
                CategoriesGV.DataSource = ds.Tables[0];
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
                SqlCommand cmd = new SqlCommand("insert into CategorTbl values('" + CategorieIdTextBox.Text + "'," +
                "'" + CategorieNameTextBox.Text + "')", Connect);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Category Successfully Added");
                Connect.Close();
                populate();
                CategorieIdTextBox.Clear();
                CategorieNameTextBox.Clear();
            }
            catch
            {

            }
        }

        private void ManageCategories_Load(object sender, EventArgs e)
        {
            populate();
        }

        private void CategoriesGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                CategorieIdTextBox.Text = CategoriesGV.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();
                CategorieNameTextBox.Text = CategoriesGV.Rows[e.RowIndex].Cells[1].FormattedValue.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("This table cannot be selected");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Connect.Open();
                SqlCommand cmd = new SqlCommand("update CategorTbl set CatName='" + CategorieNameTextBox.Text + "' where CatId='" + CategorieIdTextBox.Text + "'", Connect);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Category Successfully Updated");
                Connect.Close();
                populate();
                CategorieIdTextBox.Clear();
                CategorieNameTextBox.Clear();
            }
            catch
            {

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (CategorieIdTextBox.Text == "")
            {
                MessageBox.Show("Enter the Customer Id");
            }
            else
            {
                Connect.Open();
                string myQuery = "delete from CategorTbl where CatId='" + CategorieIdTextBox.Text + "';";
                SqlCommand cmd = new SqlCommand(myQuery, Connect);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Category Successfully Deleted");
                Connect.Close();
                populate();
                CategorieIdTextBox.Clear();
                CategorieNameTextBox.Clear();
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            HomeForm home = new HomeForm();
            home.Show();
            this.Hide();
        }
    }
}
