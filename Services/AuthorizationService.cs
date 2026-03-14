using KumariCinema.Models;

namespace KumariCinema.Services
{
    public class AuthorizationService
    {
        public const string SUPER_ADMIN = "super_admin";
        public const string ADMIN = "admin";
        public const string OWNER = "owner";
        public const string STAFF = "staff";
        public const string CUSTOMER = "customer";

        public bool IsSuperAdmin(AppUser user)
        {
            return user != null && user.Role == SUPER_ADMIN;
        }

        public bool IsAdmin(AppUser user)
        {
            return user != null && user.Role == ADMIN;
        }

        public bool IsOwner(AppUser user)
        {
            return user != null && user.Role == OWNER;
        }

        public bool IsStaff(AppUser user)
        {
            return user != null && user.Role == STAFF;
        }

        public bool IsAdminLevel(AppUser user)
        {
            return user != null && (user.Role == SUPER_ADMIN || user.Role == ADMIN || user.Role == OWNER);
        }

        public bool CanAccessTheater(AppUser user, string theaterId)
        {
            if (user == null) return false;
            if (IsSuperAdmin(user)) return true;
            if (IsAdmin(user) || IsOwner(user))
            {
                return user.TheaterId == theaterId;
            }
            return false;
        }

        public bool CanManageUsers(AppUser user)
        {
            return user != null && (IsSuperAdmin(user) || IsAdmin(user) || IsOwner(user));
        }

        public bool CanManageTheaters(AppUser user)
        {
            return user != null && IsSuperAdmin(user);
        }

        public bool CanManageMovies(AppUser user, string theaterId)
        {
            return user != null && (IsSuperAdmin(user) ||
                   ((IsAdmin(user) || IsOwner(user)) && user.TheaterId == theaterId));
        }

        public bool CanManageShows(AppUser user, string theaterId)
        {
            return user != null && (IsSuperAdmin(user) ||
                   ((IsAdmin(user) || IsOwner(user) || IsStaff(user)) && user.TheaterId == theaterId));
        }

        public bool CanViewBookings(AppUser user, string theaterId)
        {
            return user != null && (IsSuperAdmin(user) ||
                   ((IsAdmin(user) || IsOwner(user) || IsStaff(user)) && user.TheaterId == theaterId));
        }

        public bool CanManagePayments(AppUser user, string theaterId)
        {
            return user != null && (IsSuperAdmin(user) ||
                   ((IsAdmin(user) || IsOwner(user)) && user.TheaterId == theaterId));
        }
    }
}
