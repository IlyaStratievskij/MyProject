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
    public partial class ManageProducts : Form
    {
        public ManageProducts()
        {
            InitializeComponent();
        }
        SqlConnection Connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\strat\OneDrive\Рабочий стол\Inventory_Management_System\Inventorydb.mdf;Integrated Security=True;Connect Timeout=30");

        void fillCategory()
        {
            string query = "select * from CategorTbl";
            SqlCommand cmd = new SqlCommand(query, Connect);
            SqlDataReader rdr;
            try
            {
                Connect.Open();
                DataTable dt = new DataTable();
                dt.Columns.Add("CatName", typeof(string));
                rdr = cmd.ExecuteReader();
                dt.Load(rdr);
                CatComboBox.ValueMember = "CatName";
                CatComboBox.DataSource = dt;
                SearchComboBox.ValueMember = "CatName";
                SearchComboBox.DataSource = dt;
                Connect.Close();
            }
            catch
            {

            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ProductIdTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ProductIdTextBox.Text.Length > 0) label3.Visible = false;
            else label3.Visible = true;
        }

        private void ProductNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ProductNameTextBox.Text.Length > 0) label4.Visible = false;
            else label4.Visible = true;
        }

        private void ProductQtyTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ProductQtyTextBox.Text.Length > 0) label5.Visible = false;
            else label5.Visible = true;
        }

        private void ProductPriceTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ProductPriceTextBox.Text.Length > 0) label6.Visible = false;
            else label6.Visible = true;
        }

        private void DescriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (DescriptionTextBox.Text.Length > 0) label1.Visible = false;
            else label1.Visible = true;
        }

        void populate()
        {
            try
            {
                Connect.Open();
                string Myquery = "Select * from ProductTbl";
                SqlDataAdapter da = new SqlDataAdapter(Myquery, Connect);
                SqlCommandBuilder builder = new SqlCommandBuilder(da);
                var ds = new DataSet();
                da.Fill(ds);
                ProductGV.DataSource = ds.Tables[0];
                Connect.Close();
            }
            catch
            {

            }
        }

        void filterbycategory()
        {
            try
            {
                Connect.Open();
                string Myquery = "Select * from ProductTbl where ProductCat='" + SearchComboBox.SelectedValue.ToString()+"'";
                SqlDataAdapter da = new SqlDataAdapter(Myquery, Connect);
                SqlCommandBuilder builder = new SqlCommandBuilder(da);
                var ds = new DataSet();
                da.Fill(ds);
                ProductGV.DataSource = ds.Tables[0];
                Connect.Close();
            }
            catch
            {

            }
        }

        private void ManageProducts_Load(object sender, EventArgs e)
        {
            fillCategory();
            populate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Connect.Open();
                SqlCommand cmd = new SqlCommand("insert into ProductTbl values('" + ProductIdTextBox.Text + "'," +
                "'" + ProductNameTextBox.Text + "','" + ProductQtyTextBox.Text + "','" + ProductPriceTextBox.Text + "','" + DescriptionTextBox.Text + "','" + CatComboBox.SelectedValue.ToString() + "')", Connect);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Product Successfully Added");
                Connect.Close();
                populate();
                ProductIdTextBox.Clear();
                ProductNameTextBox.Clear();
                ProductQtyTextBox.Clear();
                ProductPriceTextBox.Clear();
                DescriptionTextBox.Clear();
            }
            catch
            {

            }
        }

        private void ProductGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                ProductIdTextBox.Text = ProductGV.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();
                ProductNameTextBox.Text = ProductGV.Rows[e.RowIndex].Cells[1].FormattedValue.ToString();
                ProductQtyTextBox.Text = ProductGV.Rows[e.RowIndex].Cells[2].FormattedValue.ToString();
                ProductPriceTextBox.Text = ProductGV.Rows[e.RowIndex].Cells[3].FormattedValue.ToString();
                DescriptionTextBox.Text = ProductGV.Rows[e.RowIndex].Cells[4].FormattedValue.ToString();
                CatComboBox.SelectedValue = ProductGV.Rows[e.RowIndex].Cells[5].FormattedValue.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show("This table cannot be selected");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (ProductIdTextBox.Text == "")
            {
                MessageBox.Show("Enter the Product Id");
            }
            else
            {
                Connect.Open();
                string myQuery = "delete from ProductTbl where ProductId='" + ProductIdTextBox.Text + "';";
                SqlCommand cmd = new SqlCommand(myQuery, Connect);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Product Successfully Deleted");
                Connect.Close();
                populate();
                ProductIdTextBox.Clear();
                ProductNameTextBox.Clear();
                ProductQtyTextBox.Clear();
                ProductPriceTextBox.Clear();
                DescriptionTextBox.Clear();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Connect.Open();
                SqlCommand cmd = new SqlCommand("update ProductTbl set ProductName='" + ProductNameTextBox.Text + "', " +
                    "ProductQty='" + ProductQtyTextBox.Text + "', ProductPrice='" + ProductPriceTextBox.Text + "', " +
                    "ProductDesc='" + DescriptionTextBox.Text + "', ProductCat='" + CatComboBox.SelectedValue.ToString() + "' " +
                    "where ProductId='" + ProductIdTextBox.Text + "'", Connect);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Product Successfully Updated");
                Connect.Close();
                populate();
                ProductIdTextBox.Clear();
                ProductNameTextBox.Clear();
                ProductQtyTextBox.Clear();
                ProductPriceTextBox.Clear();
                DescriptionTextBox.Clear();
            }
            catch
            {

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            filterbycategory();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            populate();
        }

        private void CatComboBox_SelectedIndexChanged(object sender, EventArgs e){}

        private void SearchComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            HomeForm home = new HomeForm();
            home.Show();
            this.Hide();
        }
    }
}
