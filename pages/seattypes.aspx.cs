using KumariCinema.Models;
using KumariCinema.Repositories;
using KumariCinema.Services;
using System;
using System.Collections.Generic;

namespace KumariCinema.Admin
{
    public partial class seattypes : System.Web.UI.Page
    {
        private SeatTypeRepository _repo;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CurrentUser"] == null) { Response.Redirect("~/pages/Login.aspx"); return; }
            var currentUser = (AppUser)Session["CurrentUser"];
            if (!new AuthorizationService().IsAdminLevel(currentUser)) { Response.Redirect("~/pages/Login.aspx"); return; }

            if (!IsPostBack)
            {
                Load();
                SetActiveLink("seatTypesLink");
            }
            else if (Request.Form["delId"] != null)
            {
                Delete(Request.Form["delId"]);
            }
        }

        private void Load()
        {
            try
            {
                _repo = new SeatTypeRepository();
                repeater.DataSource = _repo.GetAll();
                repeater.DataBind();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "e", $"showToast('{EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            try
            {
                _repo = new SeatTypeRepository();
                if (_repo.Insert(new SeatType
                {
                    Name = nameInput.Text,
                    Description = descInput.Text,
                    PriceMultiplier = decimal.Parse(priceInput.Text)
                }))
                {
                    idInput.Text = nameInput.Text = descInput.Text = priceInput.Text = "";
                    modalStateField.Value = "";
                    Load();
                    ClientScript.RegisterStartupScript(GetType(), "s", "showToast('Seat type added successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "e", "showToast('Failed to add seat type', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "e", $"showToast('{EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        protected void Update_Click(object sender, EventArgs e)
        {
            try
            {
                _repo = new SeatTypeRepository();
                if (_repo.Update(new SeatType
                {
                    SeatTypeId = editIdField.Value,
                    Name = editNameInput.Text,
                    Description = editDescInput.Text,
                    PriceMultiplier = decimal.Parse(editPriceInput.Text)
                }))
                {
                    modalStateField.Value = "";
                    Load();
                    ClientScript.RegisterStartupScript(GetType(), "s", "showToast('Seat type updated successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "e", "showToast('Failed to update seat type', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "e", $"showToast('{EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        private void Delete(string id)
        {
            try
            {
                _repo = new SeatTypeRepository();
                if (_repo.Delete(id))
                {
                    Load();
                    ClientScript.RegisterStartupScript(GetType(), "s", "showToast('Seat type deleted successfully', 'success');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "e", "showToast('Failed to delete seat type', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "e", $"showToast('{EscapeJs(ex.Message)}', 'error');", true);
            }
        }

        protected void SetActiveLink(string linkId)
        {
            ClientScript.RegisterStartupScript(GetType(), "setActive", $"setActiveLink('{linkId}');", true);
        }

        private string EscapeJs(string s) => s?.Replace("'", "\\'").Replace("\r", "").Replace("\n", " ") ?? "";
    }

}
