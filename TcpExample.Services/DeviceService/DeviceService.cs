using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpExample.DataAccessCore;
using TcpExample.Domain;
using TcpExample.Domain.Common;
using TcpExample.Domain.DBModel;

namespace TcpExample.Services.DeviceService
{
    public class DeviceService : IDeviceService
    {
        ITcpExampleDBContext db;
        public DeviceService(ITcpExampleDBContext tcpExampleDBContext)
        {
            db = tcpExampleDBContext;
        }

        public Result<Device> AddDevice(Device device)
        {
            Result<Device> result = new Result<Device>();
            try
            {
                var entity = db.Devices.Add(device);
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

        public Result<Device> EditDevice(Device device)
        {
            Result<Device> result = new Result<Device>();
            try
            {
                var entity = db.Devices.Update(device);
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

        public Result<List<Device>> GetAllDevice(bool withDeactiv = false)
        {
            Result<List<Device>> result = new Result<List<Device>>();
            try
            {
                if (withDeactiv)
                    result.Data = db.Devices.ToList();
                else
                    result.Data = db.Devices.Where(a => !a.IsDeleted).ToList();
                result.Success = true;
               
            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
            }
            return result;
        }

        public Result<List<Device>> GetAllDevice(string markCode, bool withDeactiv = false)
        {
            Result<List<Device>> result = new Result<List<Device>>();
            try
            {
                if (withDeactiv)
                    result.Data = db.Devices.Include(a=>a.Mark).Where(a=>a.MarkCode==markCode).ToList();
                else
                    result.Data = db.Devices.Include(a => a.Mark).Where(a => a.MarkCode == markCode && !a.IsDeleted).ToList();
                result.Success = true;

            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
            }
            return result;
        }

        public Result<List<DeviceWithLatestDataCollectionViewModel>> GetAllDeviceWithLatestSignal(string markCode, bool withDeactiv = false)
        {
            Result<List<DeviceWithLatestDataCollectionViewModel>> result = new Result<List<DeviceWithLatestDataCollectionViewModel>>();
            try
            {
                if (withDeactiv)
                    result.Data = db.Devices.Include(a=>a.Mark).Include(b=>b.DataCollections).Where(a=>a.MarkCode== markCode).Select(c=>new DeviceWithLatestDataCollectionViewModel
                     {
                         Mark = c.Mark,
                         IpAddress = c.IpAddress,
                         Port = c.Port,
                         SerialNumber=c.SerialNumber,
                        IsDelete = c.IsDeleted,
                         DataCollection = c.DataCollections
                       .OrderByDescending(p => p.DateCreated).Take(1).FirstOrDefault()
                     }).ToList();
                else
                    result.Data = db.Devices.Include(a => a.Mark).Include(b => b.DataCollections).Where(a => a.MarkCode == markCode && a.IsDeleted==false).Select(c => new DeviceWithLatestDataCollectionViewModel
                    {
                        Mark = c.Mark,
                        IpAddress = c.IpAddress,
                        Port = c.Port,
                        SerialNumber = c.SerialNumber,
                        IsDelete= c.IsDeleted,
                        DataCollection = c.DataCollections
                          .OrderByDescending(p => p.DateCreated).Take(1).FirstOrDefault()
                    }).ToList(); ;
                result.Success = true;

            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
            }
            return result;
        }

        public Result<Device> GetDevice(string markCode, long serialNumber)
        {
            Result<Device> result = new Result<Device>();
            try
            {
                result.Data = db.Devices.Include(a => a.Mark).Include(b => b.DataCollections).FirstOrDefault(a=>a.SerialNumber==serialNumber && a.MarkCode==markCode); ;
            result.Success = true;
            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
            }
            return result;
        }

        public Result<int> getDeviceCount()
        {
            Result<int> result = new Result<int>();
            try
            {
                var count = db.Devices.Where(a => !a.IsDeleted).Count();

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

        public Result<Device> RemoveDevice(Device device)
        {
            Result<Device> result = new Result<Device>();
            try
            {
                device.IsDeleted = true;
                var entity = db.Devices.Update(device);
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
    }
}
