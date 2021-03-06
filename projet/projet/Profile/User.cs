﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using projet.Wall;

namespace projet.Profile
{
    public class User
    {
        [BsonId]
        public String _id { get;private set;}

        //public ObjectId Id { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonElement("Password")]
        public string Password { get; set; }

        [BsonElement("First_name")]
        public string First_name { get; set; }

        [BsonElement("Last_name")]
        public string Last_name { get; set; }

        [BsonElement("Birth_date")]
        public String Birth_date { get; set; }


        [BsonElement("Gender")]
        public string Gender { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }


        [BsonElement("UrlPhoto")]
        public string UrlPhoto { get; set; }


        [BsonElement("City")]
        public string City { get; set; }

        [BsonElement("Country")]
        public string Country { get; set; }

        [BsonElement("List_post")]
        public List<String> List_post { get; set; }
       

        [BsonElement("Waiting_List")]
        public List<String> Waiting_List { get; set; }

        [BsonElement("Followers")]
        public Follower Followers{ get; set; }

        [BsonElement("Followings")]
        public Following Followings { get; set; }

        [BsonElement("NewsFeed")]
        public NewsFeed NewsFeed { get; set; }

        public User()
        {
            _id = ObjectId.GenerateNewId().ToString();
            Followings = new Following();
            Followers = new Follower();
        }

        public Boolean PostPhoto(Post p,String userId)
        {
            DataAccess db = new DataAccess();

            // On recupere les infos de l'utilisateur qui publie le poste
            var result = GetMyProfile(userId);
            User u = JsonConvert.DeserializeObject<User>(result);

            // On ajoute l'id du post à la liste des posts de l'utilisateurs
            u.List_post.Add(p._id);
           
            // On fait la mise à jour au niveau de l'utilisateur
            var filter = Builders<User>.Filter.Eq("UserId", u.UserId);
            var update = Builders<User>.Update.Set(x => x.List_post, u.List_post);

            var result2 = db._db.GetCollection<User>("user").UpdateOne(filter,update);
          
            return db.Insert(p, "post");
        }

        public String GetPost(String urlphoto){
            
            DataAccess db = new DataAccess();

            // On recupere l'id du post correspondant à l'urlphoto
            var filter = Builders<Post>.Filter.Eq("UrlPhoto", urlphoto);
            var result = db._db.GetCollection<Post>("post").Find(filter).FirstOrDefault();

            return result.ToJson();
        }


        public Boolean DeletePost(String urlphoto,String userId)
        {
            DataAccess db = new DataAccess();

            // On recupere l'id du post correspondant à l'urlphoto
            var filter = Builders<Post>.Filter.Eq("UrlPhoto", urlphoto);
            var result = db._db.GetCollection<Post>("post").Find(filter).FirstOrDefault();
            if (result == null)
            {
                return false; // si le post n'existe pas
            }

            // Correction bug dû au Id dont le parsing est pas le meême
            JObject obj = JObject.Parse(result.ToJson());
            var postId = obj.SelectToken("_id").ToString();
            Console.WriteLine(postId);

            Post p = JsonConvert.DeserializeObject<Post>(result.ToJson());
            Console.WriteLine(result.ToJson());
            //



            // On va supprimer le post dans la liste de post de l'utilisateur
            var result2 = GetMyProfile(userId);
            User u = JsonConvert.DeserializeObject<User>(result2);

            if(u.List_post.Contains(postId)==true){
                u.List_post.Remove(postId);

                // On fait la mise à jour au niveau de l'utilisateur
                var filter2 = Builders<User>.Filter.Eq("UserId", u.UserId);
                var update = Builders<User>.Update.Set(x => x.List_post, u.List_post);
                var result3 = db._db.GetCollection<User>("user").UpdateOne(filter2, update);

                // On va supprimer le post dans sa collection
                var filter3 = Builders<Post>.Filter.Eq("UrlPhoto", urlphoto);
                var result4 = db._db.GetCollection<Post>("post").DeleteOne(filter3);

                return true;
            }

            else return false;
        }

        public String GetMyProfile(String userId)
        {
            DataAccess db = new DataAccess();
            var filter = Builders<User>.Filter.Eq("UserId", userId);


            var result = db._db.GetCollection<User>("user").Find<User>(filter).FirstOrDefault();
            return result.ToJson();
           
        }

        public String GetMyPhotos(String userId)
        {
            var result=GetMyProfile(userId);

            // On crée un objet user pour pouvoir récupérer les infors qu'on a bessoin
            User u = JsonConvert.DeserializeObject<User>(result);


            // On crée un json pr renvoyer dans le format voulu
            JObject j = JObject.Parse(@"{'MyPhotos': []}");
            JArray photos = (JArray)j["MyPhotos"];

            foreach(String post in u.List_post)
            {
                DataAccess db = new DataAccess();
                var filter = Builders<Post>.Filter.Eq("_id", post);
                var result2 = db._db.GetCollection<Post>("post").Find<Post>(filter).FirstOrDefault();
                Post p = JsonConvert.DeserializeObject<Post>(result2.ToJson());

                // list d'objets
                JObject j1 = new JObject(new JProperty("Lien", p.UrlPhoto));
                photos.Add(j1);
                //photos.Add(p.UrlPhoto); // List de urls 

            }
            return j.ToString();
        }

        public JObject GetWaitingList(String userId)
        {
            var result = GetMyProfile(userId);

            // On crée un objet user pour pouvoir récupérer les infors qu'on a bessoin
            User u = JsonConvert.DeserializeObject<User>(result);

            //// On crée un json pr renvoyer dans le format voulu
            JObject waitingList = JObject.Parse(@"{'MyWaitingList': []}");
            JArray j = (JArray)waitingList["MyWaitingList"];

            if (u.Waiting_List.Capacity == 0) return null;
            foreach (String user in u.Waiting_List)
            {
                DataAccess db = new DataAccess();

                // on va chercher dans la collection user 
                var filter = Builders<User>.Filter.Eq("UserId", user);
                var result2 = db._db.GetCollection<User>("user").Find<User>(filter).FirstOrDefault();
                User u1 = JsonConvert.DeserializeObject<User>(result2.ToJson());


                JObject j1 = new JObject(new JProperty("UserId", u1.UserId));
                JProperty jp1 = new JProperty("UrlPhoto", u1.UrlPhoto);
                j1.Add(jp1);
                j.Add(j1);

            }

            return waitingList;

        }

        public Boolean ModifyMyProfile(User modification, String userId){
            Boolean test = false;
            DataAccess db = new DataAccess();

            // On recupere les infos de l'utilisateur qui publie le poste
            var result = GetMyProfile(userId);
            User u = JsonConvert.DeserializeObject<User>(result);

            if (u.First_name.Equals(modification.First_name) == false)
            {
               var filter = Builders<User>.Filter.Eq("UserId", u.UserId);
               var update = Builders<User>.Update.Set(x => x.First_name, modification.First_name);
               var result2 = db._db.GetCollection<User>("user").UpdateOne(filter, update);
               test = true;
            }

            if (u.Last_name.Equals(modification.Last_name) == false)
            {
                var filter = Builders<User>.Filter.Eq("UserId", u.UserId);
                var update = Builders<User>.Update.Set(x => x.Last_name, modification.Last_name);
                var result2 = db._db.GetCollection<User>("user").UpdateOne(filter, update);
                test = true;
            }

            if (u.Password.Equals(modification.Password) == false)
            {
                var filter = Builders<User>.Filter.Eq("UserId", u.UserId);
                var update = Builders<User>.Update.Set(x => x.Password, modification.Password);
                var result2 = db._db.GetCollection<User>("user").UpdateOne(filter, update);
                test = true;
            }

            if (u.Email.Equals(modification.Email) == false)
            {
                var filter = Builders<User>.Filter.Eq("UserId", u.UserId);
                var update = Builders<User>.Update.Set(x => x.Email, modification.Email);
                var result2 = db._db.GetCollection<User>("user").UpdateOne(filter, update);
                test = true;
            }

            if (u.UrlPhoto.Equals(modification.UrlPhoto) == false)
            {
                var filter = Builders<User>.Filter.Eq("UserId", u.UserId);
                var update = Builders<User>.Update.Set(x => x.UrlPhoto, modification.UrlPhoto);
                var result2 = db._db.GetCollection<User>("user").UpdateOne(filter, update);
                test = true;
            }

            if (u.City.Equals(modification.City) == false)
            {
                var filter = Builders<User>.Filter.Eq("UserId", u.UserId);
                var update = Builders<User>.Update.Set(x => x.City, modification.City);
                var result2 = db._db.GetCollection<User>("user").UpdateOne(filter, update);
                test = true;
            }

            if (u.Country.Equals(modification.Country) == false)
            {
                var filter = Builders<User>.Filter.Eq("UserId", u.UserId);
                var update = Builders<User>.Update.Set(x => x.Country, modification.Country);
                var result2 = db._db.GetCollection<User>("user").UpdateOne(filter, update);
                test = true;
            }


            return test;
        }

        public JObject GetAllUsers(){
            DataAccess db = new DataAccess();

            //// On crée un json pr renvoyer dans le format voulu
            JObject listUsers = JObject.Parse(@"{'ListUsers': []}");
            JArray j = (JArray)listUsers["ListUsers"];

            try{
                // On parcours tte la collection d'user
                var filter = Builders<User>.Filter.Empty;
                foreach (User u in db._db.GetCollection<User>("user").Find(filter).ToListAsync().Result)
                {                    
                        JObject j1 = new JObject(new JProperty("First_Name", u.First_name));
                        JProperty jp1 = new JProperty("Last_Name", u.Last_name);
                        JProperty jp2 = new JProperty("UrlPhoto", u.UrlPhoto);
                        JProperty jp3 = new JProperty("UserId", u.UserId);
                        j1.Add(jp1);
                        j1.Add(jp2);
                        j1.Add(jp3);
                        j.Add(j1);
                } 
                return listUsers;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }


        public JObject GetAvailableUsers(String myUserId){
            DataAccess db = new DataAccess();
            var result = GetMyProfile(myUserId);

            // On crée un objet user pour pouvoir récupérer les infors qu'on a bessoin
            User u = JsonConvert.DeserializeObject<User>(result);

            // On crée une liste avec  les personnes de demande envoyé et mes abonnements
            List<String> unavailableUsers = new List<String>();
            foreach (String user in u.Followings.RequestSendList)
            {
                unavailableUsers.Add(user);
            }

            foreach(String user in u.Followings.ListUsers)
            {
                unavailableUsers.Add(user);
            }

            //// On crée un json pr renvoyer dans le format voulu
            JObject listUsers = JObject.Parse(@"{'ListUsers': []}");
            JArray j = (JArray)listUsers["ListUsers"];

            try
            {
                // On parcours tte la collection d'user
                var filter = Builders<User>.Filter.Empty;
                foreach (User u1 in db._db.GetCollection<User>("user").Find(filter).ToListAsync().Result)
                {
                    // Si un des users est déja présent on l'ajoute pas et on ajoute pas l'utisateur lui même
                    if (unavailableUsers.Contains(u1.UserId) == false && u1.UserId != myUserId)
                    {
                        JObject j1 = new JObject(new JProperty("First_Name", u1.First_name));
                        JProperty jp1 = new JProperty("Last_Name", u1.Last_name);
                        JProperty jp2 = new JProperty("UrlPhoto", u1.UrlPhoto);
                        JProperty jp3 = new JProperty("UserId", u1.UserId);
                        j1.Add(jp1);
                        j1.Add(jp2);
                        j1.Add(jp3);
                        j.Add(j1);
                    }
                }
                return listUsers;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }


        }

        public String GetUserProfile(String urlPhoto)
        {
            DataAccess db = new DataAccess();

            try
            {
                var filter = Builders<User>.Filter.Eq("UrlPhoto", urlPhoto);
                var fieldsBuilder = Builders<User>.Projection;
                var fields = fieldsBuilder.Exclude(d => d.Password);

                var result = db._db.GetCollection<User>("user").Find<User>(filter).Project(fields).FirstOrDefault();
                return result.ToJson();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

        }
    }


        

}
