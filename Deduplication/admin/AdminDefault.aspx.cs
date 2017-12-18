using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Deduplication.admin
{
    public partial class AdminDefault : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["welcome"] == "admin")
                Response.Redirect("AllFiles.aspx?welcome=admin");
            else
                Response.Redirect("AllFiles.aspx");

        }
    }
}