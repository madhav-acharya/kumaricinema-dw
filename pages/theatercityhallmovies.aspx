<%@ Page Title="Theater City Hall Movies" Language="C#" MasterPageFile="~/pages/Admin.Master" AutoEventWireup="true" %>
<%@ Import Namespace="System.Linq" %>

<script runat="server">
    private KumariCinema.Services.AuthorizationService _authorizationService;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["CurrentUser"] == null)
        {
            Response.Redirect("~/pages/Login.aspx");
            return;
        }

        _authorizationService = new KumariCinema.Services.AuthorizationService();
        var currentUser = (KumariCinema.Models.AppUser)Session["CurrentUser"];

        if (!_authorizationService.CanViewBookings(currentUser, currentUser.TheaterId))
        {
            Response.Redirect("~/pages/Login.aspx");
            return;
        }

        if (!IsPostBack)
        {
            LoadTheaters(currentUser);
            LoadHalls();
            LoadReport(currentUser);
        }
    }

    private void LoadTheaters(KumariCinema.Models.AppUser currentUser)
    {
        var theaterRepository = new KumariCinema.Repositories.TheaterRepository();
        var theaters = _authorizationService.IsSuperAdmin(currentUser)
            ? theaterRepository.GetAll()
            : theaterRepository.GetAll().Where(t => t.TheaterId == currentUser.TheaterId).ToList();

        theaterDropdown.DataSource = theaters;
        theaterDropdown.DataTextField = "Name";
        theaterDropdown.DataValueField = "TheaterId";
        theaterDropdown.DataBind();
    }

    private void LoadHalls()
    {
        var hallRepository = new KumariCinema.Repositories.HallRepository();
        var halls = hallRepository.GetByTheaterId(theaterDropdown.SelectedValue);

        hallDropdown.DataSource = halls;
        hallDropdown.DataTextField = "HallName";
        hallDropdown.DataValueField = "HallId";
        hallDropdown.DataBind();
        hallDropdown.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Halls", ""));
    }

    protected void TheaterDropdown_Changed(object sender, EventArgs e)
    {
        var currentUser = (KumariCinema.Models.AppUser)Session["CurrentUser"];
        LoadHalls();
        LoadReport(currentUser);
    }

    protected void LoadReport_Click(object sender, EventArgs e)
    {
        var currentUser = (KumariCinema.Models.AppUser)Session["CurrentUser"];
        LoadReport(currentUser);
    }

    private void LoadReport(KumariCinema.Models.AppUser currentUser)
    {
        if (theaterDropdown.Items.Count == 0)
        {
            reportRepeater.DataSource = new System.Collections.Generic.List<KumariCinema.Models.TheaterCityHallMovieRow>();
            reportRepeater.DataBind();
            return;
        }

        var theaterId = theaterDropdown.SelectedValue;
        if (!_authorizationService.IsSuperAdmin(currentUser) && theaterId != currentUser.TheaterId)
        {
            ClientScript.RegisterStartupScript(GetType(), "accessErr", "showToast('Access denied for selected theater', 'error');", true);
            return;
        }

        var reportRepository = new KumariCinema.Repositories.ReportsRepository();
        var rows = reportRepository.GetTheaterCityHallMovies(theaterId, hallDropdown.SelectedValue);
        reportRepeater.DataSource = rows;
        reportRepeater.DataBind();
    }
</script>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-title d-flex justify-content-between align-items-center">
        <h2><i class="fas fa-building"></i> TheaterCityHall Movie</h2>
    </div>

    <div class="card mb-3">
        <div class="card-body row g-3 align-items-end">
            <div class="col-md-5">
                <label class="form-label">Theater</label>
                <asp:DropDownList ID="theaterDropdown" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="TheaterDropdown_Changed"></asp:DropDownList>
            </div>
            <div class="col-md-4">
                <label class="form-label">Hall</label>
                <asp:DropDownList ID="hallDropdown" runat="server" CssClass="form-select"></asp:DropDownList>
            </div>
            <div class="col-md-3">
                <asp:Button ID="loadButton" runat="server" Text="Load Report" CssClass="btn btn-primary-custom w-100" OnClick="LoadReport_Click" />
            </div>
        </div>
    </div>

    <div class="card">
        <div class="table-responsive">
            <table class="table table-hover mb-0">
                <thead>
                    <tr>
                        <th>Theater</th>
                        <th>City</th>
                        <th>Hall</th>
                        <th>Movie</th>
                        <th>Start</th>
                        <th>End</th>
                        <th>Category</th>
                        <th>Base Price</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="reportRepeater" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("TheaterName") %></td>
                                <td><%# Eval("TheaterCity") %></td>
                                <td><%# Eval("HallName") %></td>
                                <td><%# Eval("MovieName") %></td>
                                <td><%# Convert.ToDateTime(Eval("StartTime")).ToString("yyyy-MM-dd HH:mm") %></td>
                                <td><%# Convert.ToDateTime(Eval("EndTime")).ToString("yyyy-MM-dd HH:mm") %></td>
                                <td><%# Eval("ShowCategory") %></td>
                                <td><%# Eval("BaseTicketPrice", "{0:N2}") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        setActiveLink('theaterCityHallMovieReportLink');
    </script>
</asp:Content>
