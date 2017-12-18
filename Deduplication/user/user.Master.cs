using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Deduplication.user
{
    public partial class user : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Context.User.Identity.Name == "")
            {
                Response.Redirect("../Login.aspx");
            }
            if (Context.User.Identity.Name == "admin")
            {
                Response.Redirect("../admin/AdminDefault.aspx?welcome=admin");
            }
        }
    }
}