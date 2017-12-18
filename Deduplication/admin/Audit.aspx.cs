using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;

namespace Deduplication.admin
{
    public partial class Audit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["err"] == "try")
                Response.Write("<script>alert('File is used by another program! Please try again later !!')</script>");
            try
            {
                Response.Write(getMyFiles());
            }
            catch
            {
                Response.Redirect("Audit.aspx");
            }
        }
        protected string getMyFiles()
        {
            string str = @"<div class='container'><div class='col-lg-1'></div><div class='row filetable col-lg-10'><div class='box'>
                            <table id='filetable' class='table table-responsive'><tr>
                        <th class='sno' width='10%'>S.No</th>
                        <th class='fname' width='30%'>File Name</th>
                        <th class='ftype' width='40%'>Type</th>
                        <th class='fstatus' width='20%'>Status</th></tr>";
            str+=getfiletabledb();
            str += "<tr></tr></table></div></div><div class='col-lg-1'></div></div><footer></footer>";
            return str;
        }
        protected string getfiletabledb()
        {
            string str = "";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("select * from tblFileDetails", con);
                con.Open(); SqlDataReader sdr2 = cmd1.ExecuteReader();
                int j = 1;
                if (!sdr2.Read())
                    str+="<tr><td colspan='4' style='text-align:center'>No Files Available</td></tr>";
                else
                    do
                    {
                        str+="<tr><td>" + (j++) + "</td><td>" + sdr2["FileName"] + "</td><td>" + sdr2["FileType"] + "</td><td>" + getStatus(sdr2["FileName"].ToString()) + "</td></tr>";
                    }
                    while (sdr2.Read());
                sdr2.Close();
            }
            return str;
        }
        protected string getStatus(string filename)
        {
            if (referHash(filename))
                return "<div class='safe'>File Safe</div>";
            else if (File.Exists(Server.MapPath("../files/") +"Enc_"+ filename))
                return "<div class='mod'>File Modified</div>";
            else
            {
                string ren = isRenamed(filename);
                if (ren != "no")
                    return "<div class='ren'>File Renamed (<a href='Resolve.aspx?old=" + filename + "&new=" + ren + "'>Resolve</a>)</div>";
                else
                    return "<div class='del'>File Deleted</div>";
            }
        }
        protected bool referHash(string filename)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("select * from tblFileDetails where FileName='" + filename + "'", con);
                con.Open();
                SqlDataReader sdr2 = cmd1.ExecuteReader(); sdr2.Read();
                if (File.Exists(Server.MapPath("../files/") + "Enc_" + filename))
                {
                    StreamReader sr = new StreamReader(Server.MapPath("../files/") + "Enc_" + filename);
                    string hash = sr.ReadToEnd().GetHashCode().ToString();
                    if (hash.Equals(sdr2["EncFileKey"]))
                        return true;
                }
                sdr2.Close();
            }
            return false;
        }
        protected string isRenamed(string filename)
        {
            string fsize, fhash;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("select * from tblFileDetails where FileName='" + filename + "'", con);
                con.Open();
                SqlDataReader sdr2 = cmd1.ExecuteReader();
                sdr2.Read();
                fsize = sdr2["FileLength"].ToString();
                fhash = sdr2["EncFileKey"].ToString();
                sdr2.Close();
            }
            string[] files = Directory.GetFiles(Server.MapPath("../files/"));
            foreach (string file in files)
            {
                StreamReader sr = new StreamReader(file);
                string content = sr.ReadToEnd();
                string filetag = content.GetHashCode().ToString();
                if (filetag.Equals(fhash))
                    return file;
            }
            return "no";
        }
    }
}