<%@ Page Title="Halls" Language="C#" MasterPageFile="~/pages/Admin.Master"
AutoEventWireup="true" CodeFile="halls.aspx.cs"
Inherits="KumariCinema.Admin.halls" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-title d-flex justify-content-between align-items-center">
        <h2><i class="fas fa-door-open"></i> Halls</h2>
        <button type="button" class="btn btn-primary-custom" data-bs-toggle="modal" data-bs-target="#addHallModal">
            <i class="fas fa-plus"></i> Add Hall
        </button>
    </div>

    <div class="card mb-3">
        <div class="card-body">
            <input type="text" id="searchInput" class="form-control" placeholder="Search halls..." />
        </div>
    </div>

    <div class="card">
        <div class="table-responsive">
            <table class="table table-hover mb-0">
                <thead>
                    <tr>
                        <th>Hall ID</th>
                        <th>Name</th>
                        <th>Capacity</th>
                        <th>Screen Type</th>
                        <th>Theater</th>
                        <th class="text-end">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="hallsRepeater" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("HallId") %></td>
                                <td><%# Eval("HallName") %></td>
                                <td><%# Eval("Capacity") %></td>
                                <td><%# Eval("ScreenType") %></td>
                                <td><%# Eval("TheaterId") %></td>
                                <td class="text-end">
                                    <button type="button" class="btn btn-sm btn-warning me-1" data-bs-toggle="modal" data-bs-target="#editHallModal"
                                        onclick="editHall('<%# Eval("HallId") %>','<%# Eval("HallName") %>','<%# Eval("Capacity") %>','<%# Eval("ScreenType") %>','<%# Eval("TheaterId") %>')">
                                        <i class="fas fa-edit"></i> Edit
                                    </button>
                                    <button type="button" class="btn btn-sm btn-danger" onclick="deleteHall('<%# Eval("HallId") %>')">
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

    <div class="modal fade" id="addHallModal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Hall</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label">Hall ID</label>
                        <asp:TextBox ID="hallIdInput" runat="server" CssClass="form-control" placeholder="HAL001"></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="hallIdInput" ErrorMessage="Hall ID is required" CssClass="text-danger" Display="Dynamic" runat="server"></asp:RequiredFieldValidator>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Hall Name</label>
                        <asp:TextBox ID="hallNameInput" runat="server" CssClass="form-control" placeholder="Hall 1"></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="hallNameInput" ErrorMessage="Hall name is required" CssClass="text-danger" Display="Dynamic" runat="server"></asp:RequiredFieldValidator>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Capacity</label>
                        <asp:TextBox ID="capacityInput" runat="server" CssClass="form-control" TextMode="Number" placeholder="120"></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="capacityInput" ErrorMessage="Capacity is required" CssClass="text-danger" Display="Dynamic" runat="server"></asp:RequiredFieldValidator>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Screen Type</label>
                        <asp:DropDownList ID="screenTypeDropdown" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">Select Screen Type</asp:ListItem>
                            <asp:ListItem Value="2D">2D</asp:ListItem>
                            <asp:ListItem Value="3D">3D</asp:ListItem>
                            <asp:ListItem Value="IMAX">IMAX</asp:ListItem>
                            <asp:ListItem Value="4DX">4DX</asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ControlToValidate="screenTypeDropdown" ErrorMessage="Screen type is required" CssClass="text-danger" Display="Dynamic" runat="server"></asp:RequiredFieldValidator>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Theater</label>
                        <asp:DropDownList ID="theaterDropdown" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <asp:Button ID="saveHallButton" runat="server" Text="Save Hall" CssClass="btn btn-primary-custom" OnClick="SaveHall_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="editHallModal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Edit Hall</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="editHallIdField" runat="server" />
                    <div class="mb-3">
                        <label class="form-label">Hall ID</label>
                        <asp:TextBox ID="editHallIdInput" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Hall Name</label>
                        <asp:TextBox ID="editHallNameInput" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Capacity</label>
                        <asp:TextBox ID="editCapacityInput" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Screen Type</label>
                        <asp:DropDownList ID="editScreenTypeDropdown" runat="server" CssClass="form-select">
                            <asp:ListItem Value="2D">2D</asp:ListItem>
                            <asp:ListItem Value="3D">3D</asp:ListItem>
                            <asp:ListItem Value="IMAX">IMAX</asp:ListItem>
                            <asp:ListItem Value="4DX">4DX</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Theater</label>
                        <asp:DropDownList ID="editTheaterDropdown" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <asp:Button ID="updateHallButton" runat="server" Text="Update Hall" CssClass="btn btn-primary-custom" OnClick="UpdateHall_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        function editHall(hallId, hallName, capacity, screenType, theaterId) {
            document.getElementById('<%= editHallIdInput.ClientID %>').value = hallId;
            document.getElementById('<%= editHallNameInput.ClientID %>').value = hallName;
            document.getElementById('<%= editCapacityInput.ClientID %>').value = capacity;
            document.getElementById('<%= editScreenTypeDropdown.ClientID %>').value = screenType;
            document.getElementById('<%= editTheaterDropdown.ClientID %>').value = theaterId;
            document.getElementById('<%= editHallIdField.ClientID %>').value = hallId;
        }

        function deleteHall(hallId) {
            if (confirm('Are you sure you want to delete this hall?')) {
                const form = document.getElementById('<%= this.ClientID %>');
                const deleteField = document.createElement('input');
                deleteField.type = 'hidden';
                deleteField.name = 'deleteHallId';
                deleteField.value = hallId;
                form.appendChild(deleteField);
                form.submit();
            }
        }

        document.getElementById('searchInput').addEventListener('keyup', function () {
            const searchValue = this.value.toLowerCase();
            document.querySelectorAll('table tbody tr').forEach(row => {
                const text = row.textContent.toLowerCase();
                row.style.display = text.includes(searchValue) ? '' : 'none';
            });
        });

        setActiveLink('hallsLink');
    </script>
</asp:Content>
