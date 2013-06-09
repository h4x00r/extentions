using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using log4net;
using System.Reflection;
using SquareSnail.Infrastructure;

namespace Extentions.DataService.Providers
{
    public class DatingILProvider : ProviderBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public dynamic GetUserProfileLink(string accessToken, string imageUrl, string userInfo)
        {
            dynamic result = new ExpandoObject();

            if (!string.IsNullOrEmpty(accessToken))
            {
                var facebookUserInfo = _commonDataProvider.GetFacebookUser(accessToken, "facebook_id");

                if (facebookUserInfo == null)
                {
                    facebookUserInfo = _facebookDataProvider.GetFacebookUserInfo(accessToken);

                    //check if user is new 
                    var oldUserInfo = _commonDataProvider.GetFacebookUser
                        (long.Parse(facebookUserInfo["id"].AsString), "facebook_id", "gender", "name");

                    if (oldUserInfo == null)
                    {
                        string subject = string.Format("new user on dating-il");
                        string body = string.Format("user: [{0}]; fbid: [{1}]; gender: [{2}]",
                             facebookUserInfo["name"], facebookUserInfo["id"], facebookUserInfo.GetValue("gender", "undefined"));
                        try
                        {
                            Smtp.SendMail("admin@gokapara.com", "admin@gokapara.com", subject, body, null);
                        }
                        catch (Exception ex)
                        {
                            _logger.ErrorFormat("error sending mail ex:[{0}]", ex.Message);
                        }
                    }

                    facebookUserInfo = _commonDataProvider.UpsertFacebookUser(facebookUserInfo, "facebook_id");
                }
            }

            if (!string.IsNullOrEmpty(imageUrl))
            {
                try
                {
                    var url = new Uri(imageUrl);
                    var facebookId = url.Segments[2].Split('_')[1];

                    result.facebook_link = string.Format("http://www.facebook.com/{0}", facebookId);

                    return result;
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormat("failed to retrieve fbid from url:[{0}], error:[{1}]", imageUrl, ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(userInfo))
            {
                try
                {
                    var userIdString = userInfo.Split('_')[1];
                    var userId = int.Parse(userIdString);
                    var userInfoDatingIL = _commonDataProvider.GetFacebookUserFromDatingILDB(userId, "FacebookId");

                    result.facebook_link = string.Format("http://www.facebook.com/{0}", userInfoDatingIL["FacebookId"]);

                    return result;
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormat("failed to retrieve fbid from datingIL db userInfo:[{0}], error:[{1}]",
                        userInfo, ex.Message);
                }
            }

            throw new Exception("no parameters passed");
        }
    }
}
