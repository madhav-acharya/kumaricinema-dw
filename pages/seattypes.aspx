<%@ Page Title="Seat Types" Language="C#" MasterPageFile="~/pages/Admin.Master" AutoEventWireup="true" CodeFile="seattypes.aspx.cs" Inherits="KumariCinema.Admin.seattypes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="modalStateField" runat="server" />

    <div class="page-title d-flex justify-content-between align-items-center mb-4">
        <h2><i class="fas fa-chair"></i> Seat Types</h2>
        <button type="button" class="btn btn-custom btn-primary-custom" data-bs-toggle="modal" data-bs-target="#addModal">
            <i class="fas fa-plus"></i> Add Seat Type
        </button>
    </div>
    <div class="card">
        <div class="table-responsive">
            <table class="table table-hover">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Name</th>
                        <th>Description</th>
                        <th>Price Multiplier</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="repeater" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("SeatTypeId") %></td>
                                <td><%# Eval("Name") %></td>
                                <td><%# Eval("Description") %></td>
                                <td><%# Eval("PriceMultiplier", "{0:N2}") %>x</td>
                                <td>
                                    <button class="btn btn-sm btn-warning" data-bs-toggle="modal" data-bs-target="#editModal" onclick="edit('<%# Eval("SeatTypeId") %>', '<%# Eval("Name") %>', '<%# Eval("Description") %>', <%# Eval("PriceMultiplier") %>)">
                                        <i class="fas fa-edit"></i>
                                    </button>
                                    <button class="btn btn-sm btn-danger" onclick="del('<%# Eval("SeatTypeId") %>')">
                                        <i class="fas fa-trash"></i>
                                    </button>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </div>

    <div class="modal fade" id="addModal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Seat Type</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3" style="display:none;">
                        <label class="form-label">Seat Type ID</label>
                        <asp:TextBox ID="idInput" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label>Name</label>
                        <asp:TextBox ID="nameInput" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label>Description</label>
                        <asp:TextBox ID="descInput" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label>Price Multiplier</label>
                        <asp:TextBox ID="priceInput" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <asp:Button ID="saveBtn" runat="server" Text="Save" CssClass="btn btn-primary-custom" OnClick="Save_Click" OnClientClick="setModalState('add'); return true;" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="editModal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Edit Seat Type</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="editIdField" runat="server" />
                    <div class="mb-3" style="display:none;">
                        <label class="form-label">Seat Type ID</label>
                        <asp:TextBox ID="editIdInput" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label>Name</label>
                        <asp:TextBox ID="editNameInput" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label>Description</label>
                        <asp:TextBox ID="editDescInput" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label>Price Multiplier</label>
                        <asp:TextBox ID="editPriceInput" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <asp:Button ID="updateBtn" runat="server" Text="Update" CssClass="btn btn-primary-custom" OnClick="Update_Click" OnClientClick="setModalState('edit'); return true;" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        function edit(id, name, desc, price) {
            document.getElementById('<%= editIdInput.ClientID %>').value = id;
            document.getElementById('<%= editNameInput.ClientID %>').value = name;
            document.getElementById('<%= editDescInput.ClientID %>').value = desc;
            document.getElementById('<%= editPriceInput.ClientID %>').value = price;
            document.getElementById('<%= editIdField.ClientID %>').value = id;
        }

        function del(id) {
            showConfirm('Are you sure you want to delete this seat type?', function () {
                const f = document.getElementById('form1');
                const d = document.createElement('input');
                d.type = 'hidden';
                d.name = 'delId';
                d.value = id;
                f.appendChild(d);
                setTimeout(() => f.submit(), 100);
            });
        }

        function setModalState(modalName) {
            document.getElementById('<%= modalStateField.ClientID %>').value = modalName;
        }

        window.addEventListener('load', function() {
            const modalState = document.getElementById('<%= modalStateField.ClientID %>').value;
            if (modalState === 'add') {
                const addModal = new bootstrap.Modal(document.getElementById('addModal'));
                addModal.show();
            } else if (modalState === 'edit') {
                const editModal = new bootstrap.Modal(document.getElementById('editModal'));
                editModal.show();
            }
        });

        setActiveLink('seatTypesLink');
    </script>
</asp:Content>
