﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CH.Domain.Contracts.Dashboard;
using CH.Domain.Models.Data;
using CH.Domain.Models.Json;
using CH.Domain.Services.Commands;
using CH.Domain.Services.Queries;
using CH.Domain.StateManagement;

namespace CH.Website.Areas.Admin.Sources
{
    public class BreedDataModelSource : DataModelSource
    {
        public BreedDataModelSource(int sourceID, string title)
            : base(sourceID, title)
        {
        }

        public override async Task<DashboardItemStatus> GetDashboardItemStatusAsync(IDependencyResolver dependencyResolver, IStorageSource source, IStorageSource target, OrganizationContext organizationContext)
        {
            DashboardStatusQuery<Breed> query = new DashboardStatusQuery<Breed>(target, source, organizationContext);
            return await dependencyResolver.GetService<IDashboardStatusQueryHandler<Breed>>().RetrieveAsync(query);
        }

        public override async Task<CommandResult> ExecuteSyncAsync(IDependencyResolver dependencyResolver, OrganizationContext organizationContext)
        {
            DashboardStatusCommand<Breed> command = new DashboardStatusCommand<Breed>(organizationContext);
            return await dependencyResolver.GetService<IDashboardStatusCommandHandler<Breed>>().ExecuteAsync(command);
        }
    }
}