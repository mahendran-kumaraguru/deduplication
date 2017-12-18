using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using Simias.Encryption;

namespace Deduplication.user
{
    public partial class Upload : System.Web.UI.Page
    {
        static string filetag, dupname, dupuser;
        public static long m_originalLength = 0;
        public static string filename = "", path = "", myKey = "administrator";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["login"] == "success")
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("select Name from tblUserDetails where Username='" + Context.User.Identity.Name + "'", con);
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader(); sdr.Read();
                    Response.Write("<script>alert('Welcome " + sdr[0] + "')</script>");
                }
            }
        }
        protected void uploadButton_Click(object sender, EventArgs e)
        {
            errorLabel.Visible = false;
            if (FileUpload1.HasFile)
            {
                filename = FileUpload1.FileName;
                if (!isDuplicate(FileUpload1))
                {
                    uploadFile(FileUpload1);
                    addFileToFileTable(FileUpload1, filename);
                    getPOW(FileUpload1);
                    Response.Redirect("UserDefault.aspx");
                }
                else
                {
                    if (!File.Exists(Server.MapPath("../files/") +"Enc_"+ dupname))
                    {
                        uploadFile(FileUpload1);
                        File.Move(Server.MapPath("../files/") +"Enc_"+ filename, Server.MapPath("../files/") +"Enc_"+ dupname);
                    }
                    if (!isMyFile(FileUpload1))
                    {
                        dupuser = "another";
                        getPOW(FileUpload1);
                        Response.Redirect("UserDefault.aspx?dupuser=another&dupname=" + dupname + "");
                    }
                    else
                    {
                        dupuser = "you";
                        Response.Redirect("UserDefault.aspx?dupuser=you&dupname=" + dupname + "");
                    }
                }
            }
            else
            {
                errorLabel.Text = "Please select a file";
                errorLabel.ForeColor = System.Drawing.Color.Red;
                errorLabel.Visible = true;
            }
        }
        protected bool isMyFile(FileUpload FileUpload1)
        {
            string userid, userkey, filetoken;
            string cs = ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd2 = new SqlCommand("select * from tblUserDetails where Username='" + User.Identity.Name + "'", con);
                con.Open();
                SqlDataReader sdr2 = cmd2.ExecuteReader(); sdr2.Read();
                userid = sdr2["UserID"].ToString();
                userkey = sdr2["UserKey"].ToString(); sdr2.Close();
                filetoken = hashEncrypt(filetag, userkey);
                SqlCommand cmd3 = new SqlCommand("select * from tblHashTable where FileToken='" + filetoken + "'", con);
                SqlDataReader sdr3 = cmd3.ExecuteReader();
                while (sdr3.Read())
                    if (userid.Equals(sdr3["UserID"].ToString()))
                        return true;
            }
            return false;
        }
        protected void createFileTag(FileUpload FileUpload1)
        {
            try
            {
                StreamReader sr = new StreamReader(FileUpload1.PostedFile.InputStream);
                string content = sr.ReadToEnd();
                filetag = content.GetHashCode().ToString();
            }
            catch
            {
                Response.Write("<script type='text/javascript'>alert('File Read Failed ! Please upload again ! ')</script>");
            }
        }
        protected bool isDuplicate(FileUpload FileUpload1)
        {
            createFileTag(FileUpload1);
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("select * from tblFileDetails where FileKey='" + filetag + "'", con);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader(); sdr.Read();
                if (sdr.HasRows)
                {
                    dupname = sdr[1].ToString();
                    return true;
                }
                else
                    return false;
            }
        }
        protected void uploadFile(FileUpload FileUpload1)
        {
            int i = 1, ext = filename.IndexOf('.');
            if (File.Exists(Server.MapPath("../files/") + "Enc_" + filename))
                filename = filename.Insert(ext, " (" + (i++) + ")");
        filenamecheck:
            if (File.Exists(Server.MapPath("../files/") + "Enc_" + filename))
            {
                filename = filename.Replace("(" + (i - 1) + ")", "(" + (i++) + ")");
                goto filenamecheck;
            }
            FileUpload1.SaveAs(Server.MapPath("../files/") + filename);
            path = Server.MapPath("../files/"); encryptFile();
            
        }
        private void updateEncryptedHash()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("update tblFileDetails set EncFileKey='"+getEncHash()+"' where FileName='"+filename+"'", con);
                con.Open(); cmd.ExecuteNonQuery();
            }
        }
        private string getEncHash()
        {
            if (File.Exists(Server.MapPath("../files/") + "Enc_" + filename))
            {
                StreamReader sr = new StreamReader(Server.MapPath("../files/") + "Enc_" + filename);
                string hash = sr.ReadToEnd().GetHashCode().ToString(); return hash;
            }
            return "";
        }
        protected void addFileToFileTable(FileUpload Fileupload1, string filename)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("insert into tblFileDetails (FileName, FileType, FileLength, FileUsers, FileKey) values ('"+filename+"', '"+Fileupload1.PostedFile.ContentType+"', '"+Fileupload1.FileContent.Length+"', 0 , '"+filetag+"')", con);
                con.Open(); cmd.ExecuteNonQuery();
            }
            updateEncryptedHash();
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
            memorystream.Close(); cryptostream.Close();
            return Convert.ToBase64String(cipherbytes);
        }
        protected void getPOW(FileUpload FileUpload1)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("select UserKey from tblUserDetails where Username='" + User.Identity.Name + "'", con);
                con.Open();
                SqlDataReader sdr = cmd1.ExecuteReader(); sdr.Read();
                string userkey = sdr["UserKey"].ToString(); sdr.Close();
                SqlCommand cmd2 = new SqlCommand("spUpdateEncHash", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                SqlParameter paramUsername = new SqlParameter("@Username", User.Identity.Name);
                SqlParameter paramFileToken = new SqlParameter("@FileToken", hashEncrypt(filetag, userkey));
                cmd2.Parameters.Add(paramUsername); cmd2.Parameters.Add(paramFileToken);
                cmd2.ExecuteNonQuery();
                SqlCommand cmd4 = new SqlCommand("select * from tblFileDetails where FileKey='" + filetag + "'", con);
                SqlDataReader sdr4 = cmd4.ExecuteReader(); sdr4.Read();
                int fileusers = Convert.ToInt32(sdr4["FileUsers"]); sdr4.Close();
                SqlCommand cmd3 = new SqlCommand("update tblFileDetails set FileUsers='" + (fileusers + 1) + "' where FileKey='" + filetag + "'", con);
                cmd3.ExecuteNonQuery();
            }
        }
        private void encryptFile()
        {
            FileStream originalStream = File.OpenRead(path + filename); //Change to your file name  
            Blowfish alg = new Blowfish(Encoding.Unicode.GetBytes(myKey));
            m_originalLength = originalStream.Length;
            Byte[] buffer = new byte[originalStream.Length + (8 - (originalStream.Length % 8))];
            originalStream.Read(buffer, 0, buffer.Length);
            originalStream.Close();
            alg.Encipher(buffer, buffer.Length);
            FileStream stream = new FileStream(path + "Enc_" + filename, FileMode.Create);
            stream.Write(buffer, 0, buffer.Length);
            stream.Close();
            File.Delete(path + filename);
        }
    }
}
