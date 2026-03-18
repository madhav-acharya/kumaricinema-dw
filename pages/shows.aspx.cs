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
                Response.Redirect("~/pages/Login.aspx");
                return;
            }

            _authorizationService = new AuthorizationService();
            var currentUser = (AppUser)Session["CurrentUser"];

            if (!_authorizationService.IsAdminLevel(currentUser) && !_authorizationService.IsStaff(currentUser))
            {
                Response.Redirect("~/pages/Login.aspx");
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
            try
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
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "loadDropErr", $"showToast('Error: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        private void LoadShows(AppUser currentUser)
        {
        try
        {
                _showRepository = new MovieShowRepository();
                _movieRepository = new MovieRepository();
                _hallRepository = new HallRepository();

                var shows = _authorizationService.IsSuperAdmin(currentUser)
                    ? _showRepository.GetAll()
                    : _showRepository.GetByTheaterId(currentUser.TheaterId);

                var movieLookup = _movieRepository.GetAll().ToDictionary(m => m.MovieId, m => m.Name);
                var hallLookup = _hallRepository.GetAll().ToDictionary(h => h.HallId, h => h.HallName);

                var data = shows.Select(s => new
                {
                    s.ShowId,
                    s.MovieId,
                    MovieName = movieLookup.ContainsKey(s.MovieId) ? movieLookup[s.MovieId] : s.MovieId,
                    s.HallId,
                    HallName = hallLookup.ContainsKey(s.HallId) ? hallLookup[s.HallId] : s.HallId,
                    s.StartTime,
                    s.EndTime,
                    s.ShowCategory,
                    s.BaseTicketPrice
                }).ToList();

                showsRepeater.DataSource = data;
                showsRepeater.DataBind();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(GetType(), "loadShowsErr", $"showToast('Error: {EscapeJs(ex.Message)}', 'error');", true);
        }
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
                    modalStateField.Value = "";
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
                ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error: {EscapeJs(ex.Message)}', 'error');", true);
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
                    modalStateField.Value = "";
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
                ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error: {EscapeJs(ex.Message)}', 'error');", true);
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
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Show not found', 'error');", true);
                    return;
                }

                if (!CanAccessHall(currentUser, show.HallId))
                {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Access denied for selected show', 'error');", true);
                    return;
                }

                var ticketRepository = new TicketRepository();
                var bookingRepository = new BookingRepository();
                var paymentRepository = new PaymentRepository();

                var tickets = ticketRepository.GetByShowId(showId);
                foreach (var ticket in tickets)
                {
                    ticketRepository.Delete(ticket.TicketId);
                }

                var bookings = bookingRepository.GetAll().Where(b => b.ShowId == showId).ToList();
                foreach (var booking in bookings)
                {
                    var payments = paymentRepository.GetByBookingId(booking.BookingId);
                    foreach (var payment in payments)
                    {
                        paymentRepository.Delete(payment.PaymentId);
                    }

                    var seats = bookingRepository.GetSeatsByBookingId(booking.BookingId);
                    foreach (var seatId in seats)
                    {
                        bookingRepository.RemoveSeatFromBooking(booking.BookingId, seatId);
                    }

                    bookingRepository.Delete(booking.BookingId);
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
                ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error: {EscapeJs(ex.Message)}', 'error');", true);
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

        private string EscapeJs(string s) => s?.Replace("'", "\\'").Replace("\r", "").Replace("\n", " ") ?? "";
    }
}
