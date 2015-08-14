﻿using System;
using System.Collections.Generic;
using System.Linq;
using CritterHeroes.Web.Contracts.Configuration;
using CritterHeroes.Web.DataProviders.RescueGroups.Models;
using Newtonsoft.Json.Linq;

namespace CritterHeroes.Web.DataProviders.RescueGroups.Storage
{
    public class BreedSourceRescueGroupsStorage : RescueGroupsStorage<BreedSource>
    {
        public BreedSourceRescueGroupsStorage(IRescueGroupsConfiguration configuration)
            : base(configuration)
        {
        }

        public override string ObjectType
        {
            get
            {
                return "animalBreeds";
            }
        }

        public override bool IsPrivate
        {
            get
            {
                return true;
            }
        }

        public override IEnumerable<BreedSource> FromStorage(IEnumerable<JProperty> tokens)
        {
            return tokens.Select(x => new BreedSource(x.Name, x.Value.Value<string>("species"), x.Value.Value<string>("name")));
        }
    }
}
