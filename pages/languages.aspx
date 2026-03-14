<%@ Page Title="Languages" Language="C#" MasterPageFile="~/pages/Admin.Master" AutoEventWireup="true" CodeFile="languages.aspx.cs" Inherits="KumariCinema.Admin.languages" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-title d-flex justify-content-between align-items-center mb-4">
        <h2><i class="fas fa-language"></i> Languages</h2>
        <button type="button" class="btn btn-custom btn-primary-custom" data-bs-toggle="modal" data-bs-target="#addModal">
            <i class="fas fa-plus"></i> Add Language
        </button>
    </div>

    <div class="card">
        <div class="table-responsive">
            <table class="table table-hover">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Name</th>
                        <th>Code</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="repeater" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("LanguageId") %></td>
                                <td><%# Eval("Name") %></td>
                                <td><%# Eval("Code") %></td>
                                <td>
                                    <button class="btn btn-sm btn-warning" data-bs-toggle="modal" data-bs-target="#editModal" onclick="edit('<%# Eval("LanguageId") %>', '<%# Eval("Name") %>', '<%# Eval("Code") %>')">
                                        <i class="fas fa-edit"></i> Edit
                                    </button>
                                    <button class="btn btn-sm btn-danger" onclick="del('<%# Eval("LanguageId") %>')">
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

    <div class="modal fade" id="addModal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Language</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label>Language ID</label>
                        <asp:TextBox ID="idInput" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label>Name</label>
                        <asp:TextBox ID="nameInput" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label>Code</label>
                        <asp:TextBox ID="codeInput" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <asp:Button ID="saveBtn" runat="server" Text="Save" CssClass="btn btn-primary-custom" OnClick="Save_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="editModal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Edit Language</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="editIdField" runat="server" />
                    <div class="mb-3">
                        <label>Language ID</label>
                        <asp:TextBox ID="editIdInput" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label>Name</label>
                        <asp:TextBox ID="editNameInput" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label>Code</label>
                        <asp:TextBox ID="editCodeInput" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <asp:Button ID="updateBtn" runat="server" Text="Update" CssClass="btn btn-primary-custom" OnClick="Update_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        function edit(id, name, code) {
            document.getElementById('<%= editIdInput.ClientID %>').value = id;
            document.getElementById('<%= editNameInput.ClientID %>').value = name;
            document.getElementById('<%= editCodeInput.ClientID %>').value = code;
            document.getElementById('<%= editIdField.ClientID %>').value = id;
        }
        function del(id) {
            if (confirm('Delete?')) {
                const f = document.getElementById('<%= this.ClientID %>');
                const d = document.createElement('input');
                d.type = 'hidden'; d.name = 'delId'; d.value = id;
                f.appendChild(d); f.submit();
            }
        }
        setActiveLink('languagesLink');
    </script>
</asp:Content>
