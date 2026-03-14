using KumariCinema.Models;
using KumariCinema.Repositories;
using KumariCinema.Services;
using System;

namespace KumariCinema.Admin
{
    public partial class users : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["CurrentUser"] == null) Response.Redirect("~/components/Login.aspx");
                var auth = new AuthorizationService();
                AppUser user = (AppUser)Session["CurrentUser"];
                if (!auth.CanManageUsers(user)) Response.Redirect("~/components/Login.aspx");
                LoadUsers();
                SetActiveLink("usersLink");
            }
        }

        private void LoadUsers()
        {
            try
            {
                var repo = new AppUserRepository();
                repeater.DataSource = repo.GetAll();
                repeater.DataBind();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "e", $"showToast('{ex.Message}', 'error');", true);
            }
        }

        protected void SetActiveLink(string linkId)
        {
            ClientScript.RegisterStartupScript(GetType(), "setActive", $"setActiveLink('{linkId}');", true);
        }
    }
}
