using KumariCinema.Models;
using KumariCinema.Repositories;
using KumariCinema.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KumariCinema.Admin
{
    public partial class payments : System.Web.UI.Page
    {
        private PaymentRepository _paymentRepository;
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
            if (!_authorizationService.CanManagePayments(user, user.TheaterId)) { Response.Redirect("~/pages/Login.aspx"); return; }

            if (!IsPostBack)
            {
                LoadBookings(user);
                LoadPayments(user);
            }

            string deleteId = Request.Form["deletePaymentId"];
            if (!string.IsNullOrEmpty(deleteId))
            {
                DeletePayment(deleteId, user);
            }
        }

        private List<string> GetAllowedBookingIds(AppUser user)
        {
            _bookingRepository = new BookingRepository();
            return _authorizationService.IsSuperAdmin(user)
                ? _bookingRepository.GetAll().Select(b => b.BookingId).ToList()
                : _bookingRepository.GetByTheaterId(user.TheaterId).Select(b => b.BookingId).ToList();
        }

        private void LoadBookings(AppUser user)
        {
            try
            {
                _bookingRepository = new BookingRepository();
                var bookings = _authorizationService.IsSuperAdmin(user)
                    ? _bookingRepository.GetAll()
                    : _bookingRepository.GetByTheaterId(user.TheaterId);

                var bookingDisplayLookup = GetBookingDisplayLookup(bookings);
                var data = bookings.Select(b => new
                {
                    b.BookingId,
                    DisplayText = bookingDisplayLookup.ContainsKey(b.BookingId) ? bookingDisplayLookup[b.BookingId] : b.BookingId
                }).ToList();

                bookingDropdown.DataSource = data;
                bookingDropdown.DataTextField = "DisplayText";
                bookingDropdown.DataValueField = "BookingId";
                bookingDropdown.DataBind();

                editBookingDropdown.DataSource = data;
                editBookingDropdown.DataTextField = "DisplayText";
                editBookingDropdown.DataValueField = "BookingId";
                editBookingDropdown.DataBind();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "loadBookingsErr", $"showToast('Error loading bookings: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        private void LoadPayments(AppUser user)
        {
            try
            {
                _paymentRepository = new PaymentRepository();
                _bookingRepository = new BookingRepository();

                var payments = _authorizationService.IsSuperAdmin(user)
                    ? _paymentRepository.GetAll()
                    : _paymentRepository.GetByTheaterId(user.TheaterId);

                var bookings = _authorizationService.IsSuperAdmin(user)
                    ? _bookingRepository.GetAll()
                    : _bookingRepository.GetByTheaterId(user.TheaterId);

                var bookingDisplayLookup = GetBookingDisplayLookup(bookings);

                var data = payments.Select(p => new
                {
                    p.PaymentId,
                    p.BookingId,
                    BookingDisplay = bookingDisplayLookup.ContainsKey(p.BookingId) ? bookingDisplayLookup[p.BookingId] : p.BookingId,
                    p.AmountPaid,
                    p.PaymentMethod,
                    p.PaymentStatus
                }).ToList();

                paymentsRepeater.DataSource = data;
                paymentsRepeater.DataBind();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "loadPaymentsErr", $"showToast('Error loading payments: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        private Dictionary<string, string> GetBookingDisplayLookup(List<Booking> bookings)
        {
            _showRepository = new MovieShowRepository();
            _movieRepository = new MovieRepository();
            _userRepository = new AppUserRepository();

            var shows = _showRepository.GetAll();
            var movies = _movieRepository.GetAll();
            var users = _userRepository.GetAll();

            var showLookup = shows.ToDictionary(s => s.ShowId, s => s);
            var movieLookup = movies.ToDictionary(m => m.MovieId, m => m.Name);
            var userLookup = users.ToDictionary(u => u.UserId, u => u.Name);

            return bookings.ToDictionary(
                b => b.BookingId,
                b =>
                {
                    var userName = userLookup.ContainsKey(b.UserId) ? userLookup[b.UserId] : b.UserId;
                    MovieShow show;
                    if (showLookup.TryGetValue(b.ShowId, out show))
                    {
                        var movieName = movieLookup.ContainsKey(show.MovieId) ? movieLookup[show.MovieId] : show.MovieId;
                        return string.Format("{0} - {1} - {2:yyyy-MM-dd HH:mm}", b.BookingId, userName, show.StartTime);
                    }

                    return string.Format("{0} - {1}", b.BookingId, userName);
                }
            );
        }

        protected void SavePayment_Click(object sender, EventArgs e)
        {
            try
            {
                var user = (AppUser)Session["CurrentUser"];
                var allowed = new HashSet<string>(GetAllowedBookingIds(user));
                if (!allowed.Contains(bookingDropdown.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(GetType(), "saveErr", "showToast('Access denied for selected booking', 'error');", true);
                    return;
                }

                _paymentRepository = new PaymentRepository();
                var payment = new Payment
                {
                    BookingId = bookingDropdown.SelectedValue,
                    AmountPaid = decimal.TryParse(amountInput.Text, out decimal amt) ? amt : 0,
                    PaymentMethod = methodDropdown.SelectedValue,
                    PaymentStatus = paymentStatusDropdown.SelectedValue
                };

                if (_paymentRepository.Insert(payment))
                {
                    LoadPayments(user);
                    ClientScript.RegisterStartupScript(GetType(), "saveOk", "showToast('Payment added successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "saveErr", "showToast('Failed to add payment', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "saveErr", $"showToast('Error: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        protected void UpdatePayment_Click(object sender, EventArgs e)
        {
            try
            {
                var user = (AppUser)Session["CurrentUser"];
                var allowed = new HashSet<string>(GetAllowedBookingIds(user));
                if (!allowed.Contains(editBookingDropdown.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(GetType(), "updateErr", "showToast('Access denied for selected booking', 'error');", true);
                    return;
                }

                _paymentRepository = new PaymentRepository();
                var payment = new Payment
                {
                    PaymentId = editPaymentIdField.Value,
                    BookingId = editBookingDropdown.SelectedValue,
                    AmountPaid = decimal.TryParse(editAmountInput.Text, out decimal amt) ? amt : 0,
                    PaymentMethod = editMethodDropdown.SelectedValue,
                    PaymentStatus = editPaymentStatusDropdown.SelectedValue
                };

                if (_paymentRepository.Update(payment))
                {
                    LoadPayments(user);
                    ClientScript.RegisterStartupScript(GetType(), "updateOk", "showToast('Payment updated successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "updateErr", "showToast('Failed to update payment', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "updateErr", $"showToast('Error: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        private void DeletePayment(string paymentId, AppUser user)
        {
            try
            {
                _paymentRepository = new PaymentRepository();
                var payment = _paymentRepository.GetById(paymentId);
                if (payment == null)
                {
                    ClientScript.RegisterStartupScript(GetType(), "deleteErr", "showToast('Payment not found', 'error');", true);
                    return;
                }

                var allowed = new HashSet<string>(GetAllowedBookingIds(user));
                if (!_authorizationService.IsSuperAdmin(user) && !allowed.Contains(payment.BookingId))
                {
                    ClientScript.RegisterStartupScript(GetType(), "deleteErr", "showToast('Access denied for this payment', 'error');", true);
                    return;
                }

                if (_paymentRepository.Delete(paymentId))
                {
                    LoadPayments(user);
                    ClientScript.RegisterStartupScript(GetType(), "deleteOk", "showToast('Payment deleted successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "deleteErr", "showToast('Failed to delete payment', 'error');", true);
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
