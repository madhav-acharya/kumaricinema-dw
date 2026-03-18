<%@ Page Title="Tickets" Language="C#" MasterPageFile="~/pages/Admin.Master" AutoEventWireup="true" CodeFile="tickets.aspx.cs" Inherits="KumariCinema.Admin.tickets" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <div class="page-title d-flex justify-content-between align-items-center mb-4">
    <h2><i class="fas fa-receipt"></i> Tickets</h2>
    <button type="button" class="btn btn-primary-custom" data-bs-toggle="modal" data-bs-target="#addTicketModal">
      <i class="fas fa-plus"></i> Add Ticket
    </button>
  </div>
  <div class="card mb-3">
    <div class="card-body row g-2">
      <div class="col-md-8">
        <input type="text" id="searchInput" class="form-control" placeholder="Search tickets..." />
      </div>
      <div class="col-md-4">
        <select id="filterDropdown" class="form-select">
          <option value="">All Statuses</option>
          <option value="booked">Booked</option>
          <option value="cancelled">Cancelled</option>
          <option value="used">Used</option>
        </select>
      </div>
    </div>
  </div>
  <div class="card">
    <div class="table-responsive">
      <table class="table table-hover mb-0">
        <thead>
          <tr>
            <th>Ticket ID</th>
            <th>Seat</th>
            <th>Show</th>
            <th>Price</th>
            <th>Status</th>
            <th class="text-end">Actions</th>
          </tr>
        </thead>
        <tbody>
          <asp:Repeater ID="ticketsRepeater" runat="server">
            <ItemTemplate>
              <tr>
                <td><%# Eval("TicketId") %></td>
                <td><%# Eval("SeatId") %></td>
                <td><%# Eval("ShowId") %></td>
                <td><%# Eval("TicketPrice") %></td>
                <td><span class="badge bg-info"><%# Eval("TicketStatus") %></span></td>
                <td class="text-end">
                  <button type="button" class="btn btn-sm btn-warning me-1" data-bs-toggle="modal" data-bs-target="#editTicketModal"
                    onclick="editTicket('<%# Eval("TicketId") %>','<%# Eval("SeatId") %>','<%# Eval("ShowId") %>','<%# Eval("TicketPrice") %>','<%# Eval("TicketStatus") %>')">
                    <i class="fas fa-edit"></i> Edit
                  </button>
                  <button type="button" class="btn btn-sm btn-danger" onclick="deleteTicket('<%# Eval("TicketId") %>')">
                    <i class="fas fa-trash"></i> Delete
                  </button>
                </td>
              </tr>
            </ItemTemplate>
          </asp:Repeater>
        </tbody>
      </table>
    </div>
  </div>
  <div class="modal fade" id="addTicketModal" tabindex="-1">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Add Ticket</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
        </div>
        <div class="modal-body">
          <div class="mb-3">
            <label class="form-label">Show</label>
            <asp:DropDownList ID="showDropdown" runat="server" CssClass="form-select"></asp:DropDownList>
          </div>
          <div class="mb-3">
            <label class="form-label">Seat</label>
            <asp:DropDownList ID="seatDropdown" runat="server" CssClass="form-select"></asp:DropDownList>
          </div>
          <div class="mb-3">
            <label class="form-label">Ticket Price</label>
            <asp:TextBox ID="ticketPriceInput" runat="server" CssClass="form-control" TextMode="Number" placeholder="500"></asp:TextBox>
          </div>
          <div class="mb-3">
            <label class="form-label">Status</label>
            <asp:DropDownList ID="ticketStatusDropdown" runat="server" CssClass="form-select">
              <asp:ListItem Value="booked">Booked</asp:ListItem>
              <asp:ListItem Value="cancelled">Cancelled</asp:ListItem>
              <asp:ListItem Value="used">Used</asp:ListItem>
            </asp:DropDownList>
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
          <asp:Button ID="saveTicketButton" runat="server" Text="Save Ticket" CssClass="btn btn-primary-custom" OnClick="SaveTicket_Click" />
        </div>
      </div>
    </div>
  </div>
  <div class="modal fade" id="editTicketModal" tabindex="-1">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Edit Ticket</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
        </div>
        <div class="modal-body">
          <asp:HiddenField ID="editTicketIdField" runat="server" />
          <div class="mb-3">
            <label class="form-label">Show</label>
            <asp:DropDownList ID="editShowDropdown" runat="server" CssClass="form-select"></asp:DropDownList>
          </div>
          <div class="mb-3">
            <label class="form-label">Seat</label>
            <asp:DropDownList ID="editSeatDropdown" runat="server" CssClass="form-select"></asp:DropDownList>
          </div>
          <div class="mb-3">
            <label class="form-label">Ticket Price</label>
            <asp:TextBox ID="editTicketPriceInput" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
          </div>
          <div class="mb-3">
            <label class="form-label">Status</label>
            <asp:DropDownList ID="editTicketStatusDropdown" runat="server" CssClass="form-select">
              <asp:ListItem Value="booked">Booked</asp:ListItem>
              <asp:ListItem Value="cancelled">Cancelled</asp:ListItem>
              <asp:ListItem Value="used">Used</asp:ListItem>
            </asp:DropDownList>
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
          <asp:Button ID="updateTicketButton" runat="server" Text="Update Ticket" CssClass="btn btn-primary-custom" OnClick="UpdateTicket_Click" />
        </div>
      </div>
    </div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
  <script>
    function editTicket(ticketId, seatId, showId, price, status) {
      document.getElementById('<%= editTicketIdField.ClientID %>').value = ticketId;
      document.getElementById('<%= editShowDropdown.ClientID %>').value = showId;
      document.getElementById('<%= editSeatDropdown.ClientID %>').value = seatId;
      document.getElementById('<%= editTicketPriceInput.ClientID %>').value = price;
      document.getElementById('<%= editTicketStatusDropdown.ClientID %>').value = status;
    }
    function deleteTicket(ticketId) {
      showConfirm('', function () {
        const form = document.getElementById('form1');
        const deleteField = document.createElement('input');
        deleteField.type = 'hidden';
        deleteField.name = 'deleteTicketId';
        deleteField.value = ticketId;
        form.appendChild(deleteField);
        form.submit();
      });
    }
    document.getElementById('searchInput').addEventListener('keyup', function () {
      applyFilter();
    });
    document.getElementById('filterDropdown').addEventListener('change', function () {
      applyFilter();
    });

    function applyFilter() {
      const searchValue = document.getElementById('searchInput').value.toLowerCase();
      const status = document.getElementById('filterDropdown').value.toLowerCase();
      document.querySelectorAll('table tbody tr').forEach(row => {
        const text = row.textContent.toLowerCase();
        const rowStatus = row.children[4] ? row.children[4].textContent.toLowerCase().trim() : '';
        const matchesSearch = text.includes(searchValue);
        const matchesStatus = status === '' || rowStatus === status;
        row.style.display = matchesSearch && matchesStatus ? '' : 'none';
      });
    }
    setActiveLink('ticketsLink');
  </script>
</asp:Content>
