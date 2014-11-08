﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CH.Domain.Contracts;
using CH.Domain.Identity;
using CH.Domain.Models.Status;
using CH.Website.Areas.Admin.Json;
using CH.Website.Areas.Admin.Models;
using CH.Website.Controllers;
using CH.Website.Sources.Storage;
using CH.Website.Utility;

namespace CH.Website.Areas.Admin.Controllers
{
    [RouteArea(AreaName.Admin)]
    [Authorize(Roles = IdentityRole.RoleNames.MasterAdmin)]
    public class DataMaintenanceController : BaseController
    {
        [HttpGet]
        public ViewResult Dashboard()
        {
            DashboardModel model = new DashboardModel();

            model.TargetStorageItem = new DashboardStorageItem(GetTarget());
            model.SourceStorageItem = new DashboardStorageItem(GetSource());

            model.Items =
                from m in DataModelSource.GetAll()
                select new DashboardItemModel(m.Value, m.Title);

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> Refresh(int modelID)
        {
            DataModelSource modelSource = DataModelSource.FromValue(modelID);
            if (modelSource == null)
            {
                return Json(null);
            }

            IStorageSource target = GetTarget();
            IStorageSource source = GetSource();

            IDataStatusHandler statusHandler = modelSource.StatusHandler;
            DataStatusModel dataStatus = await statusHandler.GetModelStatusAsync(target, source);

            DashboardItemStatus model = new DashboardItemStatus();
            model.TargetItem = dataStatus.Items.First(x => x.StorageID == target.ID);
            model.SourceItem = dataStatus.Items.First(x => x.StorageID == source.ID);
            model.DataItemCount = dataStatus.DataItemCount;

            return Json(model);
        }

        [HttpPost]
        public async Task<JsonResult> Sync(int modelID)
        {
            DataModelSource modelSource = DataModelSource.FromValue(modelID);
            if (modelSource == null)
            {
                return Json(null);
            }

            IStorageSource target = GetTarget();
            IStorageSource source = GetSource();

            IDataStatusHandler statusHandler = modelSource.StatusHandler;
            DataStatusModel dataStatus = await statusHandler.SyncModelAsync(source, target);

            DashboardItemStatus model = new DashboardItemStatus();
            model.TargetItem = dataStatus.Items.First(x => x.StorageID == target.ID);
            model.SourceItem = dataStatus.Items.First(x => x.StorageID == source.ID);
            model.DataItemCount = dataStatus.DataItemCount;

            return Json(model);
        }

        private IStorageSource GetTarget()
        {
            return new AzureStorageSource(OrganizationContext.AzureName);
        }

        private IStorageSource GetSource()
        {
            return new RescueGroupsStorageSource();
        }
    }
}