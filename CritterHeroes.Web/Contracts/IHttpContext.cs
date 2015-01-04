﻿using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using Microsoft.Owin;

namespace CritterHeroes.Web.Contracts
{
    public interface IHttpContext
    {
        HttpRequestBase Request
        {
            get;
        }

        HttpResponseBase Response
        {
            get;
        }

        IPrincipal User
        {
            get;
        }

        HttpServerUtilityBase Server
        {
            get;
        }
    }
}