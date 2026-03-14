using KumariCinema.Models;
using KumariCinema.Repositories;
using KumariCinema.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KumariCinema.Admin
{
    public partial class dashboard : System.Web.UI.Page
    {
        private BookingRepository _bookingRepository;
        private PaymentRepository _paymentRepository;
        private MovieShowRepository _showRepository;
        private TheaterRepository _theaterRepository;
        private AppUserRepository _userRepository;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CheckAuthorization();
                LoadDashboardData();
                SetActiveLink("dashboardLink");
            }
        }

        private void CheckAuthorization()
        {
            if (Session["CurrentUser"] == null)
            {
                Response.Redirect("~/components/Login.aspx");
            }
        }

        private void LoadDashboardData()
        {
            try
            {
                _bookingRepository = new BookingRepository();
                _paymentRepository = new PaymentRepository();
                _showRepository = new MovieShowRepository();
                _theaterRepository = new TheaterRepository();
                _userRepository = new AppUserRepository();

                AppUser currentUser = (AppUser)Session["CurrentUser"];
                var authorizationService = new AuthorizationService();

                var allBookings = _bookingRepository.GetAll();
                var allPayments = _paymentRepository.GetAll();
                var allShows = _showRepository.GetAll();
                var allTheaters = _theaterRepository.GetAll();
                var allUsers = _userRepository.GetAll();

                List<Booking> userBookings = allBookings;
                List<Payment> userPayments = allPayments;
                List<MovieShow> userShows = allShows;

                if (!authorizationService.IsSuperAdmin(currentUser))
                {
                    userBookings = allBookings.Where(b => b.Show != null &&
                        allShows.FirstOrDefault(s => s.ShowId == b.ShowId)?.Hall != null &&
                        allShows.FirstOrDefault(s => s.ShowId == b.ShowId)?.Hall.TheaterId == currentUser.TheaterId).ToList();

                    userPayments = allPayments.Where(p => userBookings.Any(b => b.BookingId == p.BookingId)).ToList();

                    userShows = allShows.Where(s => s.Hall != null && s.Hall.TheaterId == currentUser.TheaterId).ToList();
                }

                totalBookingsLabel.Text = userBookings.Count.ToString();
                totalRevenueLabel.Text = Math.Round(userPayments.Where(p => p.PaymentStatus == "completed").Sum(p => p.AmountPaid), 0).ToString("N0");
                activeShowsLabel.Text = userShows.Count(s => s.StartTime > DateTime.Now).ToString();
                totalUsersLabel.Text = allUsers.Count.ToString();

                LoadBookingsTrendChart(userBookings);
                LoadPaymentMethodsChart(userPayments);
                LoadShowCategoryChart(userShows);
                LoadTheaterPerformanceChart(userBookings, allTheaters);
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "error", $"showToast('Error loading dashboard: {ex.Message}', 'error');", true);
            }
        }

        private void LoadBookingsTrendChart(List<Booking> bookings)
        {
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.Now.AddDays(-i).Date)
                .OrderBy(d => d)
                .ToList();

            var data = last7Days.Select(date =>
                bookings.Count(b => ((DateTime)b.Show.StartTime).Date == date)
            ).ToList();

            bookingsTrendData.Text = string.Join(",", data);
        }

        private void LoadPaymentMethodsChart(List<Payment> payments)
        {
            var methods = new[] { "cash", "card", "esewa", "khalti", "bank_transfer" };
            var data = methods.Select(m => payments.Count(p => p.PaymentMethod == m)).ToList();
            paymentMethodsData.Text = string.Join(",", data);
        }

        private void LoadShowCategoryChart(List<MovieShow> shows)
        {
            var categories = new[] { "regular", "premium", "special" };
            var data = categories.Select(c => shows.Count(s => s.ShowCategory == c)).ToList();
            showCategoryData.Text = string.Join(",", data);
        }

        private void LoadTheaterPerformanceChart(List<Booking> bookings, List<Theater> theaters)
        {
            var labels = new List<string>();
            var data = new List<int>();

            foreach (var theater in theaters)
            {
                labels.Add(theater.Name);
                int count = bookings.Count(b => true);
                data.Add(count);
            }

            theaterLabels.Text = string.Join(",", labels.Select(l => $"'{l}'"));
            theaterData.Text = string.Join(",", data);
        }

        protected void SetActiveLink(string linkId)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "setActive", $"setActiveLink('{linkId}');", true);
        }
    }
}
