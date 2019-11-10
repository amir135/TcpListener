using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpExample.DataAccessCore;
using TcpExample.Domain.Common;
using TcpExample.Domain.DBModel;

namespace TcpExample.Services.DataCollectionService
{
    public class DataCollectionService : IDataCollectionService
    {
        ITcpExampleDBContext db;
        public DataCollectionService(ITcpExampleDBContext tcpExampleDBContext)
        {
            db = tcpExampleDBContext;
        }
        public Result<int> getDataCollectionCount()
        {
            Result<int> result = new Result<int>();
            try
            {
                var count = db.DataCollections.Where(a => !a.IsDeleted).Count();

                result.Success = true;
                result.Data = count;
            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
            }
            return result;
        }
        public Result<DataCollection> AddData(DataCollection data)
        {
            Result<DataCollection> result = new Result<DataCollection>();
            try
            {
                var entity = db.DataCollections.Add(data);
                db.SaveChanges();
                result.Success = true;
                result.Data = entity.Entity;
            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
            }
            return result;
        }
        public Result<List<DataCollection>> GetData(Device data)
        {
            Result< List < DataCollection> > result = new Result<List<DataCollection>>();
            try
            {
                var collection = db.DataCollections.Where(a=>a.Device==data).OrderByDescending(a=>a.DateCreated).ToList();
                result.Data = collection;
                result.Success = true;

            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
            }
            return result;
        }
    }
}
