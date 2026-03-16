using KumariCinema.Models;
using KumariCinema.Repositories;
using KumariCinema.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KumariCinema.Admin
{
    public partial class tickets : System.Web.UI.Page
    {
        private TicketRepository _ticketRepository;
        private MovieShowRepository _showRepository;
        private HallRepository _hallRepository;
        private SeatRepository _seatRepository;
        private AuthorizationService _authorizationService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CurrentUser"] == null) { Response.Redirect("~/pages/Login.aspx"); return; }
            _authorizationService = new AuthorizationService();
            var currentUser = (AppUser)Session["CurrentUser"];
            if (!_authorizationService.CanManageShows(currentUser, currentUser.TheaterId)) { Response.Redirect("~/pages/Login.aspx"); return; }

            if (!IsPostBack)
            {
                LoadDropdowns(currentUser);
                LoadTickets(currentUser);
            }

            string deleteId = Request.Form["deleteTicketId"];
            if (!string.IsNullOrEmpty(deleteId))
            {
                DeleteTicket(deleteId, currentUser);
            }
        }

        private List<string> GetAllowedShowIds(AppUser user)
        {
            _showRepository = new MovieShowRepository();
            if (_authorizationService.IsSuperAdmin(user))
                return _showRepository.GetAll().Select(s => s.ShowId).ToList();

            _hallRepository = new HallRepository();
            var hallIds = new HashSet<string>(_hallRepository.GetByTheaterId(user.TheaterId).Select(h => h.HallId));
            return _showRepository.GetAll().Where(s => hallIds.Contains(s.HallId)).Select(s => s.ShowId).ToList();
        }

        private void LoadDropdowns(AppUser user)
        {
        try
        {
                _showRepository = new MovieShowRepository();
                _seatRepository = new SeatRepository();
    
                var allowedShowIds = new HashSet<string>(GetAllowedShowIds(user));
                var shows = _showRepository.GetAll().Where(s => allowedShowIds.Contains(s.ShowId)).ToList();
                var seats = _seatRepository.GetAll();
    
                showDropdown.DataSource = shows;
                showDropdown.DataTextField = "ShowId";
                showDropdown.DataValueField = "ShowId";
                showDropdown.DataBind();
    
                editShowDropdown.DataSource = shows;
                editShowDropdown.DataTextField = "ShowId";
                editShowDropdown.DataValueField = "ShowId";
                editShowDropdown.DataBind();
    
                seatDropdown.DataSource = seats;
                seatDropdown.DataTextField = "SeatNumber";
                seatDropdown.DataValueField = "SeatId";
                seatDropdown.DataBind();
    
                editSeatDropdown.DataSource = seats;
                editSeatDropdown.DataTextField = "SeatNumber";
                editSeatDropdown.DataValueField = "SeatId";
                editSeatDropdown.DataBind();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(GetType(), "loadDropErr", $"showToast('Error: {{EscapeJs(ex.Message)}}', 'error');", true);
        }
        }

        private void LoadTickets(AppUser user)
        {
            try
            {
                _ticketRepository = new TicketRepository();
                var all = _ticketRepository.GetAll();
                var allowedShowIds = new HashSet<string>(GetAllowedShowIds(user));
                var data = _authorizationService.IsSuperAdmin(user) ? all : all.Where(t => allowedShowIds.Contains(t.ShowId)).ToList();
                ticketsRepeater.DataSource = data;
                ticketsRepeater.DataBind();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error loading tickets: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        protected void SaveTicket_Click(object sender, EventArgs e)
        {
            try
            {
                var user = (AppUser)Session["CurrentUser"];
                var allowed = new HashSet<string>(GetAllowedShowIds(user));
                if (!allowed.Contains(showDropdown.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Access denied for selected show', 'error');", true);
                    return;
                }

                _ticketRepository = new TicketRepository();
                var ticket = new Ticket
                {
                    SeatId = seatDropdown.SelectedValue,
                    ShowId = showDropdown.SelectedValue,
                    TicketPrice = decimal.TryParse(ticketPriceInput.Text, out decimal p) ? p : 0,
                    TicketStatus = ticketStatusDropdown.SelectedValue
                };

                if (_ticketRepository.Insert(ticket))
                {
                    LoadTickets(user);
                    ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Ticket added successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Failed to add ticket', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        protected void UpdateTicket_Click(object sender, EventArgs e)
        {
            try
            {
                var user = (AppUser)Session["CurrentUser"];
                var allowed = new HashSet<string>(GetAllowedShowIds(user));
                if (!allowed.Contains(editShowDropdown.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Access denied for selected show', 'error');", true);
                    return;
                }

                _ticketRepository = new TicketRepository();
                var ticket = new Ticket
                {
                    TicketId = editTicketIdField.Value,
                    SeatId = editSeatDropdown.SelectedValue,
                    ShowId = editShowDropdown.SelectedValue,
                    TicketPrice = decimal.TryParse(editTicketPriceInput.Text, out decimal p) ? p : 0,
                    TicketStatus = editTicketStatusDropdown.SelectedValue
                };

                if (_ticketRepository.Update(ticket))
                {
                    LoadTickets(user);
                    ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Ticket updated successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Failed to update ticket', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        private void DeleteTicket(string ticketId, AppUser user)
        {
            try
            {
                _ticketRepository = new TicketRepository();
                var ticket = _ticketRepository.GetById(ticketId);
                if (ticket == null) return;

                var allowed = new HashSet<string>(GetAllowedShowIds(user));
                if (!_authorizationService.IsSuperAdmin(user) && !allowed.Contains(ticket.ShowId))
                {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Access denied', 'error');", true);
                    return;
                }

                if (_ticketRepository.Delete(ticketId))
                {
                    LoadTickets(user);
                    ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Ticket deleted successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Failed to delete ticket', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error: {EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        private string EscapeJs(string s) => s?.Replace("'", "\\'").Replace("\r", "").Replace("\n", " ") ?? "";
    }

}
