using KumariCinema.Models;
using KumariCinema.Repositories;
using KumariCinema.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KumariCinema.Admin
{
    public partial class bookings : System.Web.UI.Page
    {
        private BookingRepository _bookingRepository;
        private MovieShowRepository _showRepository;
        private HallRepository _hallRepository;
        private MovieRepository _movieRepository;
        private AppUserRepository _userRepository;
        private AuthorizationService _authorizationService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CurrentUser"] == null) { Response.Redirect("~/pages/Login.aspx"); return; }

            _authorizationService = new AuthorizationService();
            var user = (AppUser)Session["CurrentUser"];
            if (!_authorizationService.CanViewBookings(user, user.TheaterId)) { Response.Redirect("~/pages/Login.aspx"); return; }

            if (!IsPostBack)
            {
                LoadShows(user);
                LoadBookings(user);
            }

            string deleteId = Request.Form["deleteBookingId"];
            if (!string.IsNullOrEmpty(deleteId))
            {
                DeleteBooking(deleteId, user);
            }
        }

        private List<string> GetAllowedShowIds(AppUser user)
        {
            _showRepository = new MovieShowRepository();
            return _authorizationService.IsSuperAdmin(user)
                ? _showRepository.GetAll().Select(s => s.ShowId).ToList()
                : _showRepository.GetByTheaterId(user.TheaterId).Select(s => s.ShowId).ToList();
        }

        private void LoadShows(AppUser user)
        {
            try
            {
                _showRepository = new MovieShowRepository();
                _movieRepository = new MovieRepository();

                var shows = _authorizationService.IsSuperAdmin(user)
                    ? _showRepository.GetAll()
                    : _showRepository.GetByTheaterId(user.TheaterId);

                var movieLookup = _movieRepository.GetAll().ToDictionary(m => m.MovieId, m => m.Name);
                var data = shows.Select(s => new
                {
                    s.ShowId,
                    DisplayText = string.Format(
                        "{0} - {1:yyyy-MM-dd HH:mm}",
                        movieLookup.ContainsKey(s.MovieId) ? movieLookup[s.MovieId] : s.MovieId,
                        s.StartTime
                    )
                }).ToList();

                showDropdown.DataSource = data;
                showDropdown.DataTextField = "DisplayText";
                showDropdown.DataValueField = "ShowId";
                showDropdown.DataBind();

                editShowDropdown.DataSource = data;
                editShowDropdown.DataTextField = "DisplayText";
                editShowDropdown.DataValueField = "ShowId";
                editShowDropdown.DataBind();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "loadShowsErr", $"showToast('Error loading shows: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        private void LoadBookings(AppUser user)
        {
            try
            {
                _bookingRepository = new BookingRepository();
                _showRepository = new MovieShowRepository();
                _movieRepository = new MovieRepository();
                _userRepository = new AppUserRepository();

                var bookingsData = _authorizationService.IsSuperAdmin(user)
                    ? _bookingRepository.GetAll()
                    : _bookingRepository.GetByTheaterId(user.TheaterId);

                var shows = _showRepository.GetAll();
                var movies = _movieRepository.GetAll();
                var users = _userRepository.GetAll();

                var showLookup = shows.ToDictionary(s => s.ShowId, s => s);
                var movieLookup = movies.ToDictionary(m => m.MovieId, m => m.Name);
                var userLookup = users.ToDictionary(u => u.UserId, u => u.Name);

                var data = bookingsData.Select(b =>
                {
                    var userDisplay = userLookup.ContainsKey(b.UserId) ? userLookup[b.UserId] : b.UserId;
                    var showDisplay = b.ShowId;

                    MovieShow show;
                    if (showLookup.TryGetValue(b.ShowId, out show))
                    {
                        var movieName = movieLookup.ContainsKey(show.MovieId) ? movieLookup[show.MovieId] : show.MovieId;
                        showDisplay = string.Format("{0} - {1:yyyy-MM-dd HH:mm}", movieName, show.StartTime);
                    }

                    return new
                    {
                        b.BookingId,
                        b.UserId,
                        UserDisplay = userDisplay,
                        b.ShowId,
                        ShowDisplay = showDisplay,
                        b.TotalAmount
                    };
                }).ToList();

                bookingsRepeater.DataSource = data;
                bookingsRepeater.DataBind();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "loadBookingsErr", $"showToast('Error loading bookings: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        protected void SaveBooking_Click(object sender, EventArgs e)
        {
            try
            {
                var user = (AppUser)Session["CurrentUser"];
                var allowed = new HashSet<string>(GetAllowedShowIds(user));
                if (!allowed.Contains(showDropdown.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(GetType(), "saveErr", "showToast('Access denied for selected show', 'error');", true);
                    return;
                }

                _bookingRepository = new BookingRepository();
                var booking = new Booking
                {
                    UserId = userIdInput.Text.Trim(),
                    ShowId = showDropdown.SelectedValue,
                    TotalAmount = decimal.TryParse(totalAmountInput.Text, out decimal amt) ? amt : 0
                };

                if (_bookingRepository.Insert(booking))
                {
                    LoadBookings(user);
                    ClientScript.RegisterStartupScript(GetType(), "saveOk", "showToast('Booking added successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "saveErr", "showToast('Failed to add booking', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "saveErr", $"showToast('Error: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        protected void UpdateBooking_Click(object sender, EventArgs e)
        {
            try
            {
                var user = (AppUser)Session["CurrentUser"];
                var allowed = new HashSet<string>(GetAllowedShowIds(user));
                if (!allowed.Contains(editShowDropdown.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(GetType(), "updateErr", "showToast('Access denied for selected show', 'error');", true);
                    return;
                }

                _bookingRepository = new BookingRepository();
                var booking = new Booking
                {
                    BookingId = editBookingIdField.Value,
                    UserId = editUserIdInput.Text.Trim(),
                    ShowId = editShowDropdown.SelectedValue,
                    TotalAmount = decimal.TryParse(editTotalAmountInput.Text, out decimal amt) ? amt : 0
                };

                if (_bookingRepository.Update(booking))
                {
                    LoadBookings(user);
                    ClientScript.RegisterStartupScript(GetType(), "updateOk", "showToast('Booking updated successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "updateErr", "showToast('Failed to update booking', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "updateErr", $"showToast('Error: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        private void DeleteBooking(string bookingId, AppUser user)
        {
            try
            {
                _bookingRepository = new BookingRepository();
                var booking = _bookingRepository.GetById(bookingId);
                if (booking == null)
                {
                    ClientScript.RegisterStartupScript(GetType(), "deleteErr", "showToast('Booking not found', 'error');", true);
                    return;
                }

                var allowed = new HashSet<string>(GetAllowedShowIds(user));
                if (!_authorizationService.IsSuperAdmin(user) && !allowed.Contains(booking.ShowId))
                {
                    ClientScript.RegisterStartupScript(GetType(), "deleteErr", "showToast('Access denied for this booking', 'error');", true);
                    return;
                }

                if (_bookingRepository.Delete(bookingId))
                {
                    LoadBookings(user);
                    ClientScript.RegisterStartupScript(GetType(), "deleteOk", "showToast('Booking deleted successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "deleteErr", "showToast('Failed to delete booking', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "deleteErr", $"showToast('Error: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        private string EscapeJs(string s) => s?.Replace("'", "\\'").Replace("\r", "").Replace("\n", " ") ?? "";
    }
}
