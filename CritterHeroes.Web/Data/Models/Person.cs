﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CritterHeroes.Web.Data.Models
{
    public class Person : BaseContact
    {
        public Person() :
            base()
        {
            Groups = new HashSet<PersonGroup>();
            PhoneNumbers = new HashSet<PersonPhone>();
            IsActive = true;
        }

        public string FirstName
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string NewEmail
        {
            get;
            set;
        }

        public bool IsEmailConfirmed
        {
            get;
            set;
        }

        public bool IsActive
        {
            get;
            set;
        }

        public virtual ICollection<PersonGroup> Groups
        {
            get;
            private set;
        }

        public virtual ICollection<PersonPhone> PhoneNumbers
        {
            get;
            private set;
        }

        public virtual ICollection<Critter> Critters
        {
            get;
            private set;
        }

        public void AddGroup(int groupID)
        {
            PersonGroup personGroup = new PersonGroup(this, groupID);
            Groups.Add(personGroup);
        }

        public void AddGroup(Group group)
        {
            PersonGroup personGroup = new PersonGroup(this, group);
            Groups.Add(personGroup);
        }

        public PersonPhone AddPhoneNumber(string phoneNumber, string phoneExtension, int phoneTypeID)
        {
            PersonPhone personPhone = new PersonPhone(this, phoneTypeID, phoneNumber, phoneExtension);
            PhoneNumbers.Add(personPhone);
            return personPhone;
        }

        public PersonPhone AddPhoneNumber(string phoneNumber, string phoneExtension, PhoneType phoneType)
        {
            PersonPhone personPhone = new PersonPhone(this, phoneType, phoneNumber, phoneExtension);
            PhoneNumbers.Add(personPhone);
            return personPhone;
        }
    }
}
