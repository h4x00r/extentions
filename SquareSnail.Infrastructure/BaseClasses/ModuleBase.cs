using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using System.Web;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;

namespace SquareSnail.Infrastructure
{
    public class ModuleBase : NancyModule
    {
        public ModuleBase()
        {
        }
        public ModuleBase(string modulePath)
            : base(modulePath)
        {
        }

        protected const string USER_ID = "userid";

        protected string Base64Encode(string str)
        {
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(str));

            return base64;
        }
        protected string Base64Decode(string str)
        {
            byte[] result = Convert.FromBase64String(str);
            string decodedString = Encoding.UTF8.GetString(result);

            return decodedString;
        }

        protected dynamic GetRequestJson()
        {
            string bodyString = new StreamReader(Request.Body, Encoding.UTF8).ReadToEnd();

            if (bodyString == null)
                return null;

            var json = JObject.Parse(bodyString);

            return json;
        }
    }
}
