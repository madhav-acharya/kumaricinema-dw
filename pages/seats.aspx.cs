using KumariCinema.Models;
using KumariCinema.Repositories;
using KumariCinema.Services;
using System;

namespace KumariCinema.Admin
{
    public partial class seats : System.Web.UI.Page
    {
        private SeatRepository _seatRepository;
        private SeatTypeRepository _seatTypeRepository;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CurrentUser"] == null)
            {
                Response.Redirect("~/components/Login.aspx");
                return;
            }

            var auth = new AuthorizationService();
            var user = (AppUser)Session["CurrentUser"];
            if (!auth.CanManageShows(user, user.TheaterId))
            {
                Response.Redirect("~/components/Login.aspx");
                return;
            }

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

        private void LoadSeats()
        {
            _seatRepository = new SeatRepository();
            seatsRepeater.DataSource = _seatRepository.GetAll();
            seatsRepeater.DataBind();
        }

        protected void SaveSeat_Click(object sender, EventArgs e)
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
                LoadSeats();
                ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Seat added successfully', 'success');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Failed to add seat', 'error');", true);
            }
        }

        protected void UpdateSeat_Click(object sender, EventArgs e)
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
                LoadSeats();
                ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Seat updated successfully', 'success');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Failed to update seat', 'error');", true);
            }
        }

        private void DeleteSeat(string seatId)
        {
            _seatRepository = new SeatRepository();
            if (_seatRepository.Delete(seatId))
            {
                LoadSeats();
                ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Seat deleted successfully', 'success');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(GetType(), "error", "showToast('Failed to delete seat', 'error');", true);
            }
        }
    }
}
