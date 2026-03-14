using KumariCinema.Models;
using KumariCinema.Repositories;
using System;
using System.Collections.Generic;

namespace KumariCinema.Admin
{
    public partial class languages : System.Web.UI.Page
    {
        private LanguageRepository _repo;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["CurrentUser"] == null) Response.Redirect("~/components/Login.aspx");
                Load();
                SetActiveLink("languagesLink");
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
                _repo = new LanguageRepository();
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
                _repo = new LanguageRepository();
                if (_repo.Insert(new Language { LanguageId = idInput.Text, Name = nameInput.Text, Code = codeInput.Text }))
                {
                    idInput.Text = nameInput.Text = codeInput.Text = "";
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
                _repo = new LanguageRepository();
                if (_repo.Update(new Language { LanguageId = editIdField.Value, Name = editNameInput.Text, Code = editCodeInput.Text }))
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
                _repo = new LanguageRepository();
                if (_repo.Delete(id)) { Load(); ClientScript.RegisterStartupScript(GetType(), "s", "showToast('Deleted', 'success');", true); }
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
