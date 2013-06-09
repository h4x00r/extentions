using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System.Globalization;

namespace Extentions.DataService.DataPrividers
{
    public class CommonDataProvider : DataProviderBase
    {
        public BsonDocument UpsertFacebookUser(BsonDocument facebookUserInfo, params string[] fields)
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(USERS_COLLECTION_NAME);

            var facebookId = long.Parse((string)facebookUserInfo["id"]);
            var selectQuery = Query.EQ("facebook_id", facebookId);
            var userInfo = collection.Find(selectQuery)
                .SetFields(fields)
                .SingleOrDefault();

            if (userInfo != null)
            {
                var updateBuilder = new UpdateBuilder();

                foreach (var val in facebookUserInfo)
                {
                    if (val.Name == "birthday")
                        updateBuilder.Set(val.Name, DateTime.ParseExact((string)val.Value, "MM/dd/yyyy", CultureInfo.InvariantCulture));
                    else if (val.Name == "updated_time")
                        updateBuilder.Set(val.Name, DateTime.Parse((string)val.Value));
                    else if (val.Name == "id")
                        updateBuilder.Set("facebook_id", long.Parse((string)val.Value));
                    else
                        updateBuilder.Set(val.Name, val.Value);
                }

                updateBuilder.Set("date_modified", DateTime.Now);
                updateBuilder.Set("is_active", true);

                collection.Update(Query.EQ("facebook_id", facebookId), updateBuilder);

                return userInfo;
            }

            facebookUserInfo["birthday"] = DateTime.ParseExact((string)facebookUserInfo["birthday"], "MM/dd/yyyy", CultureInfo.InvariantCulture);
            facebookUserInfo["updated_time"] = DateTime.Parse((string)facebookUserInfo["updated_time"]);
            facebookUserInfo["date_modified"] = DateTime.Now;
            facebookUserInfo["date_created"] = DateTime.Now;
            facebookUserInfo["is_active"] = true;
            facebookUserInfo["is_banned"] = false;
            facebookUserInfo["facebook_id"] = long.Parse((string)facebookUserInfo["id"]);
            facebookUserInfo.Remove("id");

            collection.Insert(facebookUserInfo);

            return facebookUserInfo;
        }
        public BsonDocument GetFacebookUser(string accessToken, params string[] fields)
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(USERS_COLLECTION_NAME);

            var selectQuery = Query.EQ("access_token", accessToken);
            var userInfo = collection.Find(selectQuery)
                .SetFields(fields)
                .SingleOrDefault();

            return userInfo;
        }
        public BsonDocument GetFacebookUser(long facebookId, params string[] fields)
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(USERS_COLLECTION_NAME);

            var selectQuery = Query.EQ("facebook_id", facebookId);
            var userInfo = collection.Find(selectQuery)
                .SetFields(fields)
                .SingleOrDefault();

            return userInfo;
        }
        public BsonDocument GetFacebookUserFromDatingILDB(int userId, params string[] fields)
        {
            var dataBase = _serverWrapper.GetServer(DATING_IL_DB_NAME).GetDatabase(DATING_IL_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(DATING_IL_USERS_COLLECTION_NAME);

            var selectQuery = Query.EQ("_id", userId);
            var userInfo = collection.Find(selectQuery)
                .SetFields(fields)
                .SingleOrDefault();

            return userInfo;
        }

        public void InsertUserPageView(BsonDocument pageView)
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(PAGE_VIEWS_COLLECTION_NAME);

            collection.EnsureIndex(new IndexKeysBuilder().Descending("date_created"),
                IndexOptions.SetTimeToLive(new TimeSpan(1, 0, 0, 0)));

            collection.Insert(pageView);
        }
        public void InsertUserActivity(BsonDocument activity)
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(USERS_MAIL_ACTIVITY_COLLECTION_NAME);

            collection.Insert(activity);
        }
        public void InsertDatingILActivity(BsonDocument activity)
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(DATING_IL_ACTIVITY_COLLECTION_NAME);

            collection.Insert(activity);
        }
        public void InsertFacebookLogin(BsonDocument login)
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(LOGIN_COLLECTION_NAME);

            collection.Insert(login);
        }

        public List<BsonDocument> GetPotentialUsers()
        {
            var dataBase = _serverWrapper.GetServer(DATING_IL_DB_NAME).GetDatabase(DATING_IL_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(DATING_IL_USERS_COLLECTION_NAME);

            var query = Query.And(
                Query.EQ("Gender", "boy"),
                Query.NE("_id", 17),
                Query.NE("_id", 24),
                Query.NE("_id", 10836),
                Query.NE("Email", BsonNull.Value),
                Query.GTE("DateCreated", new DateTime(2013, 1, 1)),
                Query.GTE("DateModified", new DateTime(2013, 4, 9)));

            var result = collection.Find(query).ToList();

            return result;
        }
        public List<BsonDocument> GetPotentialUsers2()
        {
            var dataBase = _serverWrapper.GetServer(DATING_IL_DB_NAME).GetDatabase(DATING_IL_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(DATING_IL_USERS_COLLECTION_NAME);

            var query = Query.And(
                Query.NE("_id", 17),
                Query.NE("_id", 24),
                Query.NE("_id", 10836),
                Query.LTE("DateCreated", new DateTime(2013, 4, 5)),
                Query.NE("Email", BsonNull.Value));

            var result = collection.Find(query)
                .SetSortOrder(SortBy.Descending("DateModified"))
                .SetLimit(50)
                .ToList();

            return result;
        }

        public List<BsonDocument> GetBoostMeMailUsersBoy()
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(DATING_IL_USERS_COLLECTION_NAME);

            var query = Query.And(
                Query.NE("_id", 17),
                Query.NE("_id", 24),
                Query.NE("_id", 10836),
                Query.EQ("boostme_mail", BsonNull.Value),
                Query.EQ("Gender", "boy"),
                Query.NE("Email", BsonNull.Value));

            var result = collection.Find(query)
                .SetSortOrder(SortBy.Descending("DateModified"))
                .SetFields(Fields.Include("FirstName", "Email", "Gender"))
                .SetLimit(400)
                .ToList();

            return result;
        }
        public List<BsonDocument> GetBoostMeMailUsersGirl()
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(DATING_IL_USERS_COLLECTION_NAME);

            var query = Query.And(
                Query.NE("_id", 17),
                Query.NE("_id", 24),
                Query.NE("_id", 10836),
                Query.EQ("boostme_mail", BsonNull.Value),
                Query.EQ("Gender", "girl"),
                Query.NE("Email", BsonNull.Value));

            var result = collection.Find(query)
                .SetSortOrder(SortBy.Descending("DateModified"))
                .SetFields(Fields.Include("FirstName", "Email", "Gender"))
                .SetLimit(400)
                .ToList();

            return result;
        }

        public void SettBoostMeMailUserMailSent(BsonDocument user)
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(DATING_IL_USERS_COLLECTION_NAME);

            collection.Update(Query.EQ("_id", user["_id"]), Update.Set("boostme_mail", true));
        }

        public void MarkUserAsSentExtention(int userId, string email, string hebrewName, string englishName, string gender)
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(USERS_MAILING_COLLECTION_NAME);

            var user = new BsonDocument();

            user["user_id"] = userId;
            user["date_created"] = DateTime.Now;
            user["app"] = "dating-il-ext";
            user["email"] = email;
            user["hebrew_name"] = hebrewName;
            user["english_name"] = englishName;
            user["gender"] = gender;

            collection.Insert(user);
        }
        public void MarkUserAsSentBoostMe(int userId, string email, string hebrewName, string englishName, string gender)
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(BOOSTME_USERS_MAILING_COLLECTION_NAME);

            var user = new BsonDocument();

            user["user_id"] = userId;
            user["date_created"] = DateTime.Now;
            user["app"] = "boost-me";
            user["email"] = email;
            user["english_name"] = englishName;
            user["hebrew_name"] = hebrewName;
            user["gender"] = gender;

            collection.Insert(user);
        }
        public bool CheckIfUserMarkedExtention(int userId)
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(USERS_MAILING_COLLECTION_NAME);

            var result = collection.Find(Query.EQ("user_id", userId)).SingleOrDefault();

            if (result == null)
                return false;
            else
                return true;
        }
        public bool CheckIfUserMarkedBoostMe(int userId)
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(BOOSTME_USERS_MAILING_COLLECTION_NAME);

            var result = collection.Find(Query.EQ("user_id", userId)).SingleOrDefault();

            if (result == null)
                return false;
            else
                return true;
        }
        public BsonDocument GetUserHebrewName(string userName)
        {
            var dataBase = _serverWrapper.GetServer(DATING_IL_DB_NAME).GetDatabase(DATING_IL_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(HEBREW_NAMES_COLLECTION_NAME);
            var query = Query.And(Query.EQ("english_name", userName));

            var result = collection.Find(query).SingleOrDefault();

            return result;
        }


        public BsonDocument GetFacebookUserExt(long facebookId, int extId, params string[] fields)
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(EXT_USERS_COLLECTION_NAME);

            var selectQuery = Query.And(Query.EQ("facebook_id", facebookId), Query.EQ("ext_id", extId));
            var userInfo = collection.Find(selectQuery)
                .SetFields(fields)
                .SingleOrDefault();

            return userInfo;
        }
        public BsonDocument InsertFacebookUserExt(BsonDocument facebookUserInfo, int extId, params string[] fields)
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(EXT_USERS_COLLECTION_NAME);

            var facebookId = long.Parse((string)facebookUserInfo["id"]);

            facebookUserInfo["date_modified"] = DateTime.Now;
            facebookUserInfo["date_created"] = DateTime.Now;
            facebookUserInfo["ext_id"] = extId;
            facebookUserInfo["facebook_id"] = facebookId;
            facebookUserInfo.Remove("id");

            collection.Insert(facebookUserInfo);

            return facebookUserInfo;
        }
        public void UpdateFacebookUserVisit(long facebookId, int extId)
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(EXT_USERS_COLLECTION_NAME);
            var selectQuery = Query.And(Query.EQ("facebook_id", facebookId), Query.EQ("ext_id", extId));

            collection.Update(selectQuery, Update.Set("date_modified", DateTime.Now));
        }
    }
}
