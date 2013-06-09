using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using Extentions.DataService.Providers;
using SquareSnail.Infrastructure;
using Nancy;

namespace Extentions.DataService.Modules
{
    public class BoostMeModule : ModuleBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BoostMeModule(BoostMeProvider provider)
            : base("/boostme")
        {
            Get["/click-android"] = x =>
            {
                dynamic result = null;

                try
                {
                    int userId = Request.Query.id;
                    string mailType = Request.Query.mailtype;
                    string userIp = Request.Headers["X-Forwarded-For"].Count() == 0 ? Request.UserHostAddress : Request.Headers["X-Forwarded-For"].ToList()[0];

                    result = provider.GetMailClickAndroid(userId, userIp, Request.Headers.UserAgent, mailType);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);

                    result = ex;
                }

                return Response.AsText("ברגעים אלו ממש אנו עובדים על אפליקציית האנדרואיד שתעלה בקרוב.סוד קטן (ששש לא לגלות לכולם): אם תרשמו למטה אנו מבטיחים שתהיו הראשונים לקבל את האפליקציה כשתצא :)", "text/html;charset=UTF-8");
            };
            Get["/click-iphone"] = x =>
            {
                dynamic result = null;

                try
                {
                    int userId = Request.Query.id;
                    string mailType = Request.Query.mailtype;

                    result = provider.GetMailClickIphone(userId, 
                        Request.UserHostAddress, Request.Headers.UserAgent, mailType);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);

                    result = ex;
                }

                return Response.AsRedirect((string)result);
            };
            Get["/click-website"] = x =>
            {
                dynamic result = null;

                try
                {
                    int userId = Request.Query.id;
                    string mailType = Request.Query.mailtype;
                    string userIp = Request.Headers["X-Forwarded-For"].Count() == 0 ? Request.UserHostAddress : Request.Headers["X-Forwarded-For"].ToList()[0];

                    result = provider.GetMailClickWebSite(userId, userIp, Request.Headers.UserAgent, mailType);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);

                    result = ex;
                }

                return Response.AsRedirect((string)result);
            };
            Get["/get-image"] = x =>
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

                return Response.AsImage("images/boostme.jpg");
            };
            Get["/get-mail"] = x =>
            {
                return View["boostme-girl"];
            };
            Get["/unsubscribe"] = x =>
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
        }
    }
}
