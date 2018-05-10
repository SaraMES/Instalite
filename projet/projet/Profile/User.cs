﻿using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace projet.Profile
{
    public class User
    {
        public ObjectId Id { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonElement("Password")]
        public string Password { get; set; }

        [BsonElement("First_name")]
        public string First_name { get; set; }

        [BsonElement("Last_name")]
        public string Last_name { get; set; }

        [BsonElement("Birth_date")]
        public DateTime Birth_date { get; set; }


        [BsonElement("Gender")]
        public string Gender { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        // String car on met l'id de la photo
        [BsonElement("My_photo")]
        public string My_photo { get; set; }


        [BsonElement("City")]
        public string City { get; set; }

        [BsonElement("Country")]
        public string Country { get; set; }

        // List string des Id des post ou list des post ?
        [BsonElement("List_post")]
        public List<String> List_post { get; set; }
       
        /*
        [BsonElement("Waiting_List")]
        public List<String> ou List<User> Waiting_List { get; set; }
        */

    }


        

}