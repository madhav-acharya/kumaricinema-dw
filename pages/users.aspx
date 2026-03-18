<%@ Page Title="Users" Language="C#" MasterPageFile="~/pages/Admin.Master" AutoEventWireup="true" CodeFile="users.aspx.cs" Inherits="KumariCinema.Admin.users" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-title d-flex justify-content-between align-items-center mb-4">
        <h2><i class="fas fa-users"></i> Users</h2>
        <button type="button" class="btn btn-custom btn-primary-custom" data-bs-toggle="modal" data-bs-target="#addModal">
            <i class="fas fa-plus"></i> Add User
        </button>
    </div>
    <div class="card mb-3">
        <div class="card-body row g-2">
            <div class="col-md-8">
                <input type="text" id="searchInput" class="form-control" placeholder="Search users..." />
            </div>
            <div class="col-md-4">
                <select id="filterDropdown" class="form-select">
                    <option value="">All Roles</option>
                    <option value="admin">Admin</option>
                    <option value="owner">Owner</option>
                    <option value="staff">Staff</option>
                    <option value="customer">Customer</option>
                </select>
            </div>
        </div>
    </div>
    <div class="card">
        <div class="table-responsive">
            <table class="table table-hover">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Name</th>
                        <th>Email</th>
                        <th>Role</th>
                        <th>Theater</th>
                        <th class="text-end">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="repeater" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("UserId") %></td>
                                <td><%# Eval("Name") %></td>
                                <td><%# Eval("Email") %></td>
                                <td><span class="badge bg-success"><%# Eval("Role") %></span></td>
                                <td><%# Eval("TheaterId") %></td>
                                <td class="text-end">
                                    <button type="button" class="btn btn-sm btn-warning me-1" data-bs-toggle="modal" data-bs-target="#editModal" onclick="editUser('<%# Eval("UserId") %>','<%# Eval("Name") %>','<%# Eval("Email") %>','<%# Eval("Password") %>','<%# Eval("Role") %>','<%# Eval("TheaterId") %>')">Edit</button>
                                    <button type="button" class="btn btn-sm btn-danger" onclick="deleteUser('<%# Eval("UserId") %>')">Delete</button>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </div>

    <div class="modal fade" id="addModal" tabindex="-1">
        <div class="modal-dialog"><div class="modal-content"><div class="modal-header"><h5>Add User</h5><button type="button" class="btn-close" data-bs-dismiss="modal"></button></div><div class="modal-body">
            <div class="mb-3"><label class="form-label">Name</label><asp:TextBox ID="nameInput" runat="server" CssClass="form-control"></asp:TextBox></div>
            <div class="mb-3"><label class="form-label">Email</label><asp:TextBox ID="emailInput" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox></div>
            <div class="mb-3"><label class="form-label">Password</label><asp:TextBox ID="passwordInput" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox></div>
            <div class="mb-3"><label class="form-label">Role</label><asp:DropDownList ID="roleDropdown" runat="server" CssClass="form-select"><asp:ListItem Value="admin">Admin</asp:ListItem><asp:ListItem Value="owner">Owner</asp:ListItem><asp:ListItem Value="staff">Staff</asp:ListItem><asp:ListItem Value="customer">Customer</asp:ListItem></asp:DropDownList></div>
            <div class="mb-3"><label class="form-label">Theater</label><asp:DropDownList ID="theaterDropdown" runat="server" CssClass="form-select"></asp:DropDownList></div>
        </div><div class="modal-footer"><button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button><asp:Button ID="saveButton" runat="server" Text="Save User" CssClass="btn btn-primary-custom" OnClick="SaveUser_Click" /></div></div></div>
    </div>

    <div class="modal fade" id="editModal" tabindex="-1">
        <div class="modal-dialog"><div class="modal-content"><div class="modal-header"><h5>Edit User</h5><button type="button" class="btn-close" data-bs-dismiss="modal"></button></div><div class="modal-body">
            <asp:HiddenField ID="editUserIdField" runat="server" />
            <div class="mb-3"><label class="form-label">Name</label><asp:TextBox ID="editNameInput" runat="server" CssClass="form-control"></asp:TextBox></div>
            <div class="mb-3"><label class="form-label">Email</label><asp:TextBox ID="editEmailInput" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox></div>
            <div class="mb-3"><label class="form-label">Password</label><asp:TextBox ID="editPasswordInput" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox></div>
            <div class="mb-3"><label class="form-label">Role</label><asp:DropDownList ID="editRoleDropdown" runat="server" CssClass="form-select"><asp:ListItem Value="admin">Admin</asp:ListItem><asp:ListItem Value="owner">Owner</asp:ListItem><asp:ListItem Value="staff">Staff</asp:ListItem><asp:ListItem Value="customer">Customer</asp:ListItem></asp:DropDownList></div>
            <div class="mb-3"><label class="form-label">Theater</label><asp:DropDownList ID="editTheaterDropdown" runat="server" CssClass="form-select"></asp:DropDownList></div>
        </div><div class="modal-footer"><button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button><asp:Button ID="updateButton" runat="server" Text="Update User" CssClass="btn btn-primary-custom" OnClick="UpdateUser_Click" /></div></div></div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        function editUser(id, name, email, password, role, theaterId) {
            document.getElementById('<%= editUserIdField.ClientID %>').value = id;
            document.getElementById('<%= editNameInput.ClientID %>').value = name;
            document.getElementById('<%= editEmailInput.ClientID %>').value = email;
            document.getElementById('<%= editPasswordInput.ClientID %>').value = password;
            document.getElementById('<%= editRoleDropdown.ClientID %>').value = role;
            document.getElementById('<%= editTheaterDropdown.ClientID %>').value = theaterId;
        }
        function deleteUser(id) {
            showConfirm('Are you sure you want to delete this user?', function () {
                const f = document.getElementById('form1');
                const d = document.createElement('input');
                d.type = 'hidden';
                d.name = 'deleteUserId';
                d.value = id;
                f.appendChild(d);
                f.submit();
            });
        }
        document.getElementById('searchInput').addEventListener('keyup', function () {
            applyFilter();
        });
        document.getElementById('filterDropdown').addEventListener('change', function () {
            applyFilter();
        });

        function applyFilter() {
            const val = document.getElementById('searchInput').value.toLowerCase();
            const role = document.getElementById('filterDropdown').value.toLowerCase();
            document.querySelectorAll('table tbody tr').forEach(row => {
                const text = row.textContent.toLowerCase();
                const rowRole = row.children[3] ? row.children[3].textContent.toLowerCase().trim() : '';
                const matchesSearch = text.includes(val);
                const matchesRole = role === '' || rowRole === role;
                row.style.display = matchesSearch && matchesRole ? '' : 'none';
            });
        }
        setActiveLink('usersLink');
    </script>
</asp:Content>