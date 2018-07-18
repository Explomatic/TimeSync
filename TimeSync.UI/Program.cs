using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace TimeSync.UI
{
    public class Program
    {
        protected static ConcurrentDictionary<string, Assembly> LoadedAssemblies { get; } = new ConcurrentDictionary<string, Assembly>();
        private static readonly Object _lock = new object();
        private static Assembly MainAssembly { get; set; }

        [STAThread]
        public static void Main()
        {
            MainAssembly = Assembly.GetExecutingAssembly();
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
            App.Main();
        }

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            // Get assembly path
            AssemblyName assemblyName = new AssemblyName(args.Name);
            var test = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
            string path = "";
            if (assemblyName.Name.Contains("resources"))
            {
                //path = assemblyName.Name;
                path = "TimeSync.g.resources";
            }
            else
            {
                path = $"{assemblyName.Name}.dll";
            }
                
            //if ((assemblyName.CultureInfo != null) && !assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture))
            //{
            //    path = $@"{assemblyName.CultureInfo}\{path}";
            //}
            var assembly = LoadedAssemblies.GetOrAdd(path, LoadAssemblyFromEmbeddedResources);
            return assembly;
        }

        private static Assembly LoadAssemblyFromEmbeddedResources(string path)
        {
            lock (_lock)
            {
                using (Stream stream = MainAssembly.GetManifestResourceStream(path))
                {
                    if (stream == null) { return null; /* quit early */ }

                    byte[] assemblyRawBytes = new byte[stream.Length];
                    stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                    var assembly = Assembly.Load(assemblyRawBytes);
                    return assembly;
                }
            }
        }
    }
}