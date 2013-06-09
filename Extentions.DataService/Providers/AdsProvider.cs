using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extentions.DataService.Providers;
using System.Dynamic;
using MongoDB.Bson;
using System.Configuration;

namespace Extentions.DataService.DataPrividers
{
    public class AdsProvider : ProviderBase
    {
        public dynamic GetAd(int adId, string url, string userId, string ip, string userAgent)
        {
            AddImpression(adId, url, userId, ip, userAgent);

            dynamic result = new ExpandoObject();

            string width = null;
            string height = null;

            switch (adId)
            {
                case 1:
                    {
                        width = "300px";
                        height = "250px";
                    }
                    break;
                case 2:
                    {
                        width = "728px";
                        height = "90px";
                    }
                    break;
                case 3:
                    {
                        width = "160px";
                        height = "600px";
                    }
                    break;
                case 4:
                    {
                        width = "250px";
                        height = "250px";
                    }
                    break;
                case 5:
                    {
                        width = "120px";
                        height = "600px";
                    }
                    break;
                case 6:
                    {
                        width = "468px";
                        height = "60px";
                    }
                    break;
            }

            result.ad_id = adId;
            result.width = width;
            result.height = height;

            return result;
        }
        public dynamic GetIsAdsActive()
        {
            bool isActive = bool.Parse(ConfigurationManager.AppSettings["IsAdsActive"]);

            dynamic result = new ExpandoObject();

            result.is_active = isActive;

            return result;
        }

        private void AddImpression(int adId, string url, string userId, string ip, string userAgent)
        {
            Uri urlObj = null;
            Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out urlObj);

            string domainName = urlObj == null ? "" : urlObj.Host;

            var document = new BsonDocument();

            document["date_created"] = DateTime.Now;
            document["user_id"] = userId;
            document["ip"] = ip;
            document["url"] = url;
            document["user_agent"] = userAgent;
            document["domain"] = domainName;
            document["adid"] = adId;

            _adsDataProvider.InsertUserPageView(document);
        }
    }
}
