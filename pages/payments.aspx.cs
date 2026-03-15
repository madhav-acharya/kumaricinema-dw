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
            if (!_authorizationService.CanManagePayments(user, user.TheaterId))
            {
                Response.Redirect("~/components/Login.aspx");
                return;
            }

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
            if (_authorizationService.IsSuperAdmin(user))
            {
                return _bookingRepository.GetAll().Select(b => b.BookingId).ToList();
            }

            _showRepository = new MovieShowRepository();
            _hallRepository = new HallRepository();
            var hallIds = new HashSet<string>(_hallRepository.GetByTheaterId(user.TheaterId).Select(h => h.HallId));
            var showIds = new HashSet<string>(_showRepository.GetAll().Where(s => hallIds.Contains(s.HallId)).Select(s => s.ShowId));
            return _bookingRepository.GetAll().Where(b => showIds.Contains(b.ShowId)).Select(b => b.BookingId).ToList();
        }

        private void LoadBookings(AppUser user)
        {
            _bookingRepository = new BookingRepository();
            var all = _bookingRepository.GetAll();
            var allowed = new HashSet<string>(GetAllowedBookingIds(user));
            var data = all.Where(b => allowed.Contains(b.BookingId)).ToList();

            bookingDropdown.DataSource = data;
            bookingDropdown.DataTextField = "BookingId";
            bookingDropdown.DataValueField = "BookingId";
            bookingDropdown.DataBind();

            editBookingDropdown.DataSource = data;
            editBookingDropdown.DataTextField = "BookingId";
            editBookingDropdown.DataValueField = "BookingId";
            editBookingDropdown.DataBind();
        }

        private void LoadPayments(AppUser user)
        {
            _paymentRepository = new PaymentRepository();
            var all = _paymentRepository.GetAll();
            var allowed = new HashSet<string>(GetAllowedBookingIds(user));
            var data = _authorizationService.IsSuperAdmin(user) ? all : all.Where(p => allowed.Contains(p.BookingId)).ToList();

            paymentsRepeater.DataSource = data;
            paymentsRepeater.DataBind();
        }

        protected void SavePayment_Click(object sender, EventArgs e)
        {
            var user = (AppUser)Session["CurrentUser"];
            var allowed = new HashSet<string>(GetAllowedBookingIds(user));
            if (!allowed.Contains(bookingDropdown.SelectedValue))
            {
                ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Access denied for selected booking', 'error');", true);
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
                ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Payment added successfully', 'success');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Failed to add payment', 'error');", true);
            }
        }

        protected void UpdatePayment_Click(object sender, EventArgs e)
        {
            var user = (AppUser)Session["CurrentUser"];
            var allowed = new HashSet<string>(GetAllowedBookingIds(user));
            if (!allowed.Contains(editBookingDropdown.SelectedValue))
            {
                ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Access denied for selected booking', 'error');", true);
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
                ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Payment updated successfully', 'success');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Failed to update payment', 'error');", true);
            }
        }

        private void DeletePayment(string paymentId, AppUser user)
        {
            _paymentRepository = new PaymentRepository();
            var payment = _paymentRepository.GetById(paymentId);
            if (payment == null)
            {
                return;
            }

            var allowed = new HashSet<string>(GetAllowedBookingIds(user));
            if (!_authorizationService.IsSuperAdmin(user) && !allowed.Contains(payment.BookingId))
            {
                ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Access denied for selected payment', 'error');", true);
                return;
            }

            if (_paymentRepository.Delete(paymentId))
            {
                LoadPayments(user);
                ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Payment deleted successfully', 'success');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Failed to delete payment', 'error');", true);
            }
        }
    }
}
