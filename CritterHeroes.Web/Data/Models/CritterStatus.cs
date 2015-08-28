﻿using System;
using System.Collections.Generic;
using TOTD.Utility.ExceptionHelpers;

namespace CritterHeroes.Web.Data.Models
{
    public class CritterStatus
    {
        protected CritterStatus()
        {
        }

        public CritterStatus(int id, string name, string description)
            : this(name, description)
        {
            ThrowIf.Argument.IsNullOrEmpty(name, "name");

            this.ID = id;
        }

        public CritterStatus(string name, string description, string rescueGroupsID)
            : this(name, description)
        {
            this.RescueGroupsID = rescueGroupsID;
        }

        public CritterStatus(string name, string description)
        {
            ThrowIf.Argument.IsNullOrEmpty(name, "name");

            this.Name = name;
            this.Description = description;
        }

        public int ID
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string RescueGroupsID
        {
            get;
            set;
        }
    }
}