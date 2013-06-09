using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Session;
using System.ComponentModel;
using log4net.Config;
using System.IO;
using System.Web;
using Nancy.Conventions;

namespace Extentions.DataService
{
    public class ErrorHandler : IApplicationStartup
    {
        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get
            {
                return null;
            }
        }
        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations
        {
            get
            {
                return null;
            }
        }
        public IEnumerable<InstanceRegistration> InstanceRegistrations
        {
            get
            {
                return null;
            }
        }

        public void Initialize(IPipelines pipelines)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(HttpRuntime.AppDomainAppPath, "log4net.config")));
            CookieBasedSessions.Enable(pipelines);
        }
    }

    public class CustomBoostrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);
        }
    }
}
