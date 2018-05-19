using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace TimeSync.IoC
{
    public class ExceptionInterceptor : IInterceptor
    {
        private ILog _log;

        public ExceptionInterceptor()
        {
            _log = LogManager.GetLogger("System"); //TODO Make this a string resource or enum or smth
        }
        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                _log.Warn($"Exception occurred during call to {invocation.Method.Name}", exception: ex);
                var asBaseView = invocation.InvocationTarget as dynamic;
                asBaseView.OpenExceptionViewer(ex.Message, ex);
            }
            
            //TODO Put this on view model. Do try-catch. If target (I think) can be cast as BaseViewModel then activate popup. Else just throw?
        }
    }
}
