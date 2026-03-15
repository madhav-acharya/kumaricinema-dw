using KumariCinema.Models;
using KumariCinema.Repositories;
using KumariCinema.Services;
using System;
using System.Collections.Generic;

namespace KumariCinema.Admin
{
    public partial class theaters : System.Web.UI.Page
    {
        private TheaterRepository _theaterRepository;
        private AuthorizationService _authorizationService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CheckAuthorization();
                LoadTheaters();
                SetActiveLink("theatersLink");
            }
            else
            {
                if (Request.Form["deleteTheaterId"] != null)
                {
                    DeleteTheater(Request.Form["deleteTheaterId"]);
                }
            }
        }

        private void CheckAuthorization()
        {
            if (Session["CurrentUser"] == null)
            {
                Response.Redirect("~/components/Login.aspx");
            }

            _authorizationService = new AuthorizationService();
            AppUser currentUser = (AppUser)Session["CurrentUser"];

            if (!_authorizationService.CanManageTheaters(currentUser))
            {
                Response.Redirect("~/components/Login.aspx");
            }
        }

        private void LoadTheaters()
        {
            try
            {
                _theaterRepository = new TheaterRepository();
                List<Theater> theaters = _theaterRepository.GetAll();
                theatersRepeater.DataSource = theaters;
                theatersRepeater.DataBind();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "error", $"showToast('Error loading theaters: {ex.Message}', 'error');", true);
            }
        }

        protected void SaveTheater_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                    return;

                _theaterRepository = new TheaterRepository();

                Theater theater = new Theater
                {
                    Name = theaterNameInput.Text,
                    Location = locationInput.Text
                };

                if (_theaterRepository.Insert(theater))
                {
                    ClearInputs();
                    LoadTheaters();
                    ClientScript.RegisterStartupScript(this.GetType(), "success", "showToast('Theater added successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "error", "showToast('Failed to add theater', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "error", $"showToast('Error: {ex.Message}', 'error');", true);
            }
        }

        protected void UpdateTheater_Click(object sender, EventArgs e)
        {
            try
            {
                _theaterRepository = new TheaterRepository();

                Theater theater = new Theater
                {
                    TheaterId = editTheaterIdField.Value,
                    Name = editTheaterNameInput.Text,
                    Location = editLocationInput.Text
                };

                if (_theaterRepository.Update(theater))
                {
                    LoadTheaters();
                    ClientScript.RegisterStartupScript(this.GetType(), "success", "showToast('Theater updated successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "error", "showToast('Failed to update theater', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "error", $"showToast('Error: {ex.Message}', 'error');", true);
            }
        }

        private void DeleteTheater(string theaterId)
        {
            try
            {
                _theaterRepository = new TheaterRepository();

                if (_theaterRepository.Delete(theaterId))
                {
                    LoadTheaters();
                    ClientScript.RegisterStartupScript(this.GetType(), "success", "showToast('Theater deleted successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "error", "showToast('Failed to delete theater', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "error", $"showToast('Error: {ex.Message}', 'error');", true);
            }
        }

        private void ClearInputs()
        {
            theaterIdInput.Text = "";
            theaterNameInput.Text = "";
            locationInput.Text = "";
        }

        protected void SetActiveLink(string linkId)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "setActive", $"setActiveLink('{linkId}');", true);
        }
    }
}
