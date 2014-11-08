﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using CH.Website.Controllers;

namespace CH.Website.Utility
{
    public static class UrlHelperExtensions
    {
        public static string HomeAction(this UrlHelper urlHelper, Expression<Func<HomeController, ActionResult>> actionSelector)
        {
            return urlHelper.Action<HomeController>(actionSelector);
        }
    }
}