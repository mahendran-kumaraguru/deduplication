using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Security;

namespace Deduplication
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["reg"] == "success")
                Response.Write("<script type='text/javascript'>alert('Registration Successfull')</script>");  
        }
        protected void loginButton_Click(object sender, EventArgs e)
        {
            string cs = ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("select * from tblUserDetails where Username='" + username.Text + "';", con);
            con.Open(); SqlDataReader sdr = cmd.ExecuteReader();
            if (sdr.Read())
            {
                string pass = FormsAuthentication.HashPasswordForStoringInConfigFile(password.Text,"SHA1");
                if (sdr["Password"].Equals(pass))
                {
                    FormsAuthentication.RedirectFromLoginPage(username.Text, true);
                    if (Request.QueryString.HasKeys())
                        Response.Redirect(Request.QueryString["ReturnUrl"]+"?login=success&welcome=admin");
                    else
                        Response.Redirect("user/UserDefault.aspx?login=success");
                }
                else
                    errorLabel.Text = "Wrong Password";
            }
            else
                errorLabel.Text = "Username not found";   
        }
    }
}