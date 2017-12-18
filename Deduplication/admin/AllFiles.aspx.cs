using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Deduplication.admin
{
    public partial class AllFiles : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["welcome"] == "admin")
                Response.Write("<script>alert('Welcome Administrator / Auditor')</script>");

            if (Request.QueryString["err"] == "nofile")
            {
                Response.Write("<script type='text/javascript'>alert('Sorry ! File Not Found !!')</script>");
            }
            Response.Output.WriteLine(getMyFiles());
        }
        protected string getMyFiles()
        {
            string str;
            str = @"<div class='container'><div class='col-lg-1'></div><div class='row filetable col-lg-10'><div class='box'>
                            <table id='filetable' class='table table-responsive'>
                        <tr>
                        <th class='sno' width='10%'>S.No</th>
                        <th class='fname' width='25%'>File Name</th>
                        <th class='ftype' width='35%'>Type</th>
                        <th class='fsize' width='10%'>Size (KB)</th>
                        <th class='nusers' width='10%'>No. of Users</th>
                        <th class='download' width='10%'>Download</th>
                        </tr>";
            str += getfiletabledb();
            str += "<tr></tr></table></div></div><div class='col-lg-1'></div></div><footer></footer>";
            return str;
        }
        protected string getfiletabledb()
        {
            string tbl = "";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("select * from tblFileDetails", con);
                con.Open();
                SqlDataReader sdr2 = cmd1.ExecuteReader();
                int j = 1;
                if (!sdr2.Read())
                {
                    tbl += "<tr><td colspan='6' style='text-align:center'>No Files Available</td></tr>";
                }
                else
                    do
                    {
                        double len = Convert.ToDouble(sdr2["FileLength"]);
                        len = len / 1024;
                        string lent = "";
                        if (len.ToString().Contains('.'))
                            lent = len.ToString().Remove(len.ToString().IndexOf('.') + 4);
                        else
                            lent = len.ToString() + ".000";
                        tbl += "<tr><td>" + (j++) + "</td><td>" + sdr2["FileName"] + "</td><td>" + sdr2["FileType"] + "</td><td>"
                            + lent
                            + "</td><td>" + sdr2["FileUsers"] + "</td><td><a href='..\\user\\DownloadFile.aspx?fname=" + sdr2["FileName"] + "' >Download</a></td></tr>";

                    }
                    while (sdr2.Read());
                sdr2.Close();
            }
            return tbl;
        }

    }
}