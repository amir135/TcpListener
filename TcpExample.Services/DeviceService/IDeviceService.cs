using System;
using System.Collections.Generic;
using System.Text;
using TcpExample.Domain;
using TcpExample.Domain.Common;
using TcpExample.Domain.DBModel;

namespace TcpExample.Services.DeviceService
{
    public interface IDeviceService
    {
        Result<int> getDeviceCount();
        Result<Device> AddDevice(Device device);
        Result<Device> EditDevice(Device device);
        Result<Device> RemoveDevice(Device device);
        Result<Device> GetDevice(string markCode, long serialNumber);
        Result< List<Device>> GetAllDevice(bool withDeactiv = false);
        Result<List<Device>> GetAllDevice(string markCode,bool withDeactiv = false);
        Result<List<DeviceWithLatestDataCollectionViewModel>> GetAllDeviceWithLatestSignal(string markCode, bool withDeactiv = false);
    }
}
