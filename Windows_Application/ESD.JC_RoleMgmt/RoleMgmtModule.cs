﻿using DataLayer.Repositories;
using ESD.JC_RoleMgmt.Services;
using ESD.JC_RoleMgmt.Views;
using ESD.JC_Infrastructure;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Regions;
using Prism.Unity;

namespace ESD.JC_RoleMgmt
{
    public class RoleMgmtModule : IModule
    {
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;

        public RoleMgmtModule(IUnityContainer container, IRegionManager regionManager)
        {
            this.container = container;
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            this.container.RegisterType<IRoleRepository, RoleRepository>();
            this.container.RegisterType<IRoleServices, RoleServices>();

            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, () => this.container.Resolve<RoleMgmtNavigationItemView>());
            this.regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion, () => this.container.Resolve<RoleMainView>());
            this.container.RegisterTypeForNavigation<RoleDetailsView>();
            this.container.RegisterTypeForNavigation<RoleOperationView>();
        }
    }
}