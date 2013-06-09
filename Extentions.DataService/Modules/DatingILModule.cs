using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extentions.DataService.Providers;
using log4net;
using System.Reflection;
using Newtonsoft.Json.Linq;
using SquareSnail.Infrastructure;

namespace Extentions.DataService.Modules
{
    public class DatingILModule : ModuleBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DatingILModule(DatingILProvider provider)
            : base("/dating-il")
        {
            Get["/get-facebook-profile"] = x =>
            {
                dynamic result = null;

                try
                {
                    string encodedData = Request.Query.data;
                    string data = Base64Decode(encodedData);
                    dynamic json = JObject.Parse(data);

                    result = provider.GetUserProfileLink((string)json.access_token,
                                                         (string)json.picture_url,
                                                         (string)json.user_info);
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
