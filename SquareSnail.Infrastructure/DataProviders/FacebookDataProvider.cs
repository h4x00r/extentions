using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;

namespace SquareSnail.Infrastructure
{
    public class FacebookDataProvider
    {
        private const string USER_INFO_URL = "https://graph.facebook.com/me";
        private const string USER_INFO_URL_2 = "https://graph.facebook.com/{facebook_id}";

        public BsonDocument GetFacebookUserInfo(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                throw new ArgumentException("accessToken");

            var client = new RestClient();
            var request = new RestRequest(USER_INFO_URL, Method.GET);

            request.AddParameter("access_token", accessToken);

            var response = (RestResponse)client.Execute(request);
            var result = BsonSerializer.Deserialize<BsonDocument>(response.Content);

            if (!result.Contains("id"))
                throw new Exception(string.Format("bad access token [{0}]", accessToken));

            result.Add("access_token", accessToken);

            return result;
        }
        public BsonDocument GetFacebookUserInfo(long facebookId)
        {
            if (facebookId == 0)
                throw new ArgumentException("facebookId");

            var client = new RestClient();
            var request = new RestRequest(USER_INFO_URL_2, Method.GET);

            request.AddParameter("facebook_id", facebookId, ParameterType.UrlSegment);

            var response = (RestResponse)client.Execute(request);
            var result = BsonSerializer.Deserialize<BsonDocument>(response.Content);

            if (!result.Contains("id"))
                throw new Exception(string.Format("bad facebookId [{0}]", facebookId));

            return result;
        }
    }
}
