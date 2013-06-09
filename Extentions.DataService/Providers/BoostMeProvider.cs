using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MongoDB.Bson;
using System.Web;

namespace Extentions.DataService.Providers
{
    public class BoostMeProvider : ProviderBase
    {
        public dynamic GetMailClickIphone(int userId, string ip, string userAgent, string mailType)
        {
            var doc = new BsonDocument();

            doc["date_created"] = DateTime.Now;
            doc["user_id"] = userId;
            doc["type"] = "iphone-click";
            doc["ip"] = ip;
            doc["user_agent"] = userAgent;
            doc["mail_type"] = mailType == null ? "" : mailType;

            _boostMeDataProvider.InsertUserActivity(doc);

            return "https://itunes.apple.com/il/app/boostme!/id625167423";
        }
        public dynamic GetMailClickAndroid(int userId, string ip, string userAgent, string mailType)
        {
            var doc = new BsonDocument();

            doc["date_created"] = DateTime.Now;
            doc["user_id"] = userId;
            doc["type"] = "android-click";
            doc["ip"] = ip;
            doc["user_agent"] = userAgent;
            doc["mail_type"] = mailType == null ? "" : mailType;

            _boostMeDataProvider.InsertUserActivity(doc);

            return null;
        }
        public dynamic GetMailClickWebSite(int userId, string ip, string userAgent, string mailType)
        {
            var doc = new BsonDocument();

            doc["date_created"] = DateTime.Now;
            doc["user_id"] = userId;
            doc["type"] = "website-click";
            doc["ip"] = ip;
            doc["user_agent"] = userAgent;
            doc["mail_type"] = mailType == null ? "" : mailType;

            _boostMeDataProvider.InsertUserActivity(doc);

            return "http://www.boostmeapp.com";
        }
        public dynamic GetMailUnsubscribe(int userId, string ip, string userAgent, string mailType)
        {
            var doc = new BsonDocument();

            doc["date_created"] = DateTime.Now;
            doc["user_id"] = userId;
            doc["type"] = "unsubscribe";
            doc["ip"] = ip;
            doc["user_agent"] = userAgent;
            doc["mail_type"] = mailType == null ? "" : mailType;

            _boostMeDataProvider.InsertUserActivity(doc);

            return null;
        }
        public dynamic GetMailImage(int userId, string ip, string userAgent, string mailType)
        {
            var doc = new BsonDocument();

            doc["date_created"] = DateTime.Now;
            doc["user_id"] = userId;
            doc["type"] = "open-mail";
            doc["ip"] = ip;
            doc["user_agent"] = userAgent;
            doc["mail_type"] = mailType == null ? "" : mailType;

            _boostMeDataProvider.InsertUserActivity(doc);

            return null;
        }
    }
}

