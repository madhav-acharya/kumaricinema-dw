using KumariCinema.Models;
using KumariCinema.Repositories;
using KumariCinema.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace KumariCinema.Admin {
    public partial class halls : System.Web.UI.Page {
        private HallRepository _hallRepository;
        private TheaterRepository _theaterRepository;
        private AuthorizationService _authorizationService;

        protected void Page_Load(object sender, EventArgs e) {
            if (Session["CurrentUser"] == null) {
                Response.Redirect("~/components/Login.aspx");
                return;
            }

            _authorizationService = new AuthorizationService();
            var currentUser = (AppUser)Session["CurrentUser"];

            if (!_authorizationService.IsAdminLevel(currentUser) && !_authorizationService.IsStaff(currentUser)) {
                Response.Redirect("~/components/Login.aspx");
                return;
            }

            if (!IsPostBack) {
                LoadTheaters(currentUser);
                LoadHalls(currentUser);
            }

            string deleteId = Request.Form["deleteHallId"];
            if (!string.IsNullOrEmpty(deleteId)) {
                DeleteHall(deleteId, currentUser);
            }
        }

        private void LoadTheaters(AppUser currentUser) {
            _theaterRepository = new TheaterRepository();

            List<Theater> theaters = _authorizationService.IsSuperAdmin(currentUser)
                ? _theaterRepository.GetAll()
                : _theaterRepository.GetAll().Where(t => t.TheaterId == currentUser.TheaterId).ToList();

            theaterDropdown.DataSource = theaters;
            theaterDropdown.DataTextField = "Name";
            theaterDropdown.DataValueField = "TheaterId";
            theaterDropdown.DataBind();

            editTheaterDropdown.DataSource = theaters;
            editTheaterDropdown.DataTextField = "Name";
            editTheaterDropdown.DataValueField = "TheaterId";
            editTheaterDropdown.DataBind();
        }

        private void LoadHalls(AppUser currentUser) {
            try {
                _hallRepository = new HallRepository();
                List<Hall> halls = _authorizationService.IsSuperAdmin(currentUser)
                    ? _hallRepository.GetAll()
                    : _hallRepository.GetByTheaterId(currentUser.TheaterId);

                hallsRepeater.DataSource = halls;
                hallsRepeater.DataBind();
            } catch (Exception ex) {
                ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error loading halls: {ex.Message}', 'error');", true);
            }
        }

        protected void SaveHall_Click(object sender, EventArgs e) {
            try {
                if (!Page.IsValid)
                    return;

                var currentUser = (AppUser)Session["CurrentUser"];
                _hallRepository = new HallRepository();

                string theaterId = _authorizationService.IsSuperAdmin(currentUser)
                    ? theaterDropdown.SelectedValue
                    : currentUser.TheaterId;

                var hall = new Hall {
                    HallName = hallNameInput.Text.Trim(),
                    Capacity = int.TryParse(capacityInput.Text, out int capacity) ? capacity : 0,
                    ScreenType = screenTypeDropdown.SelectedValue,
                    TheaterId = theaterId
                };

                if (_hallRepository.Insert(hall)) {
                    ClearInputs();
                    LoadHalls(currentUser);
                    ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Hall added successfully', 'success');", true);
                } else {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Failed to add hall', 'error');", true);
                }
            } catch (Exception ex) {
                ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error: {ex.Message}', 'error');", true);
            }
        }

        protected void UpdateHall_Click(object sender, EventArgs e) {
            try {
                var currentUser = (AppUser)Session["CurrentUser"];
                _hallRepository = new HallRepository();

                string theaterId = _authorizationService.IsSuperAdmin(currentUser)
                    ? editTheaterDropdown.SelectedValue
                    : currentUser.TheaterId;

                if (!_authorizationService.IsSuperAdmin(currentUser) && theaterId != currentUser.TheaterId) {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('You cannot move hall to another theater', 'error');", true);
                    return;
                }

                var hall = new Hall {
                    HallId = editHallIdField.Value,
                    HallName = editHallNameInput.Text.Trim(),
                    Capacity = int.TryParse(editCapacityInput.Text, out int capacity) ? capacity : 0,
                    ScreenType = editScreenTypeDropdown.SelectedValue,
                    TheaterId = theaterId
                };

                if (_hallRepository.Update(hall)) {
                    LoadHalls(currentUser);
                    ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Hall updated successfully', 'success');", true);
                } else {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Failed to update hall', 'error');", true);
                }
            } catch (Exception ex) {
                ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error: {ex.Message}', 'error');", true);
            }
        }

        private void DeleteHall(string hallId, AppUser currentUser) {
            try {
                _hallRepository = new HallRepository();
                var hall = _hallRepository.GetById(hallId);

                if (hall == null) {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Hall not found', 'error');", true);
                    return;
                }

                if (!_authorizationService.IsSuperAdmin(currentUser) && hall.TheaterId != currentUser.TheaterId) {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Access denied for this theater', 'error');", true);
                    return;
                }

                if (_hallRepository.Delete(hallId)) {
                    LoadHalls(currentUser);
                    ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Hall deleted successfully', 'success');", true);
                } else {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Failed to delete hall', 'error');", true);
                }
            } catch (Exception ex) {
                ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error: {ex.Message}', 'error');", true);
            }
        }

        private void ClearInputs() {
            hallIdInput.Text = string.Empty;
            hallNameInput.Text = string.Empty;
            capacityInput.Text = string.Empty;
            screenTypeDropdown.SelectedIndex = 0;
        }
    }
}
