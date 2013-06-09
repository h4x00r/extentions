using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;

namespace Extentions.DataService.DataPrividers
{
    public class BoostMeDataProvider : DataProviderBase
    {
        public void InsertUserActivity(BsonDocument activity)
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(BOOSTME_USERS_MAIL_ACTIVITY_COLLECTION_NAME);

            collection.Insert(activity);
        }
    }
}
