<%@ Page Title="Users" Language="C#" MasterPageFile="~/pages/Admin.Master" AutoEventWireup="true" CodeFile="users.aspx.cs" Inherits="KumariCinema.Admin.users" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-title d-flex justify-content-between align-items-center mb-4">
        <h2><i class="fas fa-users"></i> Users</h2>
        <button type="button" class="btn btn-custom btn-primary-custom" data-bs-toggle="modal" data-bs-target="#addModal">
            <i class="fas fa-plus"></i> Add User
        </button>
    </div>

    <div class="search-box">
        <input type="text" id="searchInput" class="form-control" placeholder="Search users...">
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
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="repeater" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("UserId") %></td>
                                <td><%# Eval("Name") %></td>
                                <td><%# Eval("Email") %></td>
                                <td><span class="badge bg-primary"><%# Eval("Role") %></span></td>
                                <td><%# Eval("TheaterId") %></td>
                                <td>
                                    <button class="btn btn-sm btn-warning">Edit</button>
                                    <button class="btn btn-sm btn-danger">Delete</button>
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
                    <h5>Add User</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <p>User form implementation</p>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        document.getElementById('searchInput').addEventListener('keyup', function () {
            const val = this.value.toLowerCase();
            document.querySelectorAll('table tbody tr').forEach(row => {
                row.style.display = row.textContent.toLowerCase().includes(val) ? '' : 'none';
            });
        });
        setActiveLink('usersLink');
    </script>
</asp:Content>
