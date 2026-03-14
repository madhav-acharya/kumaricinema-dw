using KumariCinema.Models;
using KumariCinema.Repositories;
using System;
using System.Collections.Generic;

namespace KumariCinema.Admin
{
    public partial class seattypes : System.Web.UI.Page
    {
        private SeatTypeRepository _repo;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["CurrentUser"] == null) Response.Redirect("~/components/Login.aspx");
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
                ClientScript.RegisterStartupScript(GetType(), "e", $"showToast('{ex.Message}', 'error');", true);
            }
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            try
            {
                _repo = new SeatTypeRepository();
                if (_repo.Insert(new SeatType
                {
                    SeatTypeId = idInput.Text,
                    Name = nameInput.Text,
                    Description = descInput.Text,
                    PriceMultiplier = decimal.Parse(priceInput.Text)
                }))
                {
                    idInput.Text = nameInput.Text = descInput.Text = priceInput.Text = "";
                    Load();
                    ClientScript.RegisterStartupScript(GetType(), "s", "showToast('Added', 'success');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "e", $"showToast('{ex.Message}', 'error');", true);
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
                    Load();
                    ClientScript.RegisterStartupScript(GetType(), "s", "showToast('Updated', 'success');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "e", $"showToast('{ex.Message}', 'error');", true);
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
                    ClientScript.RegisterStartupScript(GetType(), "s", "showToast('Deleted', 'success');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(GetType(), "e", $"showToast('{ex.Message}', 'error');", true);
            }
        }

        protected void SetActiveLink(string linkId)
        {
            ClientScript.RegisterStartupScript(GetType(), "setActive", $"setActiveLink('{linkId}');", true);
        }
    }
}
