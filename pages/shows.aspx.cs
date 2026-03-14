using System;
namespace KumariCinema.Admin { public partial class shows : System.Web.UI.Page { protected void Page_Load(object sender, EventArgs e) { if (Session["CurrentUser"] == null) Response.Redirect("~/components/Login.aspx"); } } }
