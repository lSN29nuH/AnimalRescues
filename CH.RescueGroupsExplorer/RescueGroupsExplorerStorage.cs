﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CH.RescueGroups;
using CH.RescueGroups.Configuration;
using Newtonsoft.Json.Linq;

namespace CH.RescueGroupsExplorer
{
    public class RescueGroupsExplorerStorage : RescueGroupsStorage<JProperty>
    {
        private string _objectType;
        private bool _isPrivate;
        private string _objectAction;

        public RescueGroupsExplorerStorage()
            : base(new RescueGroupsConfiguration())
        {
        }

        public override string ObjectType
        {
            get
            {
                return _objectType;
            }
        }

        public override string ObjectAction
        {
            get
            {
                return _objectAction;
            }
        }

        public override bool IsPrivate
        {
            get
            {
                return _isPrivate;
            }
        }

        public async Task<IEnumerable<JProperty>> GetAllAsync(string objectType, string objectAction, bool isPrivate)
        {
            this._objectType = objectType;
            this._objectAction = objectAction;
            this._isPrivate = isPrivate;

            return await GetAllAsync();
        }

        public override IEnumerable<JProperty> FromStorage(IEnumerable<JProperty> tokens)
        {
            return tokens;
        }
    }
}