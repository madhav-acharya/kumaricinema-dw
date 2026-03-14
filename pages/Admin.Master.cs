using KumariCinema.Models;
using System;
using System.Web;

namespace KumariCinema.Admin
{
    public partial class Admin : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["CurrentUser"] == null)
                {
                    HttpContext.Current.Response.Redirect("~/components/Login.aspx");
                }
                else
                {
                    AppUser user = (AppUser)Session["CurrentUser"];
                    userNameLabel.Text = user.Name;
                }
            }
        }
    }
}
