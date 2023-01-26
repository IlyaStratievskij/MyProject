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
    public partial class ViewOrders : Form
    {
        public ViewOrders()
        {
            InitializeComponent();
        }
        SqlConnection Connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\strat\OneDrive\Рабочий стол\Inventory_Management_System\Inventorydb.mdf;Integrated Security=True;Connect Timeout=30");

        void populateorders()
        {
                Connect.Open();
                string Myquery = "Select * from OrderTbl";
                SqlDataAdapter da = new SqlDataAdapter(Myquery, Connect);
                SqlCommandBuilder builder = new SqlCommandBuilder(da);
                var ds = new DataSet();
                da.Fill(ds);
                OrdersGv.DataSource = ds.Tables[0];
                Connect.Close();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
        }


        private void ViewOrders_Load(object sender, EventArgs e)
        {
            populateorders();
        }

        private void OrdersGv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (printPreviewDialog1.ShowDialog() == DialogResult.OK)
                printDocument1.Print();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString("Order Summary", new Font("Century", 25, FontStyle.Bold), Brushes.LightPink, new Point(230) );
            e.Graphics.DrawString("Order Id:" + OrdersGv[0, OrdersGv.CurrentRow.Index].Value.ToString(), new Font("Century", 25, FontStyle.Bold), Brushes.Black, new Point(80, 100));
            e.Graphics.DrawString("Customer Id:" + OrdersGv[1, OrdersGv.CurrentRow.Index].Value.ToString(), new Font("Century", 25, FontStyle.Bold), Brushes.Black, new Point(80, 133));
            e.Graphics.DrawString("Customer Name:" + OrdersGv[2, OrdersGv.CurrentRow.Index].Value.ToString(), new Font("Century", 25, FontStyle.Bold), Brushes.Black, new Point(80, 166));
            e.Graphics.DrawString("Order Date:" + OrdersGv[3, OrdersGv.CurrentRow.Index].Value.ToString(), new Font("Century", 25, FontStyle.Bold), Brushes.Black, new Point(80, 199));
            e.Graphics.DrawString("Total Amount:" + OrdersGv[4, OrdersGv.CurrentRow.Index].Value.ToString(), new Font("Century", 25, FontStyle.Bold), Brushes.Black, new Point(80, 232));
            e.Graphics.DrawString("Powered by Stratievskij Ilya", new Font("Century", 25, FontStyle.Bold), Brushes.LightPink, new Point(230, 350));

        }
    }
}
