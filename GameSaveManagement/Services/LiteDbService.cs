using GameSaveManagement.Model;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSaveManagement.Services
{
    public class LiteDbService
    {
        private string _dbName = "game.db";
        private ConnectionString _connection;
        public LiteDbService()
        {
            var path = System.IO.Path.Combine(Environment.CurrentDirectory, _dbName);
            _connection = new ConnectionString
            {
                Filename = path,
            };
            using (var db = new LiteDatabase(_connection))
            {
                var col = db.GetCollection<GameModel>();
            }
        }

        public IEnumerable<GameModel> GetServers(string serverName)
        {
            using (var db = new LiteDatabase(_connection))
            {
                var col = db.GetCollection<GameModel>();
                return col.Find(p => p.GameName.Contains(serverName)).ToList();
            }
        }

        public int Insert(GameModel model)
        {
            using (var db = new LiteDatabase(_connection))
            {
                var col = db.GetCollection<GameModel>();
                return col.Insert(model);
            }
        }

        public bool Update(GameModel model)
        {
            using (var db = new LiteDatabase(_connection))
            {
                var col = db.GetCollection<GameModel>();
                if (col.Exists(p => p.Id == model.Id))
                {
                    return col.Update(model);
                }
                return false;
            }
        }

        public bool Delete(GameModel model)
        {
            using (var db = new LiteDatabase(_connection))
            {
                var col = db.GetCollection<GameModel>();
                if (col.Exists(p => p.Id == model.Id))
                {
                    return col.Delete(model.Id);
                }
                return false;
            }
        }

        public IEnumerable<GameModel> GetAllGames()
        {
            using (var db = new LiteDatabase(_connection))
            {
                return db.GetCollection<GameModel>().FindAll().ToList();
            }
        }

        public bool Exists(GameModel model)
        {
            using (var db = new LiteDatabase(_connection))
            {
                var col = db.GetCollection<GameModel>();
                if (col.Exists(p => p.Id == model.Id))
                {
                    return true;
                }
            }
            return false;
        }

        public GameModel GetModelById(int modelId)
        {
            using (var db = new LiteDatabase(_connection))
            {
                var col = db.GetCollection<GameModel>();
                return col.FindById(modelId);
            }
        }
    }
}
