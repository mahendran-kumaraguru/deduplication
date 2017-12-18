using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using Simias.Encryption;
using System.Net;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Deduplication.user
{
    public partial class DownloadFile : System.Web.UI.Page
    {
        public static string myKey = "administrator", path="", filename="";
        public static long m_originalLength = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.QueryString.HasKeys())
                Response.Redirect("UserDefault.aspx");
            filename = Request.QueryString["fname"];
            if (File.Exists(Server.MapPath("../files/") + "Enc_" + filename))
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DedupDB"].ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("select FileLength from tblFileDetails where FileName='"+filename+"'", con);
                    con.Open();
                    SqlDataReader rdr= cmd.ExecuteReader();
                    rdr.Read();
                    m_originalLength = long.Parse(rdr["FileLength"].ToString());
                }
                path = Server.MapPath("../files/");
                decryptfile();
            }
            else
            {
                if (User.Identity.Name == "admin")
                    Response.Redirect("../admin/AllFiles.aspx?err=nofile");
                Response.Redirect("UserDefault.aspx?err=nofile");
            }
        }
        private void decryptfile()
        {
            FileStream originalStream = File.OpenRead(path + "Enc_" + filename);
            Blowfish alg = new Blowfish(Encoding.Unicode.GetBytes(myKey));
            Byte[] buffer = new byte[originalStream.Length];
            originalStream.Read(buffer, 0, buffer.Length);
            originalStream.Close();
            alg.Decipher(buffer, buffer.Length);
            FileStream stream = new FileStream(path + "Dec_" + filename, FileMode.Create);
            stream.Write(buffer, 0, (int)m_originalLength); //Dangerous casting - Write in chunks.  
            stream.Close();
            WebClient wclient = new WebClient();
            HttpResponse response = HttpContext.Current.Response;
            response.Clear();
            response.ClearContent();
            response.ClearHeaders();
            response.Buffer = true;
            response.AddHeader("Content-Disposition", "attachment;filename=\"" + filename + "\"");
            byte[] data = wclient.DownloadData(path + "Dec_" + filename);
            File.Delete(path + "Dec_" + filename);
            response.BinaryWrite(data);
            response.End();   
        }
    }
}