using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebScrapingHTMLTablesApp
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            txtUrl.Text = "https://www.moneycontrol.com/financials/tataelxsi/ratiosVI/TE";
        }

        private void btnGetTables_Click(object sender, EventArgs e)
        {
            DataSet ds = GetAllTables(txtUrl.Text);

            // Bind data to DataGridView.DataSource  
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = ds.Tables[1];
        }

        private static DataSet GetAllTables(string strRequestUrl)
        {
            DataSet ds = new DataSet();
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                using (var client = new WebClient())
                {
                    string html = client.DownloadString(strRequestUrl);
                    doc.LoadHtml(html);
                    foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table"))
                    {
                        try
                        {
                            DataTable dt = new DataTable();
                            int Count = 0;//One time for adding columns
                            foreach (HtmlNode row in table.SelectNodes("tr"))
                            {
                                int tdCount = row.SelectNodes("td").Count;
                                if (Count.Equals(0))
                                {
                                    for (int i = 0; i < tdCount; i++)
                                    {
                                        dt.Columns.Add("Column" + i, typeof(string));
                                    }
                                }
                                Count++;
                                DataRow dr = dt.NewRow();
                                int tdAddedCount = 0;
                                foreach (var cell in row.SelectNodes("td"))
                                {
                                    if (!(tdAddedCount > tdCount))
                                    {
                                        dr["Column" + tdAddedCount] = cell.InnerText;
                                    }
                                    tdAddedCount++;
                                }
                                dt.Rows.Add(dr);
                            }
                            ds.Tables.Add(dt);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ds;
        }
    }
}
