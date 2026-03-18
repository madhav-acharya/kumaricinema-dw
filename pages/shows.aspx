<%@ Page Title="Shows" Language="C#" MasterPageFile="~/pages/Admin.Master" AutoEventWireup="true" CodeFile="shows.aspx.cs" Inherits="KumariCinema.Admin.shows" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="modalStateField" runat="server" />

    <div class="page-title d-flex justify-content-between align-items-center">
        <h2><i class="fas fa-clock"></i> Shows</h2>
        <button type="button" class="btn btn-primary-custom" data-bs-toggle="modal" data-bs-target="#addShowModal">
            <i class="fas fa-plus"></i> Add Show
        </button>
    </div>
    <div class="card mb-3">
        <div class="card-body row g-2">
            <div class="col-md-8">
                <input type="text" id="searchInput" class="form-control" placeholder="Search shows..." />
            </div>
            <div class="col-md-4">
                <select id="filterDropdown" class="form-select">
                    <option value="">All Categories</option>
                    <option value="morning">Morning</option>
                    <option value="afternoon">Afternoon</option>
                    <option value="evening">Evening</option>
                    <option value="night">Night</option>
                </select>
            </div>
        </div>
    </div>
    <div class="card">
        <div class="table-responsive">
            <table class="table table-hover mb-0">
                <thead>
                    <tr>
                        <th>Show ID</th>
                        <th>Movie</th>
                        <th>Hall</th>
                        <th>Start</th>
                        <th>End</th>
                        <th>Category</th>
                        <th>Price</th>
                        <th class="text-end">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="showsRepeater" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("ShowId") %></td>
                                <td><%# Eval("MovieName") %></td>
                                <td><%# Eval("HallName") %></td>
                                <td><%# Convert.ToDateTime(Eval("StartTime")).ToString("yyyy-MM-dd HH:mm") %></td>
                                <td><%# Convert.ToDateTime(Eval("EndTime")).ToString("yyyy-MM-dd HH:mm") %></td>
                                <td><%# Eval("ShowCategory") %></td>
                                <td><%# Eval("BaseTicketPrice") %></td>
                                <td class="text-end">
                                    <button type="button" class="btn btn-sm btn-warning me-1" data-bs-toggle="modal" data-bs-target="#editShowModal"
                                        data-id='<%# Eval("ShowId") %>'
                                        data-movie='<%# Eval("MovieId") %>'
                                        data-hall='<%# Eval("HallId") %>'
                                        data-start='<%# Convert.ToDateTime(Eval("StartTime")).ToString("yyyy-MM-ddTHH:mm") %>'
                                        data-end='<%# Convert.ToDateTime(Eval("EndTime")).ToString("yyyy-MM-ddTHH:mm") %>'
                                        data-category='<%# Eval("ShowCategory") %>'
                                        data-price='<%# Eval("BaseTicketPrice") %>'
                                        onclick="editShow(this)">
                                        <i class="fas fa-edit"></i> Edit
                                    </button>
                                    <button type="button" class="btn btn-sm btn-danger" onclick="deleteShow('<%# Eval("ShowId") %>')">
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

    <div class="modal fade" id="addShowModal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Show</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label">Movie</label>
                        <asp:DropDownList ID="movieDropdown" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Hall</label>
                        <asp:DropDownList ID="hallDropdown" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Start Time</label>
                        <asp:TextBox ID="startTimeInput" runat="server" CssClass="form-control" TextMode="DateTimeLocal"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">End Time</label>
                        <asp:TextBox ID="endTimeInput" runat="server" CssClass="form-control" TextMode="DateTimeLocal"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Category</label>
                        <asp:DropDownList ID="categoryDropdown" runat="server" CssClass="form-select">
                            <asp:ListItem Value="Morning">Morning</asp:ListItem>
                            <asp:ListItem Value="Afternoon">Afternoon</asp:ListItem>
                            <asp:ListItem Value="Evening">Evening</asp:ListItem>
                            <asp:ListItem Value="Night">Night</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Base Ticket Price</label>
                        <asp:TextBox ID="priceInput" runat="server" CssClass="form-control" TextMode="Number" placeholder="500"></asp:TextBox>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <asp:Button ID="saveShowButton" runat="server" Text="Save Show" CssClass="btn btn-primary-custom" OnClick="SaveShow_Click" OnClientClick="setModalState('add'); return true;" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="editShowModal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Edit Show</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="editShowIdField" runat="server" />
                    <div class="mb-3" style="display:none;">
                        <label class="form-label">Show ID</label>
                        <asp:TextBox ID="editShowIdInput" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Movie</label>
                        <asp:DropDownList ID="editMovieDropdown" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Hall</label>
                        <asp:DropDownList ID="editHallDropdown" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Start Time</label>
                        <asp:TextBox ID="editStartTimeInput" runat="server" CssClass="form-control" TextMode="DateTimeLocal"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">End Time</label>
                        <asp:TextBox ID="editEndTimeInput" runat="server" CssClass="form-control" TextMode="DateTimeLocal"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Category</label>
                        <asp:DropDownList ID="editCategoryDropdown" runat="server" CssClass="form-select">
                            <asp:ListItem Value="Morning">Morning</asp:ListItem>
                            <asp:ListItem Value="Afternoon">Afternoon</asp:ListItem>
                            <asp:ListItem Value="Evening">Evening</asp:ListItem>
                            <asp:ListItem Value="Night">Night</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Base Ticket Price</label>
                        <asp:TextBox ID="editPriceInput" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <asp:Button ID="updateShowButton" runat="server" Text="Update Show" CssClass="btn btn-primary-custom" OnClick="UpdateShow_Click" OnClientClick="setModalState('edit'); return true;" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        function editShow(btn) {
            const d = btn.dataset;
            document.getElementById('<%= editShowIdInput.ClientID %>').value = d.id;
            document.getElementById('<%= editShowIdField.ClientID %>').value = d.id;
            document.getElementById('<%= editMovieDropdown.ClientID %>').value = d.movie;
            document.getElementById('<%= editHallDropdown.ClientID %>').value = d.hall;
            document.getElementById('<%= editStartTimeInput.ClientID %>').value = d.start;
            document.getElementById('<%= editEndTimeInput.ClientID %>').value = d.end;
            document.getElementById('<%= editCategoryDropdown.ClientID %>').value = d.category;
            document.getElementById('<%= editPriceInput.ClientID %>').value = d.price;
        }

        function deleteShow(showId) {
            showConfirm('Are you sure you want to delete this show? This will also delete all associated bookings and tickets.', function () {
                const form = document.getElementById('form1');
                const deleteField = document.createElement('input');
                deleteField.type = 'hidden';
                deleteField.name = 'deleteShowId';
                deleteField.value = showId;
                form.appendChild(deleteField);
                form.submit();
            });
        }

        function setModalState(modalName) {
            document.getElementById('<%= modalStateField.ClientID %>').value = modalName;
        }

        window.addEventListener('load', function() {
            const modalState = document.getElementById('<%= modalStateField.ClientID %>').value;
            if (modalState === 'add') {
                const addModal = new bootstrap.Modal(document.getElementById('addShowModal'));
                addModal.show();
            } else if (modalState === 'edit') {
                const editModal = new bootstrap.Modal(document.getElementById('editShowModal'));
                editModal.show();
            }
        });

        document.getElementById('searchInput').addEventListener('keyup', function () {
            applyFilter();
        });
        document.getElementById('filterDropdown').addEventListener('change', function () {
            applyFilter();
        });

        function applyFilter() {
            const searchValue = document.getElementById('searchInput').value.toLowerCase();
            const category = document.getElementById('filterDropdown').value.toLowerCase();
            document.querySelectorAll('table tbody tr').forEach(row => {
                const text = row.textContent.toLowerCase();
                const rowCategory = row.children[5] ? row.children[5].textContent.toLowerCase().trim() : '';
                const matchesSearch = text.includes(searchValue);
                const matchesCategory = category === '' || rowCategory === category;
                row.style.display = matchesSearch && matchesCategory ? '' : 'none';
            });
        }

        setActiveLink('showsLink');
    </script>
</asp:Content>
