﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CritterHeroes.Web.Data.Models;
using CritterHeroes.Web.Domain.Contracts.Events;
using CritterHeroes.Web.Domain.Contracts.Storage;

namespace CritterHeroes.Web.Data.Contexts
{
    public class CritterBatchStorageContext : BaseDbContext<CritterBatchStorageContext>, ICritterBatchSqlStorageContext
    {
        public CritterBatchStorageContext(IAppEventPublisher publisher)
            : base(publisher)
        {
        }

        public virtual IDbSet<Breed> _Breeds
        {
            get;
            set;
        }

        public virtual IDbSet<CritterStatus> _CritterStatus
        {
            get;
            set;
        }

        public virtual IDbSet<Species> _Species
        {
            get;
            set;
        }

        public virtual IDbSet<Critter> _Critters
        {
            get;
            set;
        }

        public virtual IDbSet<Person> _Persons
        {
            get;
            set;
        }

        public virtual IDbSet<Location> _Locations
        {
            get;
            set;
        }

        public virtual IDbSet<CritterColor> _Colors
        {
            get;
            set;
        }

        public IQueryable<Breed> Breeds => _Breeds;

        public IQueryable<Critter> Critters => _Critters;

        public IQueryable<CritterStatus> CritterStatus => _CritterStatus;

        public IQueryable<Species> Species => _Species;

        public IQueryable<Person> People => _Persons;

        public IQueryable<Location> Locations => _Locations;

        public IQueryable<CritterColor> Colors => _Colors;

        public void AddCritter(Critter critter)
        {
            _Critters.Add(critter);
        }
    }
}
