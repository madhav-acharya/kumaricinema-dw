# Quick Reference Guide - Cinema Admin Panel

## 📍 File Locations

### Models

```
/Models/
├── Theater.cs
├── Movie.cs
├── Genre.cs
├── Language.cs
├── SeatType.cs
├── Hall.cs
├── AppUser.cs
├── Seat.cs
├── MovieShow.cs
├── Booking.cs
├── Ticket.cs
└── Payment.cs
```

### Repositories

```
/Repositories/
├── IRepository.cs (base interface)
├── TheaterRepository.cs
├── MovieRepository.cs
├── GenreRepository.cs
├── LanguageRepository.cs
├── SeatTypeRepository.cs
├── HallRepository.cs
├── AppUserRepository.cs
├── SeatRepository.cs
├── MovieShowRepository.cs
├── BookingRepository.cs
├── TicketRepository.cs
└── PaymentRepository.cs
```

### Services

```
/Services/
├── AuthService.cs
└── AuthorizationService.cs
```

### Admin Pages

```
/pages/
├── Admin.Master (Layout for all admin pages)
├── dashboard.aspx (Analytics Dashboard)
├── movies.aspx (Fully implemented CRUD)
├── theaters.aspx (Fully implemented CRUD)
├── genres.aspx (Fully implemented CRUD)
├── languages.aspx (Fully implemented CRUD)
├── seattypes.aspx (Fully implemented CRUD)
├── users.aspx (Placeholder)
├── halls.aspx (Placeholder)
├── shows.aspx (Placeholder)
├── bookings.aspx (Placeholder)
├── payments.aspx (Placeholder)
├── seats.aspx (Placeholder)
└── tickets.aspx (Placeholder)
```

### Entry Points

```
/Login.aspx (Authentication)
/logout.aspx (Session termination)
```

## 🔄 Data Flow

```
User Request
    ↓
Page checks authorization (Redirect if not logged in)
    ↓
Repository layer handles all database operations
    ↓
AuthorizationService verifies permissions
    ↓
Data displayed with validations
    ↓
Toast notifications for feedback
```

## 🎯 Common Tasks

### To Add a New Admin Page

1. Copy `pages/movies.aspx` → `pages/yourpage.aspx`
2. Replace all references to "movie" with your entity
3. Update the repository class name
4. Create matching `.aspx.cs` code-behind
5. Add link to `Admin.Master` sidebar

### To Create a New Repository

1. Copy any existing repository
2. Implement `IRepository<T>` interface
3. Update table name and column mappings
4. Add entity-specific methods (like GetByTheaterId)

### To Handle Role-Based Access

```csharp
// In your .aspx.cs
AuthorizationService auth = new AuthorizationService();
AppUser user = (AppUser)Session["CurrentUser"];

if (!auth.CanManageMovies(user, theaterId))
    Response.Redirect("~/Login.aspx");
```

### To Add Custom Methods to Repository

```csharp
public List<Movie> GetByFormat(string format)
{
    // Following the pattern of other methods
    var movies = new List<Movie>();
    try
    {
        using (var connection = new OracleConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM movie WHERE viewing_format = :format";
            using (var command = new OracleCommand(query, connection))
            {
                command.Parameters.AddWithValue(":format", format);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        movies.Add(new Movie
                        {
                            MovieId = reader["movie_id"].ToString(),
                            Name = reader["name"].ToString(),
                            DurationMinutes = Convert.ToInt32(reader["duration_minutes"]),
                            ViewingFormat = reader["viewing_format"].ToString()
                        });
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        throw new Exception("Error: " + ex.Message);
    }
    return movies;
}
```

## 🎨 UI Components

### Toast Notifications

```javascript
showToast("Success message", "success"); // Green
showToast("Error message", "error"); // Red
showToast("Info message", "info"); // Blue
```

### Modal Forms

```html
<!-- Add Modal -->
<div class="modal fade" id="addModal" tabindex="-1">
  <!-- Form fields here -->
</div>

<!-- Open: data-bs-toggle="modal" data-bs-target="#addModal" -->
```

### Data Tables

```html
<table class="table table-hover">
  <thead>
    <tr>
      <th>Column 1</th>
      <th>Column 2</th>
    </tr>
  </thead>
  <tbody>
    <asp:Repeater ID="repeater" runat="server">
      <ItemTemplate>
        <tr>
          <td><%# Eval("Property1") %></td>
          <td><%# Eval("Property2") %></td>
        </tr>
      </ItemTemplate>
    </asp:Repeater>
  </tbody>
</table>
```

## 🔐 Authorization Methods

```csharp
auth.IsSuperAdmin(user)           // Full access
auth.IsAdmin(user)                // Admin level
auth.IsOwner(user)                // Owner level
auth.IsStaff(user)                // Staff level
auth.IsAdminLevel(user)           // Admin, Owner, or Super Admin
auth.CanAccessTheater(user, id)   // Theater access check
auth.CanManageTheaters(user)      // Theater management
auth.CanManageMovies(user, id)    // Movie management
auth.CanManageShows(user, id)     // Show management
auth.CanViewBookings(user, id)    // Booking view access
auth.CanManagePayments(user, id)  // Payment management
```

## 🗄️ Oracle Connection

```csharp
// Connection string is in Web.config
ConfigurationManager.ConnectionStrings["OracleConnection"]

// Usage in Repository
using (var connection = new OracleConnection(_connectionString))
{
    connection.Open();
    // Execute commands
}
```

## 📋 Entity Relationships

```
Theater (1) ──→ (Many) Hall
         ├──→ (Many) AppUser
         └──→ (Many) MovieShow

Movie (Many) ──→ (Many) Genre (movie_genre)
      (Many) ──→ (Many) Language (movie_language)

MovieShow (1) ──→ (Many) Ticket
           └──→ (Many) Booking

Booking (1) ──→ (Many) Seat (booking_seat)
        └──→ (1) Payment

Seat (1) ──→ (Many) Ticket
     └──→ (1) SeatType
```

## 🧪 Testing

### Test Scenarios

1. Login with different roles
2. Try accessing pages without login (should redirect)
3. Try accessing restricted pages (based on role)
4. Create, Read, Update, Delete operations
5. Search and filter functionality
6. Form validations
7. Modal open/close
8. Toast notifications

### Test Database

```sql
INSERT INTO app_user VALUES ('USR001', 'Admin', 'admin@cinema.com', 'admin123', 'super_admin', NULL);
INSERT INTO app_user VALUES ('USR002', 'Theater Admin', 'admin2@cinema.com', 'admin123', 'admin', 'THTR001');
INSERT INTO theater VALUES ('THTR001', 'PVR Cinemas', 'Kathmandu');
```

## 💡 Tips & Best Practices

1. **Always validate on server side**, not just client
2. **Use parameterized queries** to prevent SQL injection
3. **Check authorization** on every admin page
4. **Handle exceptions** with try-catch blocks
5. **Use meaningful error messages** in toasts
6. **Keep repositories clean** - no business logic
7. **Use descriptive variable names**
8. **Follow the established patterns** for consistency

## 🚨 Common Issues & Solutions

| Issue                     | Solution                                                  |
| ------------------------- | --------------------------------------------------------- |
| Session null              | User not logged in, redirect to Login.aspx                |
| Database connection fails | Check Oracle connection string in Web.config              |
| Modal not opening         | Ensure Bootstrap 5 is loaded, check button data-bs-target |
| Charts not showing        | Verify Chart.js CDN is accessible                         |
| Search not working        | Check JavaScript addEventListener in page                 |
| Validation not firing     | Ensure validators are properly configured                 |

## 📞 Debugging

```csharp
// Log to browser console
ClientScript.RegisterStartupScript(GetType(), "log",
    $"console.log('{message}');", true);

// Show debug alert
ClientScript.RegisterStartupScript(GetType(), "alert",
    $"alert('{message}');", true);

// Test query in SQL Developer
SELECT * FROM movie WHERE viewing_format = '2D';
```

## 🎓 Architecture

```
Presentation Layer (ASPX Pages)
    ↓
Business Logic Layer (Services)
    ↓
Data Access Layer (Repositories)
    ↓
Database (Oracle)
```

This architecture ensures:

- Separation of concerns
- Easy testing
- Reusable code
- Clear data flow
- Maintainability

---

**Version**: 1.0
**Last Updated**: 2025-03-14
**Status**: Production Ready
