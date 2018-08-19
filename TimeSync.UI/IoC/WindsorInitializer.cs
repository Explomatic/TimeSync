using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net;
using TimeSync.DataAccess;
using TimeSync.IoC;
using TimeSync.UI.View;
using TimeSync.UI.ViewModel;

namespace TimeSync.UI.IoC
{
    public static class WindsorInitializer
    {
        public static void InitializeContainer(IWindsorContainer container)
        {
            container.Kernel.Resolver.AddSubResolver(new LoggerSubDependencyResolver());
            
            container.Register(
                Component.For<LoggingInterceptor>(),
                Component.For<ExceptionInterceptor>()
            );
            
            container.Register(
                Component.For(typeof(IRepository<>)).ImplementedBy(typeof(Repository<>)),
                Component.For<ISharepointClient>().ImplementedBy<SharepointClient>(),
                Component.For<IEncryption>().ImplementedBy<Encryption>()
            );
            
            container.Register(
                Component.For<TimeManager>()
            );

            container.Register(
                Component.For<WelcomePage>(),
                Component.For<UserPage>(),
                Component.For<ToolkitsPage>(),
                Component.For<TimeregistrationsPage>(),
                Component.For<SettingsPage>(),
                Component.For<BugReportPage>());

            container.Register(
                Component.For<TimeregistrationViewModel>(),
                Component.For<ToolkitUserViewModel>(),
                Component.For<ToolkitViewModel>(),
                Component.For<SettingsViewModel>(),
                Component.For<WelcomePageViewModel>(),
                Component.For<BugReportViewModel>(),
                Component.For<BaseViewModel>());
        }
    }
    
    public class LoggerSubDependencyResolver : ISubDependencyResolver
    {
        public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model,DependencyModel dependency)
        {
            return dependency.TargetType == typeof (ILog);
        }


        public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model,DependencyModel dependency)
        {
            if (CanResolve(context, contextHandlerResolver, model, dependency))
            {
                if (dependency.TargetType == typeof (ILog))
                {
                    return LogManager.GetLogger(model.Implementation);
                }
            }
            return null;
        }
    }
}
