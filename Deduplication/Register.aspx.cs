using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Web.Security;
using System.Net;

namespace Deduplication
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void registerButton_Click(object sender, EventArgs e)
        {
            if (validate())
                RegUser();
        }
        protected bool validate()
        {
            if (fullname.Text == "")
            {
                errorLabel.Text = "Enter Name !"; return false;
            }
            if (username.Text == "")
            {
                errorLabel.Text = "Enter Username !"; return false;
            }
            if (username.Text.Length < 5)
            {
                errorLabel.Text = "Username should contain min 5 characters"; return false;
            }
            if (username.Text.Contains(" "))
            {
                errorLabel.Text = "Username should not contain blank space"; return false;
            }
            if (password.Text == "")
            {
                errorLabel.Text = "Enter Password !"; return false;
            }
            if (password.Text.Length < 5)
            {
                errorLabel.Text = "Password should contain min 5 characters"; return false;
            }
            if (!password.Text.Equals(password2.Text))
            {
                errorLabel.Text = "Password Does Not Match !"; return false;
            }
            if (mail.Text == "")
            {
                errorLabel.Text = "Enter Mail ID !"; return false;
            }
            if (mobile.Text == "")
            {
                errorLabel.Text = "Enter Mobile Number !"; return false;
            }
            if (mobile.Text.Length != 10)
            {
                errorLabel.Text = "Invalid Mobile Number !"; return false;
            }
            return true;
        }
        protected void RegUser()
        {
            String CS = ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("spRegistration", con);
                cmd.CommandType = CommandType.StoredProcedure;
                string pass = FormsAuthentication.HashPasswordForStoringInConfigFile(password.Text, "SHA1");
                SqlParameter paramName = new SqlParameter("@Name", fullname.Text);
                SqlParameter paramUsername = new SqlParameter("@Username", username.Text);
                SqlParameter paramPassword = new SqlParameter("@Password", pass);
                SqlParameter paramMail = new SqlParameter("@Mail", mail.Text);
                SqlParameter paramMobile = new SqlParameter("@Mobile", mobile.Text);
                SqlParameter paramUserKey = new SqlParameter("@UserKey", hashEncrypt(username.Text, pass));
                cmd.Parameters.Add(paramName); cmd.Parameters.Add(paramUsername); cmd.Parameters.Add(paramPassword);
                cmd.Parameters.Add(paramMail); cmd.Parameters.Add(paramMobile); cmd.Parameters.Add(paramUserKey);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (Convert.ToBoolean(rdr["IsRegistered"]))
                    {
                        string message = "Hi "+fullname.Text+", Your Username: "+username.Text+" , Password: "+password.Text+"";
                        WebClient cli = new WebClient();
                       // Stream s = cli.OpenRead("http://49.50.69.90/api/smsapi.aspx?username=ilifetech&password=abc321&from=TXTMSG&to=" + mobile.Text + "&message=" + message);  
                        Response.Redirect("Login.aspx?reg=success");
                    }
                    else
                        errorLabel.Text = "Username already Exist !";
                }
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
            memorystream.Close(); cryptostream.Close();
            return Convert.ToBase64String(cipherbytes);
        }
    }
}