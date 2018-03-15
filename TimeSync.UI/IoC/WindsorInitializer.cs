using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using TimeSync.DataAccess;

namespace TimeSync.UI.IoC
{
    public static class WindsorInitializer
    {
        public static void InitializeContainer(IWindsorContainer container)
        {
            container.Register(
                Component.For<TimeManager>()
            );
        }
    }
}
