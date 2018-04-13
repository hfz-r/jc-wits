using System;
using System.Linq;
using DataLayer;
using DataLayer.Repositories;
using ESD.JC_Infrastructure;
using System.Threading;
using System.Text.RegularExpressions;

namespace ESD.JC_Main.LoginServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private IUserRepository userRepository;
        private IModuleAccessCtrlTransactionRepository moduleAccessRepository;

        public AuthenticationService(IUserRepository _userRepository, IModuleAccessCtrlTransactionRepository _moduleAccessRepository)
        {
            userRepository = _userRepository;
            moduleAccessRepository = _moduleAccessRepository;
        }

        public void Authentication(string username, string clearTextPassword)
        {
            var userData = userRepository.Login(username, HashConverter.CalculateHash(clearTextPassword, username));
            if (userData == null)
                throw new UnauthorizedAccessException("Access denied. Please provide some valid credentials.");            

            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
            if (customPrincipal == null)
                throw new ArgumentException("The application's default thread principal must be set to a CustomPrincipal object on startup.");

            customPrincipal.Identity = new CustomIdentity(userData.Username, userData.Role.RoleCode, userData.Email);
        }

        private bool DisableAccessForHandheldUser(long RID)
        {
            var moduleaccess = moduleAccessRepository.GetAll(true).Where(rid => rid.RoleID == RID);
            if (moduleaccess != null)
            {
                Regex regEx = new Regex("Handheld");
                return moduleaccess.Any(mod => regEx.IsMatch(mod.ModuleAccessCtrl.Module) && mod.IsAllow);
            }

            return false;
        }
    }

    public interface IAuthenticationService
    {
        void Authentication(string username, string clearTextPassword);
    }
}
