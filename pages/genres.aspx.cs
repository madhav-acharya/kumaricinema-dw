using KumariCinema.Models;
using KumariCinema.Repositories;
using System;
using System.Collections.Generic;

namespace KumariCinema.Admin
{
    public partial class genres : System.Web.UI.Page
    {
        private GenreRepository _genreRepository;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["CurrentUser"] == null) Response.Redirect("~/components/Login.aspx");
                LoadGenres();
                SetActiveLink("genresLink");
            }
            else if (Request.Form["deleteId"] != null)
            {
                Delete(Request.Form["deleteId"]);
            }
        }

        private void LoadGenres()
        {
            try
            {
                _genreRepository = new GenreRepository();
                List<Genre> genres = _genreRepository.GetAll();
                genresRepeater.DataSource = genres;
                genresRepeater.DataBind();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error: {ex.Message}', 'error');", true);
            }
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid) return;

                _genreRepository = new GenreRepository();
                var genre = new Genre
                {
                    GenreId = genreIdInput.Text,
                    Name = nameInput.Text,
                    Description = descriptionInput.InnerText
                };

                if (_genreRepository.Insert(genre))
                {
                    genreIdInput.Text = "";
                    nameInput.Text = "";
                    descriptionInput.InnerText = "";
                    LoadGenres();
                    ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Added successfully', 'success');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error: {ex.Message}', 'error');", true);
            }
        }

        protected void Update_Click(object sender, EventArgs e)
        {
            try
            {
                _genreRepository = new GenreRepository();
                var genre = new Genre
                {
                    GenreId = editIdField.Value,
                    Name = editNameInput.Text,
                    Description = editDescriptionInput.InnerText
                };

                if (_genreRepository.Update(genre))
                {
                    LoadGenres();
                    ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Updated successfully', 'success');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error: {ex.Message}', 'error');", true);
            }
        }

        private void Delete(string id)
        {
            try
            {
                _genreRepository = new GenreRepository();
                if (_genreRepository.Delete(id))
                {
                    LoadGenres();
                    ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Deleted successfully', 'success');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error: {ex.Message}', 'error');", true);
            }
        }

        protected void SetActiveLink(string linkId)
        {
            ClientScript.RegisterStartupScript(GetType(), "setActive", $"setActiveLink('{linkId}');", true);
        }
    }
}
