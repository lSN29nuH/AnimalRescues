﻿using System;
using System.Collections.Generic;
using System.Linq;
using AR.Domain.Models;
using AR.RescueGroups.Mappings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace AR.Test.RescueGroups
{
    [TestClass]
    public class MappingTests
    {
        [TestClass]
        public class AnimalStatusMappingTests 
        {
            public IRescueGroupsMapping<AnimalStatus> Mapping
            {
                get
                {
                    return RescueGroupsMappingFactory.GetMapping<AnimalStatus>();
                }
            }

            [TestMethod]
            public void ObjectTypeIsCorrect()
            {
                Assert.AreEqual("animalStatuses", Mapping.ObjectType);
            }

            [TestMethod]
            public void ConvertsJsonResultToModel()
            {
                AnimalStatus animalStatus1 = new AnimalStatus("Name 1", "Description 1");
                AnimalStatus animalStatus2 = new AnimalStatus("Name 2", "Description 2");

                JProperty element1 = new JProperty("1", new JObject(new JProperty("name", animalStatus1.Name), new JProperty("description", animalStatus1.Description)));
                JProperty element2 = new JProperty("2", new JObject(new JProperty("name", animalStatus2.Name), new JProperty("description", animalStatus2.Description)));

                JObject data = new JObject();
                data.Add(element1);
                data.Add(element2);

                IEnumerable<AnimalStatus> animalStatuses = Mapping.ToModel(data.Values());
                Assert.AreEqual(2, animalStatuses.Count());

                AnimalStatus result1 = animalStatuses.FirstOrDefault(x => x.Name == animalStatus1.Name);
                Assert.IsNotNull(result1);
                Assert.AreEqual(animalStatus1.Description, result1.Description);

                AnimalStatus result2 = animalStatuses.FirstOrDefault(x => x.Name == animalStatus2.Name);
                Assert.IsNotNull(result2);
                Assert.AreEqual(animalStatus2.Description, result2.Description);
            }
        }
    }
}
