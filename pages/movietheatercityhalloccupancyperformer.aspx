<%@ Page Title="Movie Theater City Hall Occupancy Performer" Language="C#" MasterPageFile="~/pages/Admin.Master" AutoEventWireup="true" %>
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
            LoadMovies();
            LoadReport(currentUser);
        }
    }

    private void LoadMovies()
    {
        var movieRepository = new KumariCinema.Repositories.MovieRepository();
        var movies = movieRepository.GetAll().OrderBy(m => m.Name).ToList();

        movieDropdown.DataSource = movies;
        movieDropdown.DataTextField = "Name";
        movieDropdown.DataValueField = "MovieId";
        movieDropdown.DataBind();
    }

    protected void LoadReport_Click(object sender, EventArgs e)
    {
        var currentUser = (KumariCinema.Models.AppUser)Session["CurrentUser"];
        LoadReport(currentUser);
    }

    private void LoadReport(KumariCinema.Models.AppUser currentUser)
    {
        if (movieDropdown.Items.Count == 0)
        {
            reportRepeater.DataSource = new System.Collections.Generic.List<KumariCinema.Models.MovieTheaterCityHallOccupancyRow>();
            reportRepeater.DataBind();
            return;
        }

        var reportRepository = new KumariCinema.Repositories.ReportsRepository();
        var rows = reportRepository.GetTopOccupancyPerformers(
            movieDropdown.SelectedValue,
            _authorizationService.IsSuperAdmin(currentUser) ? null : currentUser.TheaterId
        );

        reportRepeater.DataSource = rows;
        reportRepeater.DataBind();
    }
</script>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-title d-flex justify-content-between align-items-center">
        <h2><i class="fas fa-chart-bar"></i> MovieTheaterCityHallOccupancyPerformer</h2>
    </div>

    <div class="card mb-3">
        <div class="card-body row g-3 align-items-end">
            <div class="col-md-9">
                <label class="form-label">Movie</label>
                <asp:DropDownList ID="movieDropdown" runat="server" CssClass="form-select"></asp:DropDownList>
            </div>
            <div class="col-md-3">
                <asp:Button ID="loadButton" runat="server" Text="Load Top 3" CssClass="btn btn-primary-custom w-100" OnClick="LoadReport_Click" />
            </div>
        </div>
    </div>

    <div class="card">
        <div class="table-responsive">
            <table class="table table-hover mb-0">
                <thead>
                    <tr>
                        <th>Movie</th>
                        <th>Theater</th>
                        <th>City</th>
                        <th>Hall</th>
                        <th>Capacity</th>
                        <th>Shows</th>
                        <th>Paid Tickets</th>
                        <th>Occupancy %</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="reportRepeater" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("MovieName") %></td>
                                <td><%# Eval("TheaterName") %></td>
                                <td><%# Eval("TheaterCity") %></td>
                                <td><%# Eval("HallName") %></td>
                                <td><%# Eval("HallCapacity") %></td>
                                <td><%# Eval("ShowCount") %></td>
                                <td><%# Eval("PaidTickets") %></td>
                                <td><%# Eval("OccupancyPercentage", "{0:N2}") %></td>
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
        setActiveLink('occupancyPerformerReportLink');
    </script>
</asp:Content>
