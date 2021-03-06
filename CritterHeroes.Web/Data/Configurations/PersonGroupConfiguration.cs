﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using CritterHeroes.Web.Data.Models;
using TOTD.EntityFramework;

namespace CritterHeroes.Web.Data.Configurations
{
    public class PersonGroupConfiguration : EntityTypeConfiguration<PersonGroup>
    {
        public PersonGroupConfiguration()
        {
            HasKey(x => new
            {
                x.PersonID,
                x.GroupID
            });

            HasRequired(x => x.Group).WithMany().WillCascadeOnDelete();
        }
    }
}
