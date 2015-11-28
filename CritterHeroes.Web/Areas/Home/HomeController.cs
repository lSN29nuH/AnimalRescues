﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CritterHeroes.Web.Areas.Common;
using CritterHeroes.Web.Areas.Home.Models;
using CritterHeroes.Web.Areas.Home.Queries;
using CritterHeroes.Web.Contracts.Commands;
using CritterHeroes.Web.Contracts.Queries;

namespace CritterHeroes.Web.Areas.Home
{
    [Route(HomeController.Route + "/{action}")]
    public class HomeController : BaseController
    {
        public const string Route = "Home";

        public HomeController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
            : base(queryDispatcher, commandDispatcher)
        {
        }

        [ChildActionOnly]
        [AllowAnonymous]
        public PartialViewResult Menu()
        {
            MenuModel model = QueryDispatcher.Dispatch(new MenuQuery());
            return PartialView("_Menu", model);
        }

        [ChildActionOnly]
        [AllowAnonymous]
        public PartialViewResult Header()
        {
            HeaderModel model = QueryDispatcher.Dispatch(new HeaderQuery());
            return PartialView("_Header", model);
        }
    }
}
