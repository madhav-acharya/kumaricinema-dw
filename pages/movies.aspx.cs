using KumariCinema.Models;
using KumariCinema.Repositories;
using KumariCinema.Services;
using System;
using System.Collections.Generic;

namespace KumariCinema.Admin
{
    public partial class movies : System.Web.UI.Page
    {
        private MovieRepository _movieRepository;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CurrentUser"] == null) { Response.Redirect("~/pages/Login.aspx"); return; }
            var currentUser = (AppUser)Session["CurrentUser"];
            if (!new AuthorizationService().IsAdminLevel(currentUser)) { Response.Redirect("~/pages/Login.aspx"); return; }

            if (!IsPostBack)
            {
                LoadMovies();
                SetActiveLink("moviesLink");
            }
            else if (Request.Form["deleteMovieId"] != null)
            {
                DeleteMovie(Request.Form["deleteMovieId"]);
            }
        }

        private void LoadMovies()
        {
            try
            {
                _movieRepository = new MovieRepository();
                List<Movie> movies = _movieRepository.GetAll();
                moviesRepeater.DataSource = movies;
                moviesRepeater.DataBind();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "error", $"showToast('Error loading movies: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        protected void SaveMovie_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                    return;

                _movieRepository = new MovieRepository();

                Movie movie = new Movie
                {
                    Name = movieNameInput.Text,
                    DurationMinutes = int.Parse(durationInput.Text),
                    ViewingFormat = formatDropdown.SelectedValue
                };

                if (_movieRepository.Insert(movie))
                {
                    MovieIdInput_Clear();
                    LoadMovies();
                    ClientScript.RegisterStartupScript(this.GetType(), "success", "showToast('Movie added successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "error", "showToast('Failed to add movie', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "error", $"showToast('Error: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        protected void UpdateMovie_Click(object sender, EventArgs e)
        {
            try
            {
                _movieRepository = new MovieRepository();

                Movie movie = new Movie
                {
                    MovieId = editMovieIdField.Value,
                    Name = editMovieNameInput.Text,
                    DurationMinutes = int.Parse(editDurationInput.Text),
                    ViewingFormat = editFormatDropdown.SelectedValue
                };

                if (_movieRepository.Update(movie))
                {
                    LoadMovies();
                    ClientScript.RegisterStartupScript(this.GetType(), "success", "showToast('Movie updated successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "error", "showToast('Failed to update movie', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "error", $"showToast('Error: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        private void DeleteMovie(string movieId)
        {
            try
            {
                _movieRepository = new MovieRepository();

                if (_movieRepository.Delete(movieId))
                {
                    LoadMovies();
                    ClientScript.RegisterStartupScript(this.GetType(), "success", "showToast('Movie deleted successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "error", "showToast('Failed to delete movie', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "error", $"showToast('Error: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        private void MovieIdInput_Clear()
        {
            movieIdInput.Text = "";
            movieNameInput.Text = "";
            durationInput.Text = "";
            formatDropdown.SelectedValue = "";
        }

        protected void SetActiveLink(string linkId)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "setActive", $"setActiveLink('{linkId}');", true);
        }

        private string EscapeJs(string s) => s?.Replace("'", "\\'").Replace("\r", "").Replace("\n", " ") ?? "";
    }

}
