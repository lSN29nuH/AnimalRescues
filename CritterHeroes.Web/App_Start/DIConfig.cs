﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using CritterHeroes.Web.Data.Contexts;
using CritterHeroes.Web.Data.Models.Identity;
using CritterHeroes.Web.Data.Storage;
using CritterHeroes.Web.DataProviders.Azure;
using CritterHeroes.Web.DataProviders.Azure.Services;
using CritterHeroes.Web.DataProviders.RescueGroups.Configuration;
using CritterHeroes.Web.Domain.Contracts;
using CritterHeroes.Web.Domain.Contracts.Commands;
using CritterHeroes.Web.Domain.Contracts.Configuration;
using CritterHeroes.Web.Domain.Contracts.Email;
using CritterHeroes.Web.Domain.Contracts.Events;
using CritterHeroes.Web.Domain.Contracts.Identity;
using CritterHeroes.Web.Domain.Contracts.Logging;
using CritterHeroes.Web.Domain.Contracts.Queries;
using CritterHeroes.Web.Domain.Contracts.StateManagement;
using CritterHeroes.Web.Domain.Contracts.Storage;
using CritterHeroes.Web.Features.Shared;
using CritterHeroes.Web.Shared.Dispatchers;
using CritterHeroes.Web.Shared.Email;
using CritterHeroes.Web.Shared.Events;
using CritterHeroes.Web.Shared.Identity;
using CritterHeroes.Web.Shared.Logging;
using CritterHeroes.Web.Shared.Proxies;
using CritterHeroes.Web.Shared.Proxies.Configuration;
using CritterHeroes.Web.Shared.StateManagement;
using FluentValidation;
using Microsoft.Owin;
using Serilog;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Integration.Web;
using TOTD.Utility.ExceptionHelpers;

namespace CritterHeroes.Web
{
    public class DIConfig
    {
        public static Container ConfigureDependencyContainer(Assembly additionalAssembly = null)
        {
            Container container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            ILogger logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .CreateLogger();
            container.RegisterSingleton<ILogger>(logger);

            List<Assembly> defaultAssemblies = new List<Assembly>();
            defaultAssemblies.Add(typeof(DIConfig).Assembly);
            if (additionalAssembly != null)
            {
                defaultAssemblies.Add(additionalAssembly);
            }

            container.Register(typeof(IStateManager<>), defaultAssemblies, Lifestyle.Scoped);
            container.Register(typeof(IRescueGroupsStorageContext<>), defaultAssemblies);
            container.Register(typeof(IEmailBuilder<>), defaultAssemblies);
            container.Register<IFileSystem, FileSystemProxy>(Lifestyle.Scoped);

            // Register AppUserStorageContext for the one entity SqlStorageContext<> can't handle
            // then register SqlStorageContext<> as a fallback registration for ISqlStorageContext<>
            container.Register<ISqlCommandStorageContext<AppUser>>(() => new AppUserCommandStorageContext(container.GetInstance<IAppEventPublisher>()), Lifestyle.Scoped);
            container.Register<ISqlQueryStorageContext<AppUser>>(() => new AppUserQueryStorageContext(container.GetInstance<IAppEventPublisher>()), Lifestyle.Scoped);

            container.Register<IContactsStorageContext, ContactsStorageContext>(Lifestyle.Scoped);
            container.RegisterConditional(typeof(ISqlCommandStorageContext<>), typeof(SqlCommandStorageContext<>), Lifestyle.Scoped, (c) => !c.Handled);
            container.RegisterConditional(typeof(ISqlQueryStorageContext<>), typeof(SqlQueryStorageContext<>), Lifestyle.Scoped, (c) => !c.Handled);

            container.Register<ICritterBatchSqlStorageContext, CritterBatchStorageContext>(Lifestyle.Scoped);

            container.Register<IAppConfiguration, AppConfiguration>(Lifestyle.Scoped);
            container.Register<IAzureConfiguration, AzureConfiguration>(Lifestyle.Scoped);
            container.Register<IEmailConfiguration, EmailConfiguration>(Lifestyle.Scoped);
            container.Register<IRescueGroupsConfiguration, RescueGroupsConfiguration>(Lifestyle.Scoped);
            container.Register<IHttpUser, HttpUserProxy>(Lifestyle.Scoped);
            container.Register<IHttpContext, HttpContextProxy>(Lifestyle.Scoped);
            container.Register<IUrlGenerator, UrlGenerator>(Lifestyle.Scoped);
            container.Register<IHttpClient, HttpClientProxy>(Lifestyle.Scoped);
            container.Register<IAzureService, AzureService>(Lifestyle.Scoped);

            container.Register<IPageContextService, PageContextService>(Lifestyle.Scoped);
            container.Register<IStateSerializer, StateSerializer>(Lifestyle.Scoped);

            container.Register<IOrganizationLogoService, OrganizationLogoService>();
            container.Register<ICritterPictureService, CritterPictureService>();
            container.Register<ICommandDispatcher, CommandDispatcher>(Lifestyle.Scoped);
            container.Register<IQueryDispatcher, QueryDispatcher>(Lifestyle.Scoped);
            container.Register(typeof(IEmailService<>), typeof(EmailService<>));
            container.Register<IAppEventPublisher, AppEventPublisher>(Lifestyle.Scoped);

            container.Register<IAppLogEventEnricherFactory>(() => new AppLogEventEnricherFactory(container), Lifestyle.Scoped);
            container.Register(typeof(IAppLogEventEnricher<>), defaultAssemblies);

            container.Register(typeof(IValidator<>), defaultAssemblies);

            RegisterIdentityInterfaces(container);
            RegisterHandlers(container, defaultAssemblies);
            RegisterContextSensitiveInterfaces(container);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            return container;
        }

        public static void RegisterIdentityInterfaces(Container container)
        {
            container.Register<IAppSignInManager, AppSignInManager>(Lifestyle.Scoped);
            container.Register<AppUserCommandStorageContext>(() => new AppUserCommandStorageContext(container.GetInstance<IAppEventPublisher>()), Lifestyle.Scoped);
            container.Register<IAppUserStore, AppUserStore>(Lifestyle.Scoped);
            container.Register<IAppUserManager, AppUserManager>(Lifestyle.Scoped);
        }

        public static void RegisterHandlers(Container container, IEnumerable<Assembly> defaultAssemblies)
        {
            container.Register(typeof(IQueryHandler<,>), defaultAssemblies);
            container.Register(typeof(IAsyncQueryHandler<,>), defaultAssemblies);

            container.Register(typeof(IQueryRebuilder<>), defaultAssemblies);
            container.Register(typeof(IAsyncQueryRebuilder<>), defaultAssemblies);

            container.Register(typeof(ICommandHandler<>), defaultAssemblies);
            container.Register(typeof(IAsyncCommandHandler<>), defaultAssemblies);

            container.RegisterCollection(typeof(IAppEventHandler<>), defaultAssemblies);
            container.AppendToCollection(typeof(IAppEventHandler<>), typeof(AppLogEventHandler<>));
        }

        public static void RegisterContextSensitiveInterfaces(Container container)
        {
            container.Register(() =>
            {
                if (container.IsVerifying())
                {
                    return new FakeAuthenticationManager();
                }

                ThrowIf.Argument.IsNull(HttpContext.Current, "HttpContext.Current");
                return container.GetInstance<IOwinContext>().Authentication;
            }, Lifestyle.Scoped);

            container.Register(() =>
            {
                if (container.IsVerifying())
                {
                    return new FakeOwinContext();
                }

                ThrowIf.Argument.IsNull(HttpContext.Current, "HttpContext.Current");
                return HttpContext.Current.GetOwinContext();
            }, Lifestyle.Scoped);
        }
    }
}
