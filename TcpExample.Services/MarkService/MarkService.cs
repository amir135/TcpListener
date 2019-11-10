using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpExample.DataAccessCore;
using TcpExample.Domain.Common;
using TcpExample.Domain.DBModel;

namespace TcpExample.Services.MarkService
{
    public class MarkService : IMarkService
    {
        ITcpExampleDBContext db;
        public MarkService(ITcpExampleDBContext tcpExampleDBContext)
        {
            db = tcpExampleDBContext;
        }
        public Result<int> getMarkCount()
        {
            Result<int> result = new Result<int>();
            try
            {
                var count = db.Marks.Where(a=>!a.IsDeleted).Count();
               
                result.Success = true;
                result.Data =count;
            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
            }
            return result;
        }
        public Result<Mark> AddMark(Mark mark)
        {
            Result<Mark> result = new Result<Mark>();
            try {
                var entity= db.Marks.Add(mark);
                db.SaveChanges();
                result.Success = true;
                result.Data = entity.Entity;
            }
            catch (Exception err) {
                result.Success = false;
                result.Message=err.Message;
            }
            return result;
        }

        public Result<Mark> EditMark(Mark mark)
        {
            Result<Mark> result = new Result<Mark>();
            try {
                var entity = db.Marks.Update(mark);
                db.SaveChanges();
                result.Success = true;
                result.Data = entity.Entity;
            }
            catch (Exception err) {
                result.Success = false;
                result.Message = err.Message;
            }
            return result;
        }

        public Result<List<Mark>> GetAllMark(bool withDeactiv=false)
        {
            Result<List<Mark>> result = new Result<List<Mark>>();
            try {
                if(withDeactiv)
                    result.Data = db.Marks.Include(a => a.Devices).ToList();
                else
                    result.Data = db.Marks.Include(a => a.Devices).Where(a=> !a.IsDeleted).ToList();
                result.Success = true;
            }
            catch (Exception err) {
                result.Success = false;
                result.Message = err.Message;
            }
            return result;
        }
       
        public Result<List<Mark>> GetAllMarkWithDevice(bool withDeactiv = false)
        {
            Result<List<Mark>> result = new Result<List<Mark>>();
            try {
                if (withDeactiv)
                    result.Data = db.Marks.Include(a=>a.Devices).ToList();
                else
                    result.Data = db.Marks.Include(a => a.Devices).Where(a => !a.IsDeleted).ToList();
                result.Success = true;
            }
            catch (Exception err) {
                result.Success = false;
                result.Message = err.Message;
            }
            return result;
        }

        public Result<Mark> GetMark(string markCode)
        {
            Result<Mark> result = new Result<Mark>();
            try {
                result.Data = db.Marks.Include(a => a.Devices).FirstOrDefault(a=>a.Code==markCode);
                result.Success = true;
            }
            catch (Exception err) {
                result.Success = false;
                result.Message = err.Message;
            }
            return result;
        }

        public Result<Mark> RemoveMark(Mark mark)
        {
            Result<Mark> result = new Result<Mark>();
            try {
                mark.IsDeleted = true;
                var entity = db.Marks.Update(mark);
                db.SaveChanges();
                result.Success = true;
                result.Data = entity.Entity;
            }
            catch (Exception err) {
                result.Success = false;
                result.Message = err.Message;
            }
            return result;
        }


    }
}
