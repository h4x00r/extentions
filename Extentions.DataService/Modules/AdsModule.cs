using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquareSnail.Infrastructure;
using log4net;
using System.Reflection;
using Extentions.DataService.DataPrividers;
using Newtonsoft.Json.Linq;

namespace Extentions.DataService.Modules
{
    public class AdsModule : ModuleBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public AdsModule(AdsProvider provider)
            : base("/ads")
        {
            Get["/get-ad"] = x =>
            {
                dynamic result = null;

                try
                {
                    string userId = Request.Cookies.ContainsKey("user_id") ? Request.Cookies["user_id"] : "";

                    string encodedData = Request.Query.data;
                    string data = Base64Decode(encodedData);
                    string userIp = Request.Headers["X-Forwarded-For"].Count() == 0 ? Request.UserHostAddress : Request.Headers["X-Forwarded-For"].ToList()[0];

                    dynamic json = JObject.Parse(data);

                    int adId = json.ad_id;
                    string url = json.url;

                    result = provider.GetAd(adId, url, userId, userIp, Request.Headers.UserAgent);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);

                    result = ex;
                }

                return View["ads", result];
            };
            Get["/is-ads-active"] = x =>
            {
                dynamic result = null;

                try
                {
                    result = provider.GetIsAdsActive();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);

                    result = ex;
                }

                return new JsonResponse(result);
            };
        }
    }
}
