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
        private AuthorizationService _authorizationService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CurrentUser"] == null)
            {
                Response.Redirect("~/components/Login.aspx");
                return;
            }

            _authorizationService = new AuthorizationService();
            var user = (AppUser)Session["CurrentUser"];
            if (!_authorizationService.CanViewBookings(user, user.TheaterId))
            {
                Response.Redirect("~/components/Login.aspx");
                return;
            }

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
            if (_authorizationService.IsSuperAdmin(user))
            {
                return _showRepository.GetAll().Select(s => s.ShowId).ToList();
            }

            _hallRepository = new HallRepository();
            var hallIds = new HashSet<string>(_hallRepository.GetByTheaterId(user.TheaterId).Select(h => h.HallId));
            return _showRepository.GetAll().Where(s => hallIds.Contains(s.HallId)).Select(s => s.ShowId).ToList();
        }

        private void LoadShows(AppUser user)
        {
            _showRepository = new MovieShowRepository();
            var shows = _showRepository.GetAll();
            var allowedIds = new HashSet<string>(GetAllowedShowIds(user));
            var data = shows.Where(s => allowedIds.Contains(s.ShowId)).ToList();

            showDropdown.DataSource = data;
            showDropdown.DataTextField = "ShowId";
            showDropdown.DataValueField = "ShowId";
            showDropdown.DataBind();

            editShowDropdown.DataSource = data;
            editShowDropdown.DataTextField = "ShowId";
            editShowDropdown.DataValueField = "ShowId";
            editShowDropdown.DataBind();
        }

        private void LoadBookings(AppUser user)
        {
            _bookingRepository = new BookingRepository();
            var all = _bookingRepository.GetAll();
            var allowedShowIds = new HashSet<string>(GetAllowedShowIds(user));
            var data = _authorizationService.IsSuperAdmin(user) ? all : all.Where(b => allowedShowIds.Contains(b.ShowId)).ToList();
            bookingsRepeater.DataSource = data;
            bookingsRepeater.DataBind();
        }

        protected void SaveBooking_Click(object sender, EventArgs e)
        {
            var user = (AppUser)Session["CurrentUser"];
            var allowed = new HashSet<string>(GetAllowedShowIds(user));
            if (!allowed.Contains(showDropdown.SelectedValue))
            {
                ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Access denied for selected show', 'error');", true);
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
                ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Booking added successfully', 'success');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Failed to add booking', 'error');", true);
            }
        }

        protected void UpdateBooking_Click(object sender, EventArgs e)
        {
            var user = (AppUser)Session["CurrentUser"];
            var allowed = new HashSet<string>(GetAllowedShowIds(user));
            if (!allowed.Contains(editShowDropdown.SelectedValue))
            {
                ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Access denied for selected show', 'error');", true);
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
                ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Booking updated successfully', 'success');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Failed to update booking', 'error');", true);
            }
        }

        private void DeleteBooking(string bookingId, AppUser user)
        {
            _bookingRepository = new BookingRepository();
            var booking = _bookingRepository.GetById(bookingId);
            if (booking == null)
            {
                return;
            }

            var allowed = new HashSet<string>(GetAllowedShowIds(user));
            if (!_authorizationService.IsSuperAdmin(user) && !allowed.Contains(booking.ShowId))
            {
                ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Access denied for selected booking', 'error');", true);
                return;
            }

            if (_bookingRepository.Delete(bookingId))
            {
                LoadBookings(user);
                ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Booking deleted successfully', 'success');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Failed to delete booking', 'error');", true);
            }
        }
    }
}
