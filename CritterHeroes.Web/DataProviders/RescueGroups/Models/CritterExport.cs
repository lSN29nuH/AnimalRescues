﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CritterHeroes.Web.DataProviders.RescueGroups.Models
{
    [XmlRoot(ElementName = "animals")]
    public class CritterXml
    {
        [XmlElement(ElementName = "animals")]
        public CritterExport[] Critters
        {
            get;
            set;
        }
    }

    public class CritterExport
    {
        [XmlElement(ElementName = "Animal_AnimalID")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Animal_InternalID")]
        public string InternalID
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Animal_RescueID")]
        public string RescueID
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Animal_LastUpdatedDT")]
        public string LastUpdated
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Animal_Breed")]
        public string Breed
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Foster_ContactID")]
        public string FosterContactID
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Animal_Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Animal_Sex")]
        public string Sex
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Animal_Species")]
        public string Species
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Animal_Status")]
        public string Status
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Animal_Picture1")]
        public string Picture1
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Animal_Picture2")]
        public string Picture2
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Animal_Picture3")]
        public string Picture3
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Animal_Picture4")]
        public string Picture4
        {
            get;
            set;
        }
    }
}
