using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using RedArrow.Framework.Ioc;
using RedArrow.Framework.Ioc.StructureMap;
using RedArrow.Framework.Ioc.StructureMap.Extensions;
using RedArrow.Framework.Mvc.ModelMapper;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Persistence.NHibernate;
using StructureMap;
using StructureMap.Configuration.DSL;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Persistence.Mapping;
using IQI.Intuition.Infrastructure.Persistence.Repositories.Domain;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Infrastructure.Services.Development;
using IQI.Intuition.Infrastructure.Services.Protection;
using IQI.Intuition.Web.Models.Patient;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;


namespace IQI.Intuition.Infrastructure.Ioc
{
    public static class StructureMapConfig
    {
        public enum DataContextMode
        {
            UnitOfWork,
            Stateless
        }

        public static IContainer Configure(DataContextMode dataContextMode)
        {
            // Configure StructureMap
            var registry = new Registry();


            // Scan for dependencies that can be automatically resolved
            registry.Scan(scan =>
            {
                // Register Framework types
                scan.AssemblyContainingType<MvcDependencyResolver>();
                scan.AssemblyContainingType<IDataContext>();
                scan.AssemblyContainingType<DataContext>();
                scan.AssemblyContainingType<IModelMapper>();

                // Register local types
                scan.AssemblyContainingType<AccountRepository>();
                //scan.AssemblyContainingType<IUserSystem>();
                scan.AssemblyContainingType<IAuthentication>();

                // Ignore these types when scanning
                scan.ExcludeNamespaceContainingType<SeedDataBuilder>();

                // Specify the conventions to be used when scanning
                scan.WithDefaultConventions();
                scan.SingleImplementationsOfInterface();

            });


            // Scan for repositories in the domain assembly, ignore everything else
            registry.Scan(scan =>
            {
                scan.AssemblyContainingType<IAccountRepository>();
                scan.IncludeNamespaceContainingType<IAccountRepository>();
            });



            if (dataContextMode == DataContextMode.Stateless)
            {
                registry.For<IStatelessDataContext>().Use<StatelessDataContext>();
                registry.For<IDataContext>().Use<StatelessDataContext>();
                registry.For<IDataContext>().HybridUnitOfWorkLifecycle();

                registry.ForSingletonOf<ISessionActivator>().Use(() =>
                SessionActivator.Build.FromConfiguration(new StatelessConfiguration()));
            }
            else
            {
                registry.ForSingletonOf<ISessionActivator>().Use(() =>
                SessionActivator.Build.FromConfiguration(new StatefulConfiguration(
                    new object[] { 
                        new AccountWatchDogService(),
                        new ChangeTrackingService()
                    })));

                // Cache instances of these types at the HTTP request level
                registry.For<IUnitOfWork>().HybridUnitOfWorkLifecycle();


            }


            // Create a singleton of IModelMapRegistry by scanning the view model assembly
            registry.ForSingletonOf<IModelMapRegistry>().Use(() => 
                ModelMapRegistry.Build.FromScan(scan =>
                    scan.AssemblyContaining<PatientInfoMap>()));



            // Set up MVC 3 to use a ModelMetadataProvider from the ModelMapper library
            registry.For<ModelMetadataProvider>().Use<ModelMapMetadataProvider>();

            // Use SafeHttpContext so we always have an HttpContextBase implementation
            registry.For<HttpContextBase>().Use(() => 
                SafeHttpContext.Current);


            
            registry.For<IModelMapLocator>().HybridHttpOrThreadLocalScoped();
            registry.For<IAuthentication>().HybridHttpOrThreadLocalScoped();
            registry.For<IActionContext>().HybridHttpOrThreadLocalScoped();


            RedArrow.Framework.Logging.log4net.Log.Configure();

            //// Setup logging
            registry.For<ILog>().AlwaysUnique().Use(s =>
            {
                return new RedArrow.Framework.Logging.log4net.Log(
                    log4net.LogManager.GetLogger(typeof(StructureMapConfig))
                    );
            });

            // Direct requests for this interface to the IAuthentication instance in context
            registry.For<IUserNameProvider>().Use(context => 
                context.GetInstance<IAuthentication>());

            // Setup mongo conncetion

            registry.For<IDocumentStore>().Use(() => new Persistence.Reporting.MongoDocumentStore());

            // Create a StructureMap container from the registry we have configured
            var structureMapContainer = new Container(registry);

            // Hook the StructureMap container up to the Common Service Locator
            structureMapContainer.RegisterServiceLocatorProvider();


            AssertConfigurationIsValid(structureMapContainer);

            return structureMapContainer;
        }

        // Only running this validation in DEBUG mode, since there  
        //   is a slight performance penalty for executing it
        [Conditional("DEBUG")]
        private static void AssertConfigurationIsValid(IContainer container)
        {
           // container.AssertConfigurationIsValid();
        }
    }
}