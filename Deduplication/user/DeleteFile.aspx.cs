using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;


namespace Deduplication.user
{
    public partial class DeleteFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.QueryString.HasKeys())
                Response.Redirect("UserDefault.aspx");
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("select * from tblUserDetails where Username='" + User.Identity.Name + "'", con);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader(); sdr.Read();
                string uid = sdr["UserID"].ToString();
                string userkey = sdr["UserKey"].ToString(); sdr.Close();
                string fid = Request.QueryString["fid"];
                SqlCommand cmd6 = new SqlCommand("select * from tblFileDetails where FileID='" + fid + "'", con);
                SqlDataReader sdr6 = cmd6.ExecuteReader(); sdr6.Read();
                string filetag = sdr6["FileKey"].ToString();
                int fileusers = Convert.ToInt32(sdr6["FileUsers"]);
                string filename = sdr6["FileName"].ToString(); sdr6.Close();
                string filetoken = hashEncrypt(filetag, userkey);
                SqlCommand cmd2 = new SqlCommand("delete from tblHashTable where UserID='" + uid + "' and FileToken='" + filetoken + "'", con);
                cmd2.ExecuteNonQuery();
                if (fileusers > 1)
                {
                    SqlCommand cmd7 = new SqlCommand("update tblFileDetails set FileUsers='" + (fileusers - 1) + "' where FileID='" + fid + "'", con);
                    cmd7.ExecuteNonQuery();
                }
                else
                {
                    SqlCommand cmd8 = new SqlCommand("delete from tblFileDetails where FileID='" + fid + "'", con);
                    cmd8.ExecuteNonQuery();
                    File.Delete(Server.MapPath("../files/") +"Enc_" + filename);
                    
                }
                Response.Redirect("UserDefault.aspx");
            }
        }
        protected string hashEncrypt(string filetag, string userkey)
        {
            RijndaelManaged cipher = new RijndaelManaged();
            string password = userkey;
            byte[] plaintext = Encoding.Unicode.GetBytes(filetag);
            byte[] salt = Encoding.ASCII.GetBytes(password.Length.ToString());
            PasswordDeriveBytes secretkey = new PasswordDeriveBytes(password, salt);
            ICryptoTransform encryptor = cipher.CreateEncryptor(secretkey.GetBytes(32), secretkey.GetBytes(16));
            MemoryStream memorystream = new MemoryStream();
            CryptoStream cryptostream = new CryptoStream(memorystream, encryptor, CryptoStreamMode.Write);
            cryptostream.Write(plaintext, 0, plaintext.Length);
            cryptostream.FlushFinalBlock();
            byte[] cipherbytes = memorystream.ToArray();
            memorystream.Close();
            cryptostream.Close();
            return Convert.ToBase64String(cipherbytes);
        }
    }
}