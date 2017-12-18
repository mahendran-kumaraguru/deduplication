using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Net.Mail;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Deduplication
{
    public partial class About : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void contactButton_Click(object sender, EventArgs e)
        {
            if (validateForm())
                updateCommentDB();
        }

        private void updateCommentDB()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("insert into tblVisitorMessages (VisitorName, VisitorMail, VisitorMobile, VisitorMessage, VisitorTime) values ('" + Vname.Text.Replace("'", "&sq;") + "', '" + Vmail.Text.Replace("'", "&sq;") + "', '" + Vmobile.Text + "', '" + Vmessage.Text.Replace("'", "&sq;")+ "', GETDATE())", con);
                con.Open();
                cmd.ExecuteNonQuery();
                Response.Write("<script>alert('Message Sent Successfully !')</script>");
                clearFields();
            }
            
        }

        private void clearFields()
        {
            Vname.Text = "";
            Vmail.Text = "";
            Vmobile.Text = "";
            Vmessage.Text = "";
            erroLabel.Text = "";
        }

        private bool validateForm()
        {
            if(Vname.Text=="")
            {
                erroLabel.Text = "Please Enter Your Name !";
                return false;
            }
            if (Vmail.Text == "")
            {
                erroLabel.Text = "Please Enter EMail Address !";
                return false;
            }
            if (!Vmail.Text.Contains('@'))
            {
                erroLabel.Text = "Please Enter Valid Email Address !";
                return false;
            }
            if (Vmobile.Text == "")
            {
                erroLabel.Text = "Please Enter Mobile Number !";
                return false;
            }
            if (Vmobile.Text.Length != 10)
            {
                erroLabel.Text = "Please Enter Valid Mobile Number !";
                return false;
            }
            if (Vmessage.Text == "")
            {
                erroLabel.Text = "Please Enter Your Message !";
                return false;
            }
            return true;
        }

        

    }
}