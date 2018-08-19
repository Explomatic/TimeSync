using System.Net;
using System.Windows;
using Castle.Core;
using TimeSync.IoC;

namespace TimeSync.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    [Interceptor(typeof(ExceptionInterceptor))]
    public partial class App : Application
    {
        public App()
        {
            ServicePointManager
                    .ServerCertificateValidationCallback += 
                (sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }
    }
}