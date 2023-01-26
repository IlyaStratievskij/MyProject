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
    public partial class ManageCustomers : Form
    {
        public ManageCustomers()
        {
            InitializeComponent();
        }
        SqlConnection Connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\strat\OneDrive\Рабочий стол\Inventory_Management_System\Inventorydb.mdf;Integrated Security=True;Connect Timeout=30");


        private void label7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        

        private void CustomerIdTextBox_TextChanged(object sender, EventArgs e)
        {
            if (CustomerIdTextBox.Text.Length > 0) label3.Visible = false;
            else label3.Visible = true;
        }

        private void CustomerNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (CustomerNameTextBox.Text.Length > 0) label4.Visible = false;
            else label4.Visible = true;
        }

        private void CastomerTextBox_TextChanged(object sender, EventArgs e)
        {
            if (CustomerPhoneTextBox.Text.Length > 0) label5.Visible = false;
            else label5.Visible = true;
        }

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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Connect.Open();
                SqlCommand cmd = new SqlCommand("insert into CustomerTbl values('" + CustomerIdTextBox.Text + "'," +
                "'" + CustomerNameTextBox.Text + "','" + CustomerPhoneTextBox.Text + "')", Connect);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Customer Successfully Added");
                Connect.Close();
                populate();
                CustomerIdTextBox.Clear();
                CustomerNameTextBox.Clear();
                CustomerPhoneTextBox.Clear();
            }
            catch
            {

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (CustomerIdTextBox.Text == "")
            {
                MessageBox.Show("Enter the Customer Id");
            }
            else
            {
                Connect.Open();
                string myQuery = "delete from CustomerTbl where CustId='" + CustomerIdTextBox.Text + "';";
                SqlCommand cmd = new SqlCommand(myQuery, Connect);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Customer Successfully Deleted");
                Connect.Close();
                populate();
                CustomerIdTextBox.Clear();
                CustomerNameTextBox.Clear();
                CustomerPhoneTextBox.Clear();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Connect.Open();
                SqlCommand cmd = new SqlCommand("update CustomerTbl set CustName='" + CustomerNameTextBox.Text + "', CustPhone='" + CustomerPhoneTextBox.Text + "' where CustId='" + CustomerIdTextBox.Text + "'", Connect);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Customer Successfully Updated");
                Connect.Close();
                populate();
                CustomerIdTextBox.Clear();
                CustomerNameTextBox.Clear();
                CustomerPhoneTextBox.Clear();
            }
            catch
            {

            }
        }

        private void CustomerGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                CustomerIdTextBox.Text = CustomerGV.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();
                CustomerNameTextBox.Text = CustomerGV.Rows[e.RowIndex].Cells[1].FormattedValue.ToString();
                CustomerPhoneTextBox.Text = CustomerGV.Rows[e.RowIndex].Cells[2].FormattedValue.ToString();
                Connect.Open();
                SqlDataAdapter sda1 = new SqlDataAdapter("Select Count(*) from orderTbl where CustId = "+CustomerIdTextBox.Text+"", Connect);
                DataTable dt1 = new DataTable();
                sda1.Fill(dt1);
                OrderLabel.Text = dt1.Rows[0][0].ToString();

                SqlDataAdapter sda2 = new SqlDataAdapter("Select Sum(TotalAmount) from orderTbl where CustId = " + CustomerIdTextBox.Text + "", Connect);
                DataTable dt2 = new DataTable();
                sda2.Fill(dt2);
                AmountLabel.Text = dt2.Rows[0][0].ToString();

                SqlDataAdapter sda3 = new SqlDataAdapter("Select Max(OrderDate) from orderTbl where CustId = " + CustomerIdTextBox.Text + "", Connect);
                DataTable dt3 = new DataTable();
                sda3.Fill(dt3);
                DateLabel.Text = dt3.Rows[0][0].ToString();
                Connect.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("This table cannot be selected");
            }
        }

        private void ManageCustomers_Load(object sender, EventArgs e)
        {
            populate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            HomeForm home = new HomeForm();
            home.Show();
            this.Hide();
        }
    }
}
