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
    public partial class ManageOrders : Form
    {
        int flag = 0;
        int stock;

        int num = 0;
        int uprice, totalprice, qty;
        string product;

        public ManageOrders()
        {
            InitializeComponent();
        }
        SqlConnection Connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\strat\OneDrive\Рабочий стол\Inventory_Management_System\Inventorydb.mdf;Integrated Security=True;Connect Timeout=30");

        void populate()
        {
            try
            {
                Connect.Open();
                string Myquery = "Select * from CustomerTbl";
                SqlDataAdapter da = new SqlDataAdapter(Myquery, Connect);
                SqlCommandBuilder builder = new SqlCommandBuilder(da);
                var ds = new DataSet();
                da.Fill(ds);
                CustomerGV.DataSource = ds.Tables[0];
                Connect.Close();
            }
            catch
            {

            }
        }

        void populateproduct()
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
                SearchComboBox.ValueMember = "CatName";
                SearchComboBox.DataSource = dt;
                Connect.Close();
            }
            catch
            {

            }
        }

        void updateProduct()
        {
            try
            {
                int id = Convert.ToInt16(ProductGV[0, ProductGV.CurrentRow.Index].Value.ToString());
      
                    
                if (ProductQtyTextBox.Text == "")
                    MessageBox.Show("Enter Stock Products");
                else if (stock - Convert.ToInt32(ProductQtyTextBox.Text) < 0)
                    MessageBox.Show("Operation Failed");
                else
                {
                    int newQty = stock - Convert.ToInt32(ProductQtyTextBox.Text);
                    Connect.Open();
                    string query = "update ProductTbl set ProductQty = " + newQty + " where ProductId = " + id + ";";
                    SqlCommand cmd = new SqlCommand(query, Connect);
                    cmd.ExecuteNonQuery();
                    Connect.Close();
                    populateproduct();
                }
            }
            catch
            {
                MessageBox.Show("Error Again!!!");
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void ManageOrders_Load(object sender, EventArgs e)
        {
            populate();
            populateproduct();
            fillCategory();
            OrderGv.Columns.Add("Num", "Num");
            OrderGv.Columns.Add("Product", "Product");
            OrderGv.Columns.Add("Quantity", "Quantity");
            OrderGv.Columns.Add("UPrice", "UPrice");
            OrderGv.Columns.Add("TotalPrice", "TotalPrice");
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e){}

        private void CustomerGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                CustomerIdTextBox.Text = CustomerGV.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();
                CustomerNametextBox.Text = CustomerGV.Rows[e.RowIndex].Cells[1].FormattedValue.ToString();

            }
            catch
            {

            }
        }

        private void OrderIdTextBox_TextChanged(object sender, EventArgs e)
        {
            if (OrderIdTextBox.Text.Length > 0) label3.Visible = false;
            else label3.Visible = true;
        }

        private void ProductGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            product = ProductGV.Rows[e.RowIndex].Cells[1].FormattedValue.ToString();
            stock = Convert.ToInt32(ProductGV.Rows[e.RowIndex].Cells[2].Value.ToString());
            // qty = Convert.ToInt32(ProductQtyTextBox.Text);
            uprice = Convert.ToInt32(ProductGV.Rows[e.RowIndex].Cells[3].Value.ToString());
            //totalprice = qty * uprice;
            flag = 1;
        }

        int sum = 0;
        private void button1_Click(object sender, EventArgs e)
        {

            if (ProductQtyTextBox.Text == "")
                MessageBox.Show("Enter The Quantity of Products");
            else if (flag == 0)
                MessageBox.Show("Select The Product");
            else if (Convert.ToInt32(ProductQtyTextBox.Text) > stock)
                MessageBox.Show("No Enough Stock Available");
            else
            {
                num = num + 1;
                qty = Convert.ToInt32(ProductQtyTextBox.Text);
                totalprice = qty * uprice;

                OrderGv.Rows.Add(num, product, qty, uprice, totalprice);
                flag = 0;

                sum = sum + totalprice;
                TotalAmount.Text = sum.ToString();
                updateProduct();
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (OrderIdTextBox.Text == "" || CustomerIdTextBox.Text == "" || CustomerNametextBox.Text == "" || TotalAmount.Text == "Rs")
            {
                MessageBox.Show("Fill The data Correctly");
            }
            else
            {
                    Connect.Open();
                    SqlCommand cmd = new SqlCommand("insert into OrderTbl values('"+OrderIdTextBox.Text+"', '"+CustomerIdTextBox.Text+"', '"+CustomerNametextBox.Text+"', '"+OrderDateTimePicker.Value.ToString("yyyy-MM-dd")+"', '"+TotalAmount.Text+"')",Connect);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Order Added Successfully");
                    Connect.Close();
                    //populate();

            }
        }

        private void CustomerNametextBox_TextChanged(object sender, EventArgs e)
        {
            if (CustomerNametextBox.Text.Length > 0) label5.Visible = false;
            else label5.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ViewOrders view = new ViewOrders();
            view.Show();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            HomeForm home = new HomeForm();
            home.Show();
            this.Hide();
        }

        private void CustomerIdTextBox_TextChanged(object sender, EventArgs e)
        {
            if (CustomerIdTextBox.Text.Length > 0) label1.Visible = false;
            else label1.Visible = true;
        }

        private void SearchComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Connect.Open();
                string Myquery = "Select * from ProductTbl where ProductCat='" + SearchComboBox.SelectedValue.ToString() + "'";
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
    }
}
