using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using log4net;

namespace TimeSync.IoC
{
    public class LoggingInterceptor : IInterceptor
    {
        private ILog _log;

        public LoggingInterceptor()
        {
            _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); //TODO Make this a string resource or enum or smth
        }

        public void Intercept(IInvocation invocation)
        {
            _log.Info($"Method {invocation.Method.Name} called.");                        
            invocation.Proceed();
        }
    }
}
