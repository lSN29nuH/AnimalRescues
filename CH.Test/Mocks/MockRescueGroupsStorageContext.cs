﻿using System;
using System.Collections.Generic;
using System.Linq;
using CritterHeroes.Web.DataProviders.RescueGroups.Models;
using CritterHeroes.Web.Domain.Contracts.Storage;
using Moq;

namespace CH.Test.Mocks
{
    public class MockRescueGroupsStorageContext<T> : Mock<IRescueGroupsStorageContext<T>> where T : BaseSource
    {
        public MockRescueGroupsStorageContext(T entity, int entityID)
            : base()
        {
            AddEntity(entity, entityID);
        }

        public MockRescueGroupsStorageContext(params T[] entities)
            : base()
        {
            AddEntities(entities);
        }

        public MockRescueGroupsStorageContext(IEnumerable<T> entities)
            : base()
        {
            AddEntities(entities);
        }

        public void AddEntity(T entity, int entityID)
        {
            AddEntities(new[] { entity });

            this.Setup(x => x.GetAsync(entityID)).ReturnsAsync(entity);
        }

        public void AddEntities(IEnumerable<T> entities)
        {
            this.Setup(x => x.GetAllAsync()).ReturnsAsync(entities);
        }
    }
}
