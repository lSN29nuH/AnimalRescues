﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CritterHeroes.Web.Models;

namespace CritterHeroes.Web.Contracts.Logging
{
    public interface IEmailLogger
    {
        Task LogEmailAsync(EmailModel email);
    }
}
