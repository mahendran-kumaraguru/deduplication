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
    public partial class Messages : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write(getMyFiles());
        }

        protected string getMyFiles()
        {
            string str = @"<div class='container'><div class='col-lg-1'></div><div class='row filetable col-lg-10 col-xs-12'><div class='box'><div class='col-lg-12'>";
            str += getfiletabledb();
            str += "</div></div></div></div><footer></footer>";
            return str;
        }
        protected string getfiletabledb()
        {
            string str = "";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("select * from tblVisitorMessages order by VisitorTime desc", con);
                con.Open();
                SqlDataReader sdr2 = cmd1.ExecuteReader();

                if (!sdr2.Read())
                {
                    str += "<h3>No Comments Available</h3>";
                }
                else
                    do
                    {
                        str += "<div class='col-md-6 col-lg-12'><h3>" + sdr2["VisitorName"].ToString().Replace("&sq;","'") + " <small>says</small></h3>";
                        str += "<p class='mailid'>" + sdr2["VisitorMail"].ToString().Replace("&sq;", "'") + "  |  " + sdr2["VisitorMobile"] + "  |  " + sdr2["VisitorTime"] + "</p><br/><br/>";
                        str += "<p>" + sdr2["VisitorMessage"].ToString().Replace("&sq;", "'") + "</p><br/><hr></div>";
                    } while (sdr2.Read());
                sdr2.Close();
            }
            return str;
        }
    }
}