# Cinema Booking System - Admin Panel Documentation

## Project Structure

```
KumariCinema/
├── Database/
│   └── OracleDatabaseConnection.cs         (Oracle DB Connection)
├── Models/                                  (Entity Models)
│   ├── Theater.cs
│   ├── Movie.cs
│   ├── Genre.cs
│   ├── Language.cs
│   ├── SeatType.cs
│   ├── Hall.cs
│   ├── AppUser.cs
│   ├── Seat.cs
│   ├── MovieShow.cs
│   ├── Booking.cs
│   ├── Ticket.cs
│   └── Payment.cs
├── Repositories/                            (Data Access Layer)
│   ├── IRepository.cs                       (Base Interface)
│   ├── TheaterRepository.cs
│   ├── MovieRepository.cs
│   ├── GenreRepository.cs
│   ├── LanguageRepository.cs
│   ├── SeatTypeRepository.cs
│   ├── HallRepository.cs
│   ├── AppUserRepository.cs
│   ├── SeatRepository.cs
│   ├── MovieShowRepository.cs
│   ├── BookingRepository.cs
│   ├── TicketRepository.cs
│   └── PaymentRepository.cs
├── Services/                                (Business Logic)
│   ├── AuthService.cs                      (Authentication)
│   └── AuthorizationService.cs             (Role-based Access Control)
└── pages/
    ├── Admin.Master                         (Master Layout)
    ├── Admin.Master.cs
    ├── dashboard.aspx                       (Analytics Dashboard)
    ├── dashboard.aspx.cs
    ├── movies.aspx                          (CRUD Template Example)
    ├── movies.aspx.cs
    ├── theaters.aspx
    ├── theaters.aspx.cs
    └── [Other CRUD pages follow same pattern]
```

## Database Tables

15 Oracle tables configured:

- theater
- language
- genre
- seat_type
- movie
- hall
- app_user
- seat
- movie_language (many-to-many)
- movie_genre (many-to-many)
- movie_show
- booking
- ticket
- booking_seat (many-to-many)
- payment

## How to Implement Remaining CRUD Pages

### 1. Genres Page

Create `pages/genres.aspx` and `genres.aspx.cs` following the movies.aspx pattern:

**Key Differences:**

- Table columns: GenreId, Name, Description, Actions
- Form fields: GenreId, Name (text), Description (textarea)
- Repository: GenreRepository

### 2. Languages Page

Similar to genres with columns:

- LanguageId, Name, Code (unique), Actions
- Form fields: LanguageId, Name, Code

### 3. SeatTypes Page

Columns: SeatTypeId, Name, PriceMultiplier, Actions
Form fields: SeatTypeId, Name, Description, PriceMultiplier (number)

### 4. Users Page

Columns: UserId, Name, Email, Role, Theater, Actions
Form fields: UserId, Name, Email, Password, Role (dropdown), Theater (dropdown)
Authorization: Only super_admin can manage all users. Admin/Owner can manage their theater users.

### 5. Halls Page

Columns: HallId, HallName, Capacity, ScreenType, Theater, Actions
Form fields: HallId, HallName, Capacity (number), ScreenType (dropdown: 2D/3D/IMAX/4DX), Theater (dropdown)
Related: HallRepository.GetByTheaterId()

### 6. Seats Page

Columns: SeatId, SeatNumber, Status, SeatType, Actions
Form fields: SeatId, SeatNumber, Status (dropdown: available/reserved/maintenance), SeatType (dropdown)

### 7. Shows Page

Columns: ShowId, Movie, Hall, StartTime, Category, BasePrice, Actions
Form fields: ShowId, Movie (dropdown), Hall (dropdown), StartTime (datetime), EndTime (datetime), Category (dropdown: regular/premium/special), BasePrice (number)
Related: MovieShowRepository.GetByMovieId(), GetByHallId()

### 8. Bookings Page

Columns: BookingId, Movie, User, TotalAmount, Seats, Actions
Display: Expandable row showing booked seats
Related: BookingRepository.GetByUserId(), GetSeatsByBookingId()

### 9. Payments Page

Columns: PaymentId, BookingId, Amount, Method, Status, Actions
Form fields: PaymentId, BookingId (dropdown), Amount (number), PaymentMethod (dropdown: cash/card/esewa/khalti/bank_transfer), PaymentStatus (dropdown: pending/completed/failed/refunded)
Related: PaymentRepository.GetByBookingId()

### 10. Tickets Page

Columns: TicketId, Seat, Show, Price, Status, Actions
Form fields: TicketId, SeatId (dropdown), ShowId (dropdown), TicketPrice (number), TicketStatus (dropdown: booked/cancelled/used)
Related: TicketRepository.GetByShowId()

## Authorization Rules

**Super Admin:**

- Full access to all entities globally
- Can manage all theaters and users

**Admin/Owner:**

- Can only access assigned theater
- Full CRUD for: Movies, Halls, Shows, Seats, Shows
- Can view: Bookings, Payments, Tickets (for their theater only)

**Staff:**

- Limited to shows, bookings, and tickets
- Can only view (read-only) for their assigned theater

**Customer:**

- No admin panel access

## Implementation Steps for New CRUD Pages

1. Create `.aspx` file with modal forms and data table
2. Create `.aspx.cs` code-behind with:
   - `CheckAuthorization()` method
   - `Load[Entity]()` method
   - `Save[Entity]_Click()` for insert
   - `Update[Entity]_Click()` for update
   - `Delete[Entity]()` for delete
   - `SetActiveLink()` for sidebar highlighting

3. Use this pattern for all CRUD operations:

```csharp
protected void SaveGenre_Click(object sender, EventArgs e)
{
    try
    {
        if (!Page.IsValid) return;

        var repo = new GenreRepository();
        var entity = new Genre
        {
            GenreId = genreIdInput.Text,
            Name = genreNameInput.Text,
            Description = descriptionInput.Text
        };

        if (repo.Insert(entity))
        {
            ClearInputs();
            LoadGenres();
            ClientScript.RegisterStartupScript(GetType(), "success", "showToast('Added successfully', 'success');", true);
        }
    }
    catch (Exception ex)
    {
        ClientScript.RegisterStartupScript(GetType(), "error", $"showToast('Error: {ex.Message}', 'error');", true);
    }
}
```

## Key Features Implemented

✅ Role-Based Access Control (RBAC)
✅ Bootstrap 5 Responsive Design
✅ Modal Forms for Add/Edit
✅ Toast Notifications
✅ Search/Filter Functionality
✅ Server-Side Validation
✅ Gradient UI with Font Awesome Icons
✅ Dashboard with Chart.js Analytics
✅ Sidebar Navigation
✅ Master Page Template
✅ Many-to-Many Relationship Handling

## Connection String

Configured in Web.config:

```xml
<add name="OracleConnection"
     connectionString="User Id=kumaricinema;Password=12345;Data Source=192.168.100.13:1521/ORCLPDB1;"
     providerName="Oracle.ManagedDataAccess.Client" />
```

## Authentication Flow

1. User logs in via Login.aspx
2. AuthService validates credentials
3. Redirect to dashboard.aspx
4. Session["CurrentUser"] stores AppUser object
5. Each page checks authorization before loading
6. Redirect to login if not authorized

## Coding Standards Applied

✅ No comments (as requested)
✅ No SQL in .aspx.cs (all in repositories)
✅ AuthorizationService handles all permission checks
✅ Clean, maintainable code structure
✅ Consistent naming conventions
✅ Error handling with try-catch blocks
✅ Toast notifications for user feedback
✅ Modal-based CRUD operations
