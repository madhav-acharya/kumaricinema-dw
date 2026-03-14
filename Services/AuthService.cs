using KumariCinema.Models;
using KumariCinema.Repositories;
using System;

namespace KumariCinema.Services
{
    public class AuthService
    {
        private readonly AppUserRepository _userRepository;

        public AuthService()
        {
            _userRepository = new AppUserRepository();
        }

        public AppUser Login(string email, string password)
        {
            try
            {
                var user = _userRepository.GetByEmail(email);
                if (user != null && user.Password == password)
                {
                    return user;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during login: " + ex.Message);
            }
        }

        public bool Register(AppUser user)
        {
            try
            {
                var existingUser = _userRepository.GetByEmail(user.Email);
                if (existingUser != null)
                {
                    return false;
                }
                return _userRepository.Insert(user);
            }
            catch (Exception ex)
            {
                throw new Exception("Error during registration: " + ex.Message);
            }
        }

        public bool ChangePassword(string userId, string oldPassword, string newPassword)
        {
            try
            {
                var user = _userRepository.GetById(userId);
                if (user == null || user.Password != oldPassword)
                {
                    return false;
                }
                user.Password = newPassword;
                return _userRepository.Update(user);
            }
            catch (Exception ex)
            {
                throw new Exception("Error changing password: " + ex.Message);
            }
        }
    }
}
