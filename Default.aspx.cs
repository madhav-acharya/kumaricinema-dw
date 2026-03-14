using System;
using System.Web.UI;

namespace KumariCinema
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CurrentUser"] != null)
            {
                Response.Redirect("~/pages/dashboard.aspx", true);
                return;
            }

            Response.Redirect("~/components/Login.aspx", true);
        }
    }
}