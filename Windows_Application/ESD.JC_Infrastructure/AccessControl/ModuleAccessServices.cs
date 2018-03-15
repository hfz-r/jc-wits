using System.Linq;
using DataLayer.Repositories;

namespace ESD.JC_Infrastructure.AccessControl
{
    public class ModuleAccessServices : IModuleAccessServices
    {
        private IUserRepository userRepository;
        private IModuleAccessCtrlTransactionRepository moduleAccessRepository;

        public ModuleAccessServices(IUserRepository userRepository, IModuleAccessCtrlTransactionRepository moduleAccessRepository)
        {
            this.userRepository = userRepository;
            this.moduleAccessRepository = moduleAccessRepository;
        }

        public bool Initialize(string AuthenticatedUser, string ModuleName)
        {
            var authuser = userRepository.GetUserByUserName(AuthenticatedUser);
            if (authuser != null)
            {
                var moduleaccess = moduleAccessRepository.GetAll(true).Where(name => name.ModuleAccessCtrl.Module == ModuleName);
                if (moduleaccess != null)
                {
                    bool IsAllow = moduleaccess.Where(r => r.Role.ID == authuser.RoleID).Select(ok => ok.IsAllow).FirstOrDefault();
                    return IsAllow;
                }
            }
            return false;
        }
    }

    public interface IModuleAccessServices
    {
        bool Initialize(string AuthenticatedUser, string ModuleName);
    }
}
