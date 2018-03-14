using DataLayer;
using DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;

namespace ESD.JC_RoleMgmt.Services
{
    public class ModuleAccessCtrlServices : IModuleAccessCtrlServices
    {
        private IModuleAccessCtrlRepository moduleAccessCtrlRepository;

        public ModuleAccessCtrlServices(IModuleAccessCtrlRepository _moduleAccessCtrlRepository)
        {
            moduleAccessCtrlRepository = _moduleAccessCtrlRepository;
        }

        public IEnumerable<ModuleAccessCtrl> GetAll()
        {
            return moduleAccessCtrlRepository.GetAll();
        }

        public ModuleAccessCtrl GetModule(long ID)
        {
            return moduleAccessCtrlRepository.GetModuleAccessCtrl(ID);
        }
    }
}
