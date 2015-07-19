﻿using System;
using System.Collections.Generic;
using CritterHeroes.Web.Data.Models;

namespace CritterHeroes.Web.Models
{
    public class Organization
    {
        public Organization()
        {
            ID = Guid.NewGuid();
        }

        public Organization(Guid organizationID)
        {
            this.ID = organizationID;
        }

        public Guid ID
        {
            get;
            private set;
        }

        public string FullName
        {
            get;
            set;
        }

        public string ShortName
        {
            get;
            set;
        }

        public string AzureName
        {
            get;
            set;
        }

        public string LogoFilename
        {
            get;
            set;
        }

        public string EmailAddress
        {
            get;
            set;
        }

        public IEnumerable<Species> SupportedCritters
        {
            get;
            set;
        }
    }
}
