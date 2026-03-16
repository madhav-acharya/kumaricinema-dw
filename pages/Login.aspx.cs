using KumariCinema.Services;
using System;

namespace KumariCinema
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CurrentUser"] != null)
            {
                Response.Redirect("~/pages/dashboard.aspx");
            }
        }

        protected void Login_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                    return;

                AuthService authService = new AuthService();
                var user = authService.Login(emailInput.Text, passwordInput.Text);

                if (user != null)
                {
                    Session["CurrentUser"] = user;
                    Response.Redirect("~/pages/dashboard.aspx");
                }
                else
                {
                    errorLabel.Text = "Invalid email or password";
                    ClientScript.RegisterStartupScript(GetType(), "show",
                        "document.getElementById('errorMessage').classList.add('show');", true);
                }
            }
            catch (Exception ex)
            {
                errorLabel.Text = "Login error: " + ex.Message;
                ClientScript.RegisterStartupScript(GetType(), "show",
                    "document.getElementById('errorMessage').classList.add('show');", true);
            }
        }
    }
}
