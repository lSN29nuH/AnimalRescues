﻿using System;
using System.Collections.Generic;
using System.Linq;
using CritterHeroes.Web.Contracts;
using CritterHeroes.Web.Contracts.Configuration;
using CritterHeroes.Web.Contracts.Events;
using CritterHeroes.Web.DataProviders.RescueGroups.Models;
using Newtonsoft.Json.Linq;

namespace CritterHeroes.Web.DataProviders.RescueGroups.Storage
{
    public class CritterStatusSourceStorage : RescueGroupsStorage<CritterStatusSource>
    {
        public CritterStatusSourceStorage(IRescueGroupsConfiguration configuration, IHttpClient client, IAppEventPublisher publisher)
            : base(configuration, client, publisher)
        {
            this.Fields = new[]
            {
                new SearchField("name"),
                new SearchField("description")
            };
        }

        public override string ObjectType
        {
            get
            {
                return "animalStatuses";
            }
        }

        public override bool IsPrivate
        {
            get
            {
                return true;
            }
        }

        public override IEnumerable<SearchField> Fields
        {
            get;
        }

        protected override string SortField
        {
            get
            {
                return "name";
            }
        }

        protected override string KeyField
        {
            get
            {
                return "id";
            }
        }

        public override IEnumerable<CritterStatusSource> FromStorage(IEnumerable<JProperty> tokens)
        {
            return tokens.Select(x => new CritterStatusSource(x.Name, x.Value.Value<string>("name"), x.Value.Value<string>("description")));
        }
    }
}
