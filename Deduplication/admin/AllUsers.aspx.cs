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
    public partial class AllUsers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write(getMyFiles());
        }

        protected string getMyFiles()
        {
            string str = @"<div class='container'><div class='col-lg-1'></div><div class='row filetable col-lg-10'><div class='box'>
                            <table id='filetable' class='table table-responsive'>
                         
                        <tr>
                        <th class='sno' width='10%'>S.No</th>
                        <th class='fname' width='25%'>Name</th>
                        <th class='ftype' width='20%'>Username</th>
                        <th class='fsize' width='30%'>Mail</th>
                        <th class='nusers' width='15%'>Mobile</th>
                        </tr>";
            str+=getfiletabledb();
            str += "<tr></tr></table></div></div><div class='col-lg-1'></div></div><footer></footer>";
            return str;
        }
        protected string getfiletabledb()
        {
            string str = "";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("select * from tblUserDetails", con);
                con.Open();
                SqlDataReader sdr2 = cmd1.ExecuteReader();
                int j = 1;

                if (!sdr2.Read())
                {
                    str+="<tr><td colspan='4' style='text-align:center'>No Users Available</td></tr>";
                }
                else
                    while(sdr2.Read())
                    {
                        str+="<tr><td>" + (j++) + "</td><td>" + sdr2["Name"] + "</td><td>" + sdr2["Username"] + "</td><td>"
                         + sdr2["Mail"] + "</td><td>" + sdr2["Mobile"] + "</td></tr>";

                    }
                sdr2.Close();
            }
            return str;
        }

    }
}