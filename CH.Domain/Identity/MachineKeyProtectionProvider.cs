﻿using System;
using System.Collections.Generic;
using Microsoft.Owin.Security.DataProtection;

namespace CH.Domain.Identity
{
    public class MachineKeyProtectionProvider : IDataProtectionProvider
    {
        public IDataProtector Create(params string[] purposes)
        {
            return new MachineKeyDataProtector(purposes);
        }
    }
}
