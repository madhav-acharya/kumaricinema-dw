<%@ Page Title="User Tickets" Language="C#" MasterPageFile="~/pages/Admin.Master" AutoEventWireup="true" %>
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
            periodStartInput.Text = DateTime.Today.AddMonths(-6).ToString("yyyy-MM-dd");
            LoadUsers(currentUser);
            LoadReport(currentUser);
        }
    }

    private void LoadUsers(KumariCinema.Models.AppUser currentUser)
    {
        var appUserRepository = new KumariCinema.Repositories.AppUserRepository();
        var users = appUserRepository.GetAll();

        if (!_authorizationService.IsSuperAdmin(currentUser))
        {
            var bookingRepository = new KumariCinema.Repositories.BookingRepository();
            var allowedUserIds = new System.Collections.Generic.HashSet<string>(bookingRepository.GetByTheaterId(currentUser.TheaterId).Select(b => b.UserId));
            users = users.Where(u => allowedUserIds.Contains(u.UserId)).ToList();
        }

        userDropdown.DataSource = users.OrderBy(u => u.Name).ToList();
        userDropdown.DataTextField = "Name";
        userDropdown.DataValueField = "UserId";
        userDropdown.DataBind();
    }

    protected void LoadReport_Click(object sender, EventArgs e)
    {
        var currentUser = (KumariCinema.Models.AppUser)Session["CurrentUser"];
        LoadReport(currentUser);
    }

    private void LoadReport(KumariCinema.Models.AppUser currentUser)
    {
        if (userDropdown.Items.Count == 0)
        {
            reportRepeater.DataSource = new System.Collections.Generic.List<KumariCinema.Models.UserTicketReportRow>();
            reportRepeater.DataBind();
            userInfoCard.Visible = false;
            return;
        }

        var selectedUserId = userDropdown.SelectedValue;
        DateTime periodStart;
        if (!DateTime.TryParse(periodStartInput.Text, out periodStart))
        {
            periodStart = DateTime.Today.AddMonths(-6);
        }

        var reportsRepository = new KumariCinema.Repositories.ReportsRepository();
        var rows = reportsRepository.GetUserTicketsForSixMonths(
            selectedUserId,
            periodStart,
            _authorizationService.IsSuperAdmin(currentUser) ? null : currentUser.TheaterId
        );

        reportRepeater.DataSource = rows;
        reportRepeater.DataBind();

        var appUserRepository = new KumariCinema.Repositories.AppUserRepository();
        var selectedUser = appUserRepository.GetById(selectedUserId);
        userInfoCard.Visible = selectedUser != null;
        selectedUserNameLabel.Text = selectedUser == null ? "" : selectedUser.Name;
        selectedUserEmailLabel.Text = selectedUser == null ? "" : selectedUser.Email;
        selectedPeriodLabel.Text = periodStart.ToString("yyyy-MM-dd") + " to " + periodStart.AddMonths(6).ToString("yyyy-MM-dd");
    }
</script>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-title d-flex justify-content-between align-items-center">
        <h2><i class="fas fa-receipt"></i> User Ticket (6 Months)</h2>
    </div>

    <div class="card mb-3">
        <div class="card-body row g-3 align-items-end">
            <div class="col-md-5">
                <label class="form-label">User</label>
                <asp:DropDownList ID="userDropdown" runat="server" CssClass="form-select"></asp:DropDownList>
            </div>
            <div class="col-md-4">
                <label class="form-label">Period Start</label>
                <asp:TextBox ID="periodStartInput" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
            <div class="col-md-3">
                <asp:Button ID="loadButton" runat="server" Text="Load Report" CssClass="btn btn-primary-custom w-100" OnClick="LoadReport_Click" />
            </div>
        </div>
    </div>

    <div class="card mb-3" runat="server" id="userInfoCard" visible="false">
        <div class="card-body">
            <div><strong>User:</strong> <asp:Label ID="selectedUserNameLabel" runat="server"></asp:Label></div>
            <div><strong>Email:</strong> <asp:Label ID="selectedUserEmailLabel" runat="server"></asp:Label></div>
            <div><strong>Period:</strong> <asp:Label ID="selectedPeriodLabel" runat="server"></asp:Label></div>
        </div>
    </div>

    <div class="card">
        <div class="table-responsive">
            <table class="table table-hover mb-0">
                <thead>
                    <tr>
                        <th>Booking</th>
                        <th>Ticket</th>
                        <th>Movie</th>
                        <th>Theater</th>
                        <th>Hall</th>
                        <th>Seat</th>
                        <th>Show Time</th>
                        <th>Price</th>
                        <th>Payment</th>
                        <th>Booking Date</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="reportRepeater" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("BookingId") %></td>
                                <td><%# Eval("TicketId") %></td>
                                <td><%# Eval("MovieName") %></td>
                                <td><%# Eval("TheaterName") %> - <%# Eval("TheaterCity") %></td>
                                <td><%# Eval("HallName") %></td>
                                <td><%# Eval("SeatNumber") %></td>
                                <td><%# Convert.ToDateTime(Eval("ShowTime")).ToString("yyyy-MM-dd HH:mm") %></td>
                                <td><%# Eval("TicketPrice", "{0:N2}") %></td>
                                <td><%# Eval("PaymentStatus") %></td>
                                <td><%# Convert.ToDateTime(Eval("BookingDate")).ToString("yyyy-MM-dd") %></td>
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
        setActiveLink('userTicketsReportLink');
    </script>
</asp:Content>
