﻿using System;
using System.Collections.Generic;
using CH.Domain.Contracts.Queries;
using CH.Domain.Models;
using CH.Domain.Models.Data;

namespace CH.Domain.StateManagement
{
    public class OrganizationContext : IQueryResult
    {
        public static OrganizationContext FromOrganization(Organization organization)
        {
            return new OrganizationContext()
            {
                OrganizationID = organization.ID,
                FullName = organization.FullName,
                ShortName = organization.ShortName,
                AzureName = organization.AzureName,
                LogoFilename = organization.LogoFilename,
                SupportedCritters = organization.SupportedCritters
            };
        }

        public Guid OrganizationID
        {
            get;
            set;
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

        public IEnumerable<Species> SupportedCritters
        {
            get;
            set;
        }
    }
}
