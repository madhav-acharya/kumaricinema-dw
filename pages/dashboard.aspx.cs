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
        private HallRepository _hallRepository;
        private List<MovieShow> _currentShows;

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
                Response.Redirect("~/pages/Login.aspx");
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
                _hallRepository = new HallRepository();

                AppUser currentUser = (AppUser)Session["CurrentUser"];
                var authorizationService = new AuthorizationService();

                var allBookings = _bookingRepository.GetAll();
                var allPayments = _paymentRepository.GetAll();
                var allShows = _showRepository.GetAll();
                var allTheaters = _theaterRepository.GetAll();
                var allUsers = _userRepository.GetAll();
                var allHalls = _hallRepository.GetAll();

                var visibleTheaterIds = authorizationService.IsSuperAdmin(currentUser)
                    ? new HashSet<string>(allTheaters.Select(t => t.TheaterId))
                    : new HashSet<string>(new[] { currentUser.TheaterId });

                var visibleHallIds = new HashSet<string>(allHalls.Where(h => visibleTheaterIds.Contains(h.TheaterId)).Select(h => h.HallId));
                var visibleShowIds = new HashSet<string>(allShows.Where(s => visibleHallIds.Contains(s.HallId)).Select(s => s.ShowId));

                var userBookings = allBookings.Where(b => visibleShowIds.Contains(b.ShowId)).ToList();
                var userPayments = allPayments.Where(p => userBookings.Any(b => b.BookingId == p.BookingId)).ToList();
                var userShows = allShows.Where(s => visibleShowIds.Contains(s.ShowId)).ToList();
                var userUsers = authorizationService.IsSuperAdmin(currentUser)
                    ? allUsers
                    : allUsers.Where(u => u.TheaterId == currentUser.TheaterId).ToList();

                _currentShows = userShows;

                totalBookingsLabel.Text = userBookings.Count.ToString();
                totalRevenueLabel.Text = Math.Round(
                    userPayments
                        .Where(p => string.Equals(p.PaymentStatus, "completed", StringComparison.OrdinalIgnoreCase))
                        .Sum(p => p.AmountPaid),
                    0
                ).ToString("N0");
                activeShowsLabel.Text = userShows.Count(s => s.StartTime > DateTime.Now).ToString();
                totalUsersLabel.Text = userUsers.Count.ToString();

                LoadBookingsTrendChart(userBookings);
                LoadPaymentMethodsChart(userPayments);
                LoadShowCategoryChart(userShows);
                LoadTheaterPerformanceChart(userBookings, allTheaters, allShows, allHalls, visibleTheaterIds);
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "error", $"showToast('Error loading dashboard: {ex.Message}', 'error');", true);
            }
        }

        private void LoadBookingsTrendChart(List<Booking> bookings)
        {
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.Now.Date.AddDays(-6 + i))
                .ToList();

            var labels = last7Days.Select(d => d.ToString("ddd")).ToList();
            var data = last7Days
                .Select(date => bookings.Count(b => b.CreatedAt.Date == date.Date))
                .ToList();

            bookingsTrendLabels.Text = string.Join(",", labels.Select(l => $"'{l}'"));
            bookingsTrendData.Text = string.Join(",", data);
        }

        private void LoadPaymentMethodsChart(List<Payment> payments)
        {
            Func<string, string> normalizeMethod = method =>
                string.IsNullOrWhiteSpace(method)
                    ? string.Empty
                    : method.Replace("_", " ").Replace("-", " ").Trim().ToLowerInvariant();

            var methods = new[] { "cash", "card", "esewa", "khalti", "bank transfer" };
            var data = methods
                .Select(m => payments.Count(p => normalizeMethod(p.PaymentMethod) == m))
                .ToList();

            paymentMethodsData.Text = string.Join(",", data);
        }

        private void LoadShowCategoryChart(List<MovieShow> shows)
        {
            var grouped = shows
                .Where(s => !string.IsNullOrWhiteSpace(s.ShowCategory))
                .GroupBy(s => s.ShowCategory.Trim())
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key)
                .ToList();

            if (!grouped.Any())
            {
                showCategoryLabels.Text = "'No Data'";
                showCategoryData.Text = "0";
                return;
            }

            showCategoryLabels.Text = string.Join(",", grouped.Select(g => $"'{g.Key.Replace("'", "\\'")}'"));
            showCategoryData.Text = string.Join(",", grouped.Select(g => g.Count()));
        }

        private void LoadTheaterPerformanceChart(List<Booking> bookings, List<Theater> theaters, List<MovieShow> shows, List<Hall> halls, HashSet<string> visibleTheaterIds)
        {
            var labels = new List<string>();
            var data = new List<int>();
            var hallToTheater = halls.ToDictionary(h => h.HallId, h => h.TheaterId);
            var showToTheater = shows.Where(s => hallToTheater.ContainsKey(s.HallId)).ToDictionary(s => s.ShowId, s => hallToTheater[s.HallId]);

            foreach (var theater in theaters.Where(t => visibleTheaterIds.Contains(t.TheaterId)))
            {
                labels.Add(theater.Name);
                int count = bookings.Count(b => showToTheater.ContainsKey(b.ShowId) && showToTheater[b.ShowId] == theater.TheaterId);
                data.Add(count);
            }

            theaterLabels.Text = string.Join(",", labels.Select(l => $"'{l.Replace("'", "\\'")}'"));
            theaterData.Text = string.Join(",", data);
        }

        protected void SetActiveLink(string linkId)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "setActive", $"setActiveLink('{linkId}');", true);
        }
    }
}
