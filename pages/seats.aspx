<%@ Page Title="Seats" Language="C#" MasterPageFile="~/pages/Admin.Master" AutoEventWireup="true" CodeFile="seats.aspx.cs" Inherits="KumariCinema.Admin.seats" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="modalStateField" runat="server" />

    <div class="page-title d-flex justify-content-between align-items-center">
        <h2><i class="fas fa-couch"></i> Seats</h2>
        <button type="button" class="btn btn-primary-custom" data-bs-toggle="modal" data-bs-target="#addSeatModal">
            <i class="fas fa-plus"></i> Add Seat
        </button>
    </div>
    <div class="card mb-3">
        <div class="card-body">
            <input type="text" id="searchInput" class="form-control" placeholder="Search seats..." />
        </div>
    </div>
    <div class="card">
        <div class="table-responsive">
            <table class="table table-hover mb-0">
                <thead>
                    <tr>
                        <th>Seat ID</th>
                        <th>Seat Number</th>
                        <th>Status</th>
                        <th>Seat Type</th>
                        <th class="text-end">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="seatsRepeater" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("SeatId") %></td>
                                <td><%# Eval("SeatNumber") %></td>
                                <td><%# Eval("Status") %></td>
                                <td><%# Eval("SeatTypeId") %></td>
                                <td class="text-end">
                                    <button type="button" class="btn btn-sm btn-warning me-1" data-bs-toggle="modal" data-bs-target="#editSeatModal"
                                        data-id='<%# Eval("SeatId") %>'
                                        data-number='<%# System.Web.HttpUtility.HtmlAttributeEncode(Eval("SeatNumber").ToString()) %>'
                                        data-status='<%# Eval("Status") %>'
                                        data-seattype='<%# Eval("SeatTypeId") %>'
                                        onclick="editSeat(this)">
                                        <i class="fas fa-edit"></i> Edit
                                    </button>
                                    <button type="button" class="btn btn-sm btn-danger" onclick="deleteSeat('<%# Eval("SeatId") %>')">
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

    <div class="modal fade" id="addSeatModal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Seat</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3" style="display:none;">
                        <label class="form-label">Seat ID</label>
                        <asp:TextBox ID="seatIdInput" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Seat Number</label>
                        <asp:TextBox ID="seatNumberInput" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Status</label>
                        <asp:DropDownList ID="statusDropdown" runat="server" CssClass="form-select">
                            <asp:ListItem Value="available">Available</asp:ListItem>
                            <asp:ListItem Value="reserved">Reserved</asp:ListItem>
                            <asp:ListItem Value="blocked">Blocked</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Seat Type</label>
                        <asp:DropDownList ID="seatTypeDropdown" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <asp:Button ID="saveSeatButton" runat="server" Text="Save Seat" CssClass="btn btn-primary-custom" OnClick="SaveSeat_Click" OnClientClick="setModalState('add'); return true;" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="editSeatModal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Edit Seat</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="editSeatIdField" runat="server" />
                    <div class="mb-3" style="display:none;">
                        <label class="form-label">Seat ID</label>
                        <asp:TextBox ID="editSeatIdInput" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Seat Number</label>
                        <asp:TextBox ID="editSeatNumberInput" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Status</label>
                        <asp:DropDownList ID="editStatusDropdown" runat="server" CssClass="form-select">
                            <asp:ListItem Value="available">Available</asp:ListItem>
                            <asp:ListItem Value="reserved">Reserved</asp:ListItem>
                            <asp:ListItem Value="blocked">Blocked</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Seat Type</label>
                        <asp:DropDownList ID="editSeatTypeDropdown" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <asp:Button ID="updateSeatButton" runat="server" Text="Update Seat" CssClass="btn btn-primary-custom" OnClick="UpdateSeat_Click" OnClientClick="setModalState('edit'); return true;" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        function editSeat(btn) {
            const d = btn.dataset;
            document.getElementById('<%= editSeatIdInput.ClientID %>').value = d.id;
            document.getElementById('<%= editSeatIdField.ClientID %>').value = d.id;
            document.getElementById('<%= editSeatNumberInput.ClientID %>').value = d.number;
            document.getElementById('<%= editStatusDropdown.ClientID %>').value = d.status;
            document.getElementById('<%= editSeatTypeDropdown.ClientID %>').value = d.seattype;
        }

        function deleteSeat(id) {
            showConfirm('Are you sure you want to delete this seat?', function () {
                const form = document.getElementById('form1');
                const f = document.createElement('input');
                f.type = 'hidden';
                f.name = 'deleteSeatId';
                f.value = id;
                form.appendChild(f);
                setTimeout(() => form.submit(), 100);
            });
        }

        function setModalState(modalName) {
            document.getElementById('<%= modalStateField.ClientID %>').value = modalName;
        }

        window.addEventListener('load', function() {
            const modalState = document.getElementById('<%= modalStateField.ClientID %>').value;
            if (modalState === 'add') {
                const addModal = new bootstrap.Modal(document.getElementById('addSeatModal'));
                addModal.show();
            } else if (modalState === 'edit') {
                const editModal = new bootstrap.Modal(document.getElementById('editSeatModal'));
                editModal.show();
            }
        });

        document.getElementById('searchInput').addEventListener('keyup', function () {
            const v = this.value.toLowerCase();
            document.querySelectorAll('table tbody tr').forEach(r => r.style.display = r.textContent.toLowerCase().includes(v) ? '' : 'none');
        });

        setActiveLink('seatsLink');
    </script>
</asp:Content>
