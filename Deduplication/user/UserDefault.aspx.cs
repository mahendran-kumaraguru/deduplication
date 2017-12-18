using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Deduplication.user
{
    public partial class UserDefault : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["login"] == "success")
            {
                using(SqlConnection con=new SqlConnection(ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("select Name from tblUserDetails where Username='"+Context.User.Identity.Name+"'", con);
                    con.Open(); SqlDataReader sdr = cmd.ExecuteReader(); sdr.Read();
                    Response.Write("<script>alert('Welcome "+sdr[0]+"')</script>");
                }
            }
            if (Request.QueryString["dupuser"] == "you")
                Response.Write("<script type='text/javascript'>alert('You have already uloaded this file: " + Request.QueryString["dupname"] + "')</script>");
            else if (Request.QueryString["dupuser"] == "another")
                Response.Write("<script type='text/javascript'>alert('Another user have already uloaded this file: " + Request.QueryString["dupname"] + "')</script>");
            else if (Request.QueryString["err"] == "nofile")
                Response.Write("<script type='text/javascript'>alert('Sorry ! File Not Found !! Please UPLOAD again or DELETE the file entry !')</script>");   
            Response.Write(addFileTable());
        }
        private string addFileTable()
        {
            string str = @"<div class='container'><div class='col-lg-1'></div><div class='row filetable col-lg-10'><div class='box'>
                           <table id='filetable' class='table table-responsive'><tr>
                           <th class='sno' width='10%'>S.No</th>
                           <th class='fname' width='25%'>File Name</th>
                           <th class='ftype' width='35%'>Type</th>
                           <th class='fsize' width='10%'>Size (KB)</th>
                           <th class='download' width='10%'>Download</th>
                           <th class='delete' width='10%'>Delete</th></tr>";
            str += getfiletabledb();
            str += "<tr></tr></table></div></div><div class='col-lg-1'></div></div><footer></footer>";
            return str;
        }
        protected string getfiletabledb()
        {
            string tbl = "",  cs = ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                int[] fileid = new int[100];
                string[] filetoken = new string[100]; string userkey;
                int filecount = 0, i = 0;
                SqlCommand cmd = new SqlCommand("spGetFiles", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter paramUsername = new SqlParameter("@Username", User.Identity.Name);
                cmd.Parameters.Add(paramUsername); con.Open();
                SqlDataReader sdr1 = cmd.ExecuteReader();
                while (sdr1.Read())
                    filetoken[i++] = sdr1["FileToken"].ToString();
                filecount = i; sdr1.Close();
                SqlCommand cmd2 = new SqlCommand("select UserKey from tblUserDetails where Username='" + User.Identity.Name + "'", con);
                SqlDataReader sdr3 = cmd2.ExecuteReader(); sdr3.Read();
                userkey = sdr3["UserKey"].ToString(); sdr3.Close();
                if (filecount == 0)
                    tbl += "<tr><td colspan='6' style='text-align:center'>No Files Available</td></tr>";
                for (int j = 0; j < filecount; j++)
                {
                    string filekey = DecryptFunc(filetoken[j], userkey);
                    SqlCommand cmd1 = new SqlCommand("select * from tblFileDetails where FileKey='" + filekey + "'", con);
                    SqlDataReader sdr2 = cmd1.ExecuteReader();
                    while (sdr2.Read())
                    {
                        double len = Convert.ToDouble(sdr2["FileLength"]);
                        len = len / 1024;
                        string lent="";
                        if (len.ToString().Contains('.'))
                            lent = len.ToString().Remove(len.ToString().IndexOf('.') + 4);
                        else
                            lent = len.ToString() + ".000";
                        tbl += @"<tr><td>" + (j + 1) + "</td><td>" + sdr2["FileName"] + "</td><td>" + sdr2["FileType"] + "</td><td>"
                            + lent
                            + "</td><td><a href='DownloadFile.aspx?fname=" + sdr2["FileName"] + "' >Download</a></td><td><a href='DeleteFile.aspx?fid=" + sdr2["FileID"] + "'>Delete</a></td></tr>";
                    }
                    sdr2.Close();
                }
            }
            return tbl;
        }
        protected string DecryptFunc(string ciphertext, string key)
        {
            RijndaelManaged cipher = new RijndaelManaged();
            string password = key, result;
            try
            {
                byte[] encryptedData = Convert.FromBase64String(ciphertext);
                byte[] salt = Encoding.ASCII.GetBytes(password.Length.ToString());
                PasswordDeriveBytes secretkey = new PasswordDeriveBytes(password, salt);
                ICryptoTransform decryptor = cipher.CreateDecryptor(secretkey.GetBytes(32), secretkey.GetBytes(16));
                MemoryStream memorystream = new MemoryStream(encryptedData);
                CryptoStream cryptostream = new CryptoStream(memorystream, decryptor, CryptoStreamMode.Read);
                byte[] plaintext = new byte[encryptedData.Length];
                int decryptedCount = cryptostream.Read(plaintext, 0, plaintext.Length);
                memorystream.Close(); cryptostream.Close();
                result = Encoding.Unicode.GetString(plaintext, 0, decryptedCount);
            }
            catch
            {
                result = ciphertext;
            }
            return result;
        }
    }
}