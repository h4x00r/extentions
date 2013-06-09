using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using System.IO;
using System.Web;

namespace SquareSnail.Infrastructure
{
    public class JsonResponse : Response
    {
        public JsonResponse(object model)
        {
            GetResult(model, false, true);
        }
        public JsonResponse(object model, bool enableHttpCache)
        {
            GetResult(model, enableHttpCache, true);
        }
        public JsonResponse(object model, bool enableHttpCache, bool enableBase64Output)
        {
            GetResult(model, enableHttpCache, enableBase64Output);
        }

        private void GetResult(object model, bool enableHttpCache, bool enableBase64Output)
        {
            ContentType = "application/json";
            StatusCode = HttpStatusCode.OK;

            if (model == null)
            {
                StatusCode = HttpStatusCode.NoContent;

                return;
            }

            if (model is UnauthorizedAccessException)
            {
                StatusCode = HttpStatusCode.Unauthorized;

                model = new
                {
                    error_type = ((UnauthorizedAccessException)model).GetType().Name,
                    error_message = ((UnauthorizedAccessException)model).Message,
                    status_code = 401
                };

            }
            else if (model is BoostMeException)
            {
                StatusCode = HttpStatusCode.InternalServerError;

                model = new
                {
                    error_type = ((BoostMeException)model).GetType().Name,
                    error_message = ((BoostMeException)model).Message,
                    status_code = ((BoostMeException)model).StatusCode
                };
            }
            else if (model is Exception)
            {
                StatusCode = HttpStatusCode.InternalServerError;

                model = new
                {
                    error_type = ((Exception)model).GetType().Name,
                    error_message = ((Exception)model).Message,
                    status_code = 1000
                };
            }

            Contents = GetJsonContents(model, false);
        }

        private Action<Stream> GetJsonContents(object model, bool enableBase64Output)
        {
            ISerializer jsonSerializer = new JsonSerializer();
            return stream => jsonSerializer.Serialize("application/json", model, stream);
        }
    }
}
