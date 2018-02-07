using System;
using DataLayer;
using DataLayer.Repositories;
using ESD.JC_Infrastructure;
using System.Threading;

namespace ESD.JC_Main.LoginServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private IUserRepository userRepository;

        public AuthenticationService()
        {
            userRepository = new UserRepository(new InventoryContext());
        }

        public AuthenticationService(IUserRepository _userRepository)
        {
            userRepository = _userRepository;
        }

        public void Authentication(string username, string clearTextPassword)
        {
            try
            {
                var userData = userRepository.Login(username, HashConverter.CalculateHash(clearTextPassword, username));
                if (userData == null)
                    throw new UnauthorizedAccessException("Access denied. Please provide some valid credentials.");

                CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
                if (customPrincipal == null)
                    throw new ArgumentException("The application's default thread principal must be set to a CustomPrincipal object on startup.");

                customPrincipal.Identity = new CustomIdentity(userData.Username, userData.Role.RoleCode, userData.Email);
            }
            catch
            {
                throw;
            }
            finally
            {
                userRepository.Dispose();
            }
        }
    }

    public interface IAuthenticationService
    {
        void Authentication(string username, string clearTextPassword);
    }
}
