using KumariCinema.Models;
using KumariCinema.Repositories;
using KumariCinema.Services;
using System;
using System.Linq;

namespace KumariCinema.Admin
{
    public partial class users : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CurrentUser"] == null) Response.Redirect("~/pages/Login.aspx");

            var auth = new AuthorizationService();
            AppUser currentUser = (AppUser)Session["CurrentUser"];
            if (!auth.CanManageUsers(currentUser)) Response.Redirect("~/pages/Login.aspx");

            if (!IsPostBack)
            {
                LoadTheaters(currentUser);
                LoadUsers(currentUser);
                SetActiveLink("usersLink");
            }
            else if (!string.IsNullOrEmpty(Request.Form["deleteUserId"]))
            {
                DeleteUser(Request.Form["deleteUserId"], currentUser);
            }
        }

        private void LoadTheaters(AppUser currentUser)
        {
            var repo = new TheaterRepository();
            var data = currentUser.Role == AuthorizationService.SUPER_ADMIN ? repo.GetAll() : repo.GetAll().Where(t => t.TheaterId == currentUser.TheaterId).ToList();

            theaterDropdown.DataSource = data;
            theaterDropdown.DataTextField = "Name";
            theaterDropdown.DataValueField = "TheaterId";
            theaterDropdown.DataBind();

            editTheaterDropdown.DataSource = data;
            editTheaterDropdown.DataTextField = "Name";
            editTheaterDropdown.DataValueField = "TheaterId";
            editTheaterDropdown.DataBind();
        }

        private void LoadUsers(AppUser currentUser)
        {
            try
            {
                var repo = new AppUserRepository();
                var data = currentUser.Role == AuthorizationService.SUPER_ADMIN ? repo.GetAll() : repo.GetByTheaterId(currentUser.TheaterId);
                repeater.DataSource = data;
                repeater.DataBind();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "e", $"showToast('{EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        protected void SaveUser_Click(object sender, EventArgs e)
        {
            try
            {
                var currentUser = (AppUser)Session["CurrentUser"];
                var repo = new AppUserRepository();
                var theaterId = currentUser.Role == AuthorizationService.SUPER_ADMIN ? theaterDropdown.SelectedValue : currentUser.TheaterId;
                var user = new AppUser
                {
                    Name = nameInput.Text.Trim(),
                    Email = emailInput.Text.Trim(),
                    Password = passwordInput.Text.Trim(),
                    Role = roleDropdown.SelectedValue,
                    TheaterId = theaterId
                };

                if (repo.Insert(user))
                {
                    LoadUsers(currentUser);
                    ClientScript.RegisterStartupScript(GetType(), "s", "showToast('User added successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "e", "showToast('Failed to add user', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "e", $"showToast('{EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        protected void UpdateUser_Click(object sender, EventArgs e)
        {
            try
            {
                var currentUser = (AppUser)Session["CurrentUser"];
                var repo = new AppUserRepository();
                var existing = repo.GetById(editUserIdField.Value);
                if (existing == null)
                {
                    ClientScript.RegisterStartupScript(GetType(), "e", "showToast('User not found', 'error');", true);
                    return;
                }

                if (currentUser.Role != AuthorizationService.SUPER_ADMIN && existing.TheaterId != currentUser.TheaterId)
                {
                    ClientScript.RegisterStartupScript(GetType(), "e", "showToast('You do not have permission to update this user', 'error');", true);
                    return;
                }

                existing.Name = editNameInput.Text.Trim();
                existing.Email = editEmailInput.Text.Trim();
                existing.Password = editPasswordInput.Text.Trim();
                existing.Role = editRoleDropdown.SelectedValue;
                existing.TheaterId = currentUser.Role == AuthorizationService.SUPER_ADMIN ? editTheaterDropdown.SelectedValue : currentUser.TheaterId;

                if (repo.Update(existing))
                {
                    LoadUsers(currentUser);
                    ClientScript.RegisterStartupScript(GetType(), "s", "showToast('User updated successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "e", "showToast('Failed to update user', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "e", $"showToast('{EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        private void DeleteUser(string id, AppUser currentUser)
        {
            try
            {
                var repo = new AppUserRepository();
                var existing = repo.GetById(id);
                if (existing == null)
                {
                    ClientScript.RegisterStartupScript(GetType(), "e", "showToast('User not found', 'error');", true);
                    return;
                }

                if (currentUser.Role != AuthorizationService.SUPER_ADMIN && existing.TheaterId != currentUser.TheaterId)
                {
                    ClientScript.RegisterStartupScript(GetType(), "e", "showToast('You do not have permission to delete this user', 'error');", true);
                    return;
                }

                if (repo.Delete(id))
                {
                    LoadUsers(currentUser);
                    ClientScript.RegisterStartupScript(GetType(), "s", "showToast('User deleted successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "e", "showToast('Failed to delete user', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "e", $"showToast('{EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        protected void SetActiveLink(string linkId)
        {
            ClientScript.RegisterStartupScript(GetType(), "setActive", $"setActiveLink('{linkId}');", true);
        }

        private string EscapeJs(string s) => s?.Replace("'", "\\'").Replace("\r", "").Replace("\n", " ") ?? "";
    }
}
