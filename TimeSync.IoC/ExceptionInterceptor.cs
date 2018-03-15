using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSync.IoC
{
    public class ExceptionInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
            //TODO Put this on view model. Do try-catch. If target (I think) can be cast as BaseViewModel then activate popup. Else just throw?
        }
    }
}
