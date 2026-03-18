using KumariCinema.Models;
using KumariCinema.Repositories;
using KumariCinema.Services;
using System;
using System.Linq;

namespace KumariCinema.Admin
{
    public partial class seats : System.Web.UI.Page
    {
        private SeatRepository _seatRepository;
        private SeatTypeRepository _seatTypeRepository;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CurrentUser"] == null) { Response.Redirect("~/pages/Login.aspx"); return; }

            var auth = new AuthorizationService();
            var user = (AppUser)Session["CurrentUser"];
            if (!auth.CanManageShows(user, user.TheaterId)) { Response.Redirect("~/pages/Login.aspx"); return; }

            if (!IsPostBack)
            {
                LoadSeatTypes();
                LoadSeats();
            }

            string deleteId = Request.Form["deleteSeatId"];
            if (!string.IsNullOrEmpty(deleteId))
            {
                DeleteSeat(deleteId);
            }
        }

        private void LoadSeatTypes()
        {
            try
            {
                _seatTypeRepository = new SeatTypeRepository();
                var data = _seatTypeRepository.GetAll();

                seatTypeDropdown.DataSource = data;
                seatTypeDropdown.DataTextField = "Name";
                seatTypeDropdown.DataValueField = "SeatTypeId";
                seatTypeDropdown.DataBind();

                editSeatTypeDropdown.DataSource = data;
                editSeatTypeDropdown.DataTextField = "Name";
                editSeatTypeDropdown.DataValueField = "SeatTypeId";
                editSeatTypeDropdown.DataBind();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "loadTypesErr", $"showToast('Error loading seat types: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        private void LoadSeats()
        {
            try
            {
                _seatRepository = new SeatRepository();
                seatsRepeater.DataSource = _seatRepository.GetAll();
                seatsRepeater.DataBind();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "loadSeatsErr", $"showToast('Error loading seats: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        protected void SaveSeat_Click(object sender, EventArgs e)
        {
            try
            {
                _seatRepository = new SeatRepository();
                var seat = new Seat
                {
                    SeatNumber = seatNumberInput.Text.Trim(),
                    Status = statusDropdown.SelectedValue,
                    SeatTypeId = seatTypeDropdown.SelectedValue
                };

                if (_seatRepository.Insert(seat))
                {
                    modalStateField.Value = "";
                    LoadSeats();
                    ClientScript.RegisterStartupScript(GetType(), "saveOk", "showToast('Seat added successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "saveErr", "showToast('Failed to add seat', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "saveErr", $"showToast('Error: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        protected void UpdateSeat_Click(object sender, EventArgs e)
        {
            try
            {
                _seatRepository = new SeatRepository();
                var seat = new Seat
                {
                    SeatId = editSeatIdField.Value,
                    SeatNumber = editSeatNumberInput.Text.Trim(),
                    Status = editStatusDropdown.SelectedValue,
                    SeatTypeId = editSeatTypeDropdown.SelectedValue
                };

                if (_seatRepository.Update(seat))
                {
                    modalStateField.Value = "";
                    LoadSeats();
                    ClientScript.RegisterStartupScript(GetType(), "updateOk", "showToast('Seat updated successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "updateErr", "showToast('Failed to update seat', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "updateErr", $"showToast('Error: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        private void DeleteSeat(string seatId)
        {
            try
            {
                _seatRepository = new SeatRepository();

                var ticketRepository = new TicketRepository();
                var bookingRepository = new BookingRepository();

                var tickets = ticketRepository.GetAll().Where(t => t.SeatId == seatId).ToList();
                foreach (var ticket in tickets)
                {
                    ticketRepository.Delete(ticket.TicketId);
                }

                var bookings = bookingRepository.GetAll();
                foreach (var booking in bookings)
                {
                    var bookingSeats = bookingRepository.GetSeatsByBookingId(booking.BookingId);
                    if (bookingSeats.Contains(seatId))
                    {
                        bookingRepository.RemoveSeatFromBooking(booking.BookingId, seatId);
                    }
                }

                if (_seatRepository.Delete(seatId))
                {
                    LoadSeats();
                    ClientScript.RegisterStartupScript(GetType(), "deleteOk", "showToast('Seat deleted successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "deleteErr", "showToast('Failed to delete seat', 'error');", true);
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
