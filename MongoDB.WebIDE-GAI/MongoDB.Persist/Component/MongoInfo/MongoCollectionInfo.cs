using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Defination;
using MongoDB.Driver;
using MongoDB.Model;
using System.Collections;
//using MongoDB.Defination;
//using MongoDB.Driver;
//using MongoDB.Model;

namespace MongoDB.Component
{
    public class MongoCollectionInfo : MongoBaseInfo
    {
        public MongoCollectionInfo(uint id)
        {
            var tbNode = MongoCache.GetTreeNode(id);
            Table = MongoCache.GetMongoObject(id) as MongoCollectionModel;
            var dbNode = MongoCache.GetTreeNode(tbNode.PID);
            Database = MongoCache.GetMongoObject(dbNode.ID) as MongoDatabaseModel;
            var serverNode = MongoCache.GetTreeNode(dbNode.PID);
            Server = MongoCache.GetMongoObject(serverNode.ID) as MongoServerModel;
        }

        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <returns></returns>
        public override List<MongoTreeNode> GetInfo()
        {
            //kuki改20161116
            //var mongo = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            //var server = mongo.GetServer();
            var server = new MongoClient(string.Format(MongoConst.ConnString, Server.Name));
            var db = server.GetDatabase(Database.Name);
            //var rst = db.RunCommand(new CommandDocument { { "collstats",Table.Name } });
            var command = new CommandDocument("collstats", Table.Name);
            var rst = db.RunCommand<CommandResult>(command);

            var list = new List<MongoTreeNode>();
            
            if (rst.Ok)
            {
                BuildTreeNode(list, 0, rst.Response);
            }
            return list;
        }
    }
}