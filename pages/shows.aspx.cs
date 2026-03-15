using KumariCinema.Models;
using KumariCinema.Repositories;
using KumariCinema.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KumariCinema.Admin
{
    public partial class shows : System.Web.UI.Page
    {
        private MovieShowRepository _showRepository;
        private HallRepository _hallRepository;
        private MovieRepository _movieRepository;
        private AuthorizationService _authorizationService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CurrentUser"] == null)
            {
                Response.Redirect("~/components/Login.aspx");
                return;
            }

            _authorizationService = new AuthorizationService();
            var currentUser = (AppUser)Session["CurrentUser"];

            if (!_authorizationService.IsAdminLevel(currentUser) && !_authorizationService.IsStaff(currentUser))
            {
                Response.Redirect("~/components/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadDropdowns(currentUser);
                LoadShows(currentUser);
            }

            string deleteId = Request.Form["deleteShowId"];
            if (!string.IsNullOrEmpty(deleteId))
            {
                DeleteShow(deleteId, currentUser);
            }
        }

        private void LoadDropdowns(AppUser currentUser)
        {
            _movieRepository = new MovieRepository();
            _hallRepository = new HallRepository();

            var movies = _movieRepository.GetAll();
            var halls = _authorizationService.IsSuperAdmin(currentUser)
                ? _hallRepository.GetAll()
                : _hallRepository.GetByTheaterId(currentUser.TheaterId);

            movieDropdown.DataSource = movies;
            movieDropdown.DataTextField = "Name";
            movieDropdown.DataValueField = "MovieId";
            movieDropdown.DataBind();

            editMovieDropdown.DataSource = movies;
            editMovieDropdown.DataTextField = "Name";
            editMovieDropdown.DataValueField = "MovieId";
            editMovieDropdown.DataBind();

            hallDropdown.DataSource = halls;
            hallDropdown.DataTextField = "HallName";
            hallDropdown.DataValueField = "HallId";
            hallDropdown.DataBind();

            editHallDropdown.DataSource = halls;
            editHallDropdown.DataTextField = "HallName";
            editHallDropdown.DataValueField = "HallId";
            editHallDropdown.DataBind();
        }

        private void LoadShows(AppUser currentUser)
        {
            _showRepository = new MovieShowRepository();
            _hallRepository = new HallRepository();

            var allShows = _showRepository.GetAll();

            if (_authorizationService.IsSuperAdmin(currentUser))
            {
                showsRepeater.DataSource = allShows;
                showsRepeater.DataBind();
                return;
            }

            var hallIds = new HashSet<string>(_hallRepository.GetByTheaterId(currentUser.TheaterId).Select(h => h.HallId));
            var filtered = allShows.Where(s => hallIds.Contains(s.HallId)).ToList();
            showsRepeater.DataSource = filtered;
            showsRepeater.DataBind();
        }

        protected void SaveShow_Click(object sender, EventArgs e)
        {
            try
            {
                var currentUser = (AppUser)Session["CurrentUser"];
                _showRepository = new MovieShowRepository();
                _hallRepository = new HallRepository();

                if (!CanAccessHall(currentUser, hallDropdown.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Access denied for selected hall', 'error');", true);
                    return;
                }

                var show = new MovieShow
                {
                    MovieId = movieDropdown.SelectedValue,
                    HallId = hallDropdown.SelectedValue,
                    StartTime = DateTime.Parse(startTimeInput.Text),
                    EndTime = DateTime.Parse(endTimeInput.Text),
                    ShowCategory = categoryDropdown.SelectedValue,
                    BaseTicketPrice = decimal.TryParse(priceInput.Text, out decimal p) ? p : 0
                };

                if (_showRepository.Insert(show))
                {
                    LoadShows(currentUser);
                    ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Show added successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Failed to add show', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error: {ex.Message}', 'error');", true);
            }
        }

        protected void UpdateShow_Click(object sender, EventArgs e)
        {
            try
            {
                var currentUser = (AppUser)Session["CurrentUser"];
                _showRepository = new MovieShowRepository();

                if (!CanAccessHall(currentUser, editHallDropdown.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Access denied for selected hall', 'error');", true);
                    return;
                }

                var show = new MovieShow
                {
                    ShowId = editShowIdField.Value,
                    MovieId = editMovieDropdown.SelectedValue,
                    HallId = editHallDropdown.SelectedValue,
                    StartTime = DateTime.Parse(editStartTimeInput.Text),
                    EndTime = DateTime.Parse(editEndTimeInput.Text),
                    ShowCategory = editCategoryDropdown.SelectedValue,
                    BaseTicketPrice = decimal.TryParse(editPriceInput.Text, out decimal p) ? p : 0
                };

                if (_showRepository.Update(show))
                {
                    LoadShows(currentUser);
                    ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Show updated successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Failed to update show', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error: {ex.Message}', 'error');", true);
            }
        }

        private void DeleteShow(string showId, AppUser currentUser)
        {
            try
            {
                _showRepository = new MovieShowRepository();
                var show = _showRepository.GetById(showId);
                if (show == null)
                {
                    return;
                }

                if (!CanAccessHall(currentUser, show.HallId))
                {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Access denied for selected show', 'error');", true);
                    return;
                }

                if (_showRepository.Delete(showId))
                {
                    LoadShows(currentUser);
                    ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Show deleted successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Failed to delete show', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error: {ex.Message}', 'error');", true);
            }
        }

        private bool CanAccessHall(AppUser currentUser, string hallId)
        {
            if (_authorizationService.IsSuperAdmin(currentUser))
            {
                return true;
            }

            _hallRepository = new HallRepository();
            var hall = _hallRepository.GetById(hallId);
            return hall != null && hall.TheaterId == currentUser.TheaterId;
        }
    }
}
