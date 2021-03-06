﻿using MongoDB.Driver;
using MongoDB.Model;
using MongoDB.Defination;
using System.Collections.Generic;
using System;
using System.Linq;
using MongoDB.Bson;

namespace MongoDB.Component
{
    public class MongoDataContext : MongoBase
    {
        public MongoDataContext(uint id)
        {
            ID = id;
            var fieldNode = MongoCache.GetTreeNode(ID);

            var tbNode = MongoCache.GetTreeNode(fieldNode.PID);
            Table = MongoCache.GetMongoObject(tbNode.ID) as MongoCollectionModel;
            var dbNode = MongoCache.GetTreeNode(tbNode.PID);
            Database = MongoCache.GetMongoObject(dbNode.ID) as MongoDatabaseModel;
            var serverNode = MongoCache.GetTreeNode(dbNode.PID);
            Server = MongoCache.GetMongoObject(serverNode.ID) as MongoServerModel;
        }

        public List<MongoFieldModel> GetFields()
        {
            var fieldNodes = MongoCache.GetTreeNodes().Where(n => n.PID == ID).ToList();

            var list = new List<MongoFieldModel>();
            foreach (var node in fieldNodes)
            {
                list.Add(MongoCache.GetMongoObject(node.ID) as MongoFieldModel);
            }
            return list;
        }

        public List<BsonDocument> GetData(string jsonfind, string jsonsort, int skip, int limit)
        {

            //kuki改20161116
            //var mongo = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            //var server = mongo.GetServer();                                                    
            var server = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));            
            var db = server.GetDatabase(Database.Name);                        
            
            var findDoc = string.IsNullOrEmpty(jsonfind) ? new QueryDocument() : new QueryDocument(BsonDocument.Parse(jsonfind));
            var sortDoc = string.IsNullOrEmpty(jsonsort) ? new SortByDocument() : new SortByDocument(BsonDocument.Parse(jsonsort));
            //var query = db.GetCollection(Table.Name).Find(findDoc);                 

            //query.SetSortOrder(sortDoc);                        

            List<BsonDocument> query=new List<BsonDocument>();
            if (skip > 0)
            {
                query = db.GetCollection<BsonDocument>(Table.Name).Find(findDoc).Skip(skip).Sort(sortDoc).ToList();
                //query.Skip = skip;
                //query.Skip(skip);                
            }
            if (limit > 0)
            {
                query = db.GetCollection<BsonDocument>(Table.Name).Find(findDoc).Limit(limit).ToList();
                //var query.Limit(limit);
                //query.Limit = limit;
                
            }
            
            return query;
        }

        public List<MongoTreeNode> Explain(string jsonfind, string jsonsort)
        {
            //kuki改20161116
            //var mongo = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            //var server = mongo.GetServer();
            //var db = server.GetDatabase(Database.Name);
            var server = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            var db = server.GetDatabase(Database.Name);


            var findDoc = string.IsNullOrEmpty(jsonfind) ? new QueryDocument() : new QueryDocument(BsonDocument.Parse(jsonfind));
            var sortDoc = string.IsNullOrEmpty(jsonsort) ? new SortByDocument() : new SortByDocument(BsonDocument.Parse(jsonsort));
            //var query = db.GetCollection(Table.Name).Find(findDoc);
            //query.SetSortOrder(sortDoc);
            
            var query = db.GetCollection<BsonDocument>(Table.Name).Find(findDoc).Sort(sortDoc).ToBsonDocument();
                
            //var doc = query.Explain(true);

            var list = new List<MongoTreeNode>();
            //BuildTreeNode(list, 0, doc);
            BuildTreeNode(list, 0, query);
            return list;
        }
    }
}