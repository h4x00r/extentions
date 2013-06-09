using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquareSnail.Infrastructure;
using log4net;
using System.Reflection;
using System.Configuration;
using System.IO;
using Nancy;
using System.Web;
using Extentions.DataService.Providers;
using SquareSnail.Infrastructure.Extensions;

namespace Extentions.DataService.Modules
{
    public class StaticModule : ModuleBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public StaticModule(StaticProvider provider)
            : base("/static")
        {
            Get["/get-js"] = p =>
            {
                dynamic result = null;

                try
                {
                    string staticCacheKey = ConfigurationManager.AppSettings["StaticCacheKey"];
                    string requestSaticCacheKey = Request.Headers.IfNoneMatch.Count() == 0 ? null : Request.Headers.IfNoneMatch.ToList()[0];
                    string extentionId = Request.Query.ext == null ? "1000" : Request.Query.ext;

                    if (extentionId == "trijiconx")
                        extentionId = "2000";

                    if (requestSaticCacheKey == staticCacheKey)
                        return new Response().WithStatusCode(HttpStatusCode.NotModified);

                    string fileName = Request.Query.name;

                    result = provider.GetJs(fileName);
                    result = (string)result.Replace("$extId$", extentionId);
                    result = (string)result.Replace("$gaId$", extentionId == "1000" ? "UA-39885571-1" : "UA-39885571-3");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);

                    result = ex.Message;
                }

                return Response.AsText((string)result, "application/javascript")
                       .WithHeader("Etag", ConfigurationManager.AppSettings["StaticCacheKey"]);
            };
            Get["/get-image"] = p =>
            {
                dynamic result = null;

                try
                {
                    string staticCacheKey = ConfigurationManager.AppSettings["StaticCacheKey"];
                    string requestSaticCacheKey = Request.Headers.IfNoneMatch.Count() == 0 ? null : Request.Headers.IfNoneMatch.ToList()[0];

                    if (requestSaticCacheKey == staticCacheKey)
                        return new Response().WithStatusCode(HttpStatusCode.NotModified);

                    string fileName = Request.Query.name;

                    result = provider.GetImage(fileName);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);

                    result = ex.Message;

                    return Response.AsText((string)result, "text/html")
                       .WithHeader("Etag", ConfigurationManager.AppSettings["StaticCacheKey"]);
                }

                return Response.FromByteArray((byte[])result.content, (string)result.content_type)
                       .WithHeader("Etag", ConfigurationManager.AppSettings["StaticCacheKey"]);
            };
            Get["/get-css"] = p =>
            {
                dynamic result = null;

                try
                {
                    string staticCacheKey = ConfigurationManager.AppSettings["StaticCacheKey"];
                    string requestSaticCacheKey = Request.Headers.IfNoneMatch.Count() == 0 ? null : Request.Headers.IfNoneMatch.ToList()[0];

                    if (requestSaticCacheKey == staticCacheKey)
                        return new Response().WithStatusCode(HttpStatusCode.NotModified);

                    string fileName = Request.Query.name;

                    result = provider.GetCss(fileName);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);

                    result = ex.Message;
                }

                return Response.AsText((string)result, "text/css")
                       .WithHeader("Etag", ConfigurationManager.AppSettings["StaticCacheKey"]);
            };
        }
    }
}
