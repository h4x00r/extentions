using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using Nancy;

namespace SquareSnail.Infrastructure
{
    public class JsonSerializer : ISerializer
    {
        private readonly bool _enableBase64Output = false;

        public bool CanSerialize(string contentType)
        {
            if (contentType == "application/json")
                return true;
            return false;
        }
        public void Serialize<TModel>(string contentType, TModel model, Stream outputStream)
        {
            if (contentType != "application/json")
                throw new NotSupportedException(contentType);

            string serialized = null;
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
            if (model is string)
                serialized = model as string;
            else
                serialized = JsonConvert.SerializeObject(model, Formatting.None, settings);

            if (_enableBase64Output)
            {
                dynamic result = new
                {
                    result = Convert.ToBase64String(Encoding.UTF8.GetBytes(serialized == null ? model as string : serialized))
                };

                serialized = JsonConvert.SerializeObject(result, Formatting.None, settings);
            }

            StreamWriter streamWriter = new StreamWriter(outputStream, new UTF8Encoding(false));
            streamWriter.AutoFlush = true;
            streamWriter.Write(serialized);
        }
        public IEnumerable<string> Extensions
        {
            get
            {
                return null;
            }
        }
    }
}
