﻿using System;
using MongoDB.Bson;
using MongoDB.Driver;
//using MongoDB.Driver.Builders;
using System.Collections.Generic;

using System.Net.Mail;
using System.Net;
using projet.Wall;

namespace projet.Profile
{
    public class DataAccess 
    {
        MongoClient _client;
        IMongoDatabase _db;

        public DataAccess()
        {
            _client = new MongoClient("mongodb://localhost:27017");
            _db=_client.GetDatabase("Instalite");
        }


        // Inscrire un nouveau utilisateur
        public void Inscription(User u)
        {
            _db.GetCollection<User>("user").InsertOne(new User
            {
                UserId = u.UserId,
                Password = u.Password,
                First_name = u.First_name,
                Last_name = u.Last_name,
                Gender = u.Gender,
                Email = u.Email,
                My_photo= u.My_photo,
                Birth_date = u.Birth_date,
                City = u.City,
                Country = u.Country,
            });
            
        }

        // Test si l'id est déjà prit ou pas
        public Boolean IsIdUsed(String Id)
        {

            var filter = Builders<User>.Filter.Eq("UserId", Id);
            Boolean result = _db.GetCollection<User>("user").Find(filter).Any();
            return result;
        }

        // Ajoute photo et renvoie son id
        public String AddPhoto(Byte[] a)
        {
            Photo photo = new Photo();
            photo.Image = a;
            _db.GetCollection<Photo>("photo").InsertOne(photo);
            var id = photo.Id.ToString();
            return id;

        }

        //Test si le mot de passe est valide
        public Boolean ValidePassword(String Id,String Password)
        {

            var filterId = Builders<User>.Filter.Eq("UserId", Id);
            var filterPassword = Builders<User>.Filter.Eq("Password", Password);
            var filter = filterId & filterPassword;
            Boolean result = _db.GetCollection<User>("user").Find(filter).Any();

            return result;
        }


        // Test retour brute
        public List<User> GetAllUsers()
        {
            List<User> allUsers = new List<User>();

            var result=_db.GetCollection<User>("user").Find(new BsonDocument()).ToEnumerable();
             foreach (var document in result)
            {
                
                Console.WriteLine(document);
               allUsers.Add(document);
            }

            return allUsers;
        }

        // Test retour simplifié
        public String GetAllUsers2()
        {
            List<User> allUsers = new List<User>();
            String a="";
            //var result=_db.GetCollection<User>("user").Find(new BsonDocument());

            var projection = Builders<BsonDocument>.Projection.Include("UserId").Include("Password").Exclude("_id");
            var result = _db.GetCollection<BsonDocument>("user").Find(new BsonDocument()).Project(projection).ToEnumerable();
            Console.WriteLine(result.ToJson());
            a = result.ToJson();


            return a;
        }

        public void SendMail(String Message)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("inkaran.thuraiyappah@gmail.com", "password");

            MailMessage mm = new MailMessage("inkaran.thuraiyappah@gmail.com", "inkaran.thuraiyappah@gmail.com");

            client.Send(mm);
        }



    }
}