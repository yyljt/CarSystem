﻿using System;
using System.Threading;
using Commons;
using Entitys;
using ServiceStack.OrmLite;
using System.Collections.Generic;

namespace DataBase
{
   public class CarDetialInfoDb : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;
        public CarDetialInfoDb()
        {
            _dbFactory = new OrmLiteConnectionFactory(ZnmDbConnectionString, SqlServerDialect.Provider);
            // _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<CarDetialInfo>();
            }
        }

        public void AddCarinfo(CarDetialInfo item)
        {
            int error = 0;
            do
            {
                try
                {
                    using (var db = _dbFactory.OpenDbConnection())
                    {
                        if (item.Id == 0)
                        {
                            item.CreateTime = DateTime.Now;

                            db.Insert(item);
                        }
                        else
                        {
                            //db.SingleById<CarDetialInfo>(item.Id);
                            db.Update<CarDetialInfo>(item);
                        }
              
                    }
                    break;
                }
                catch (Exception ex1)
                {
                    error++;
                    Thread.Sleep(10000);
                    LogServer.WriteLog(ex1.Message, "DBError");
                }
            } while (error < 4);

        }
        public CarDetialInfo GetCarinfo(int id)
        {
            if (id == 0)
                return null;
            int error = 0;
            do
            {
                try
                {
                    using (var db = _dbFactory.OpenDbConnection())
                    {
                      return  db.SingleById<CarDetialInfo>(id);
                    }
                }
                catch (Exception ex1)
                {
                    error++;
                    Thread.Sleep(10000);
                    LogServer.WriteLog(ex1.Message, "DBError");
                }
            } while (error < 4);
            return null;

        }

        public List<CarDetialInfo> GetCarLIst(string Num)
        {
            int error = 0;
            do
            {
                try
                {
                    using (var db = _dbFactory.OpenDbConnection())
                    {
                        return db.Select<CarDetialInfo>(c=>c.SellerNumber==Num);
                    }
                }
                catch (Exception ex1)
                {
                    error++;
                    Thread.Sleep(10000);
                    LogServer.WriteLog(ex1.Message, "DBError");
                }
            } while (error < 4);
            return null;
        }
    }
}
