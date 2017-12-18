using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Configuration;

namespace Deduplication.admin
{
    public partial class Resolve : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.QueryString.HasKeys())
            {
                Response.Redirect("AdminDefault.aspx");
            }
            string newname = Request.QueryString["new"];
            string oldname = Request.QueryString["old"];
            try
            {
                File.Move(newname, Server.MapPath("../files/") + "Enc_" + oldname);
            }
            catch
            {
                Response.Redirect("Audit.aspx?err=try");
            }
            Response.Redirect("Audit.aspx");

        }
    }
}