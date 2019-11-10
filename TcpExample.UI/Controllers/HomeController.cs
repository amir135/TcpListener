
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TcpExample.Domain.Common;
using TcpExample.Domain.Common.UserService;
using TcpExample.Domain.ViewModel;
using TcpExample.Services.DataCollectionService;
using TcpExample.Services.DeviceService;
using TcpExample.Services.MarkService;

namespace TcpExample.UI.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {


        IMarkService _markService;
        IDeviceService _deviceService;
        IDataCollectionService _dataCollectionService;
        IUserService _userService;
        public HomeController(IUserService userService,IMarkService markService,IDeviceService deviceService, IDataCollectionService dataCollectionService)
        {
            _markService = markService;
            _deviceService = deviceService;
            _dataCollectionService = dataCollectionService;
            _userService = userService;
        }

        public IActionResult Index()
        {
           ViewBag.MarkCount= _markService.getMarkCount();
            ViewBag.DeviceCount=_deviceService.getDeviceCount();
            ViewBag.DataCollectionCount=_dataCollectionService.getDataCollectionCount();
            ViewBag.UserCount=_userService.getUserCount();
            return View();
        }

        public IActionResult Mark() {
            var model= _markService.GetAllMark(true);
            return View(model.Data);
        }
        [HttpPost]
        public JsonResult DeleteMark([FromBody]GlobalViewModel model)
        {
            Result result = new Result();
            var mark = _markService.GetMark(model.MarkCode);
            if (mark.Success)
            {
                if (mark.Data.Devices.Where(a => a.IsDeleted == false).Count() > 0)
                {
                    result.Success = false;
                    result.Message = "This mark have some Devices! You first must delete devices";
                }
                else
                {
                    var deleteResult = _markService.RemoveMark(mark.Data);
                    if (deleteResult.Success)
                    {
                        result.Success = true;
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = deleteResult.Message;
                    }
                }
            }
            else {
                result.Success = false;
                result.Message = "Mark is not exists";
            }
            return new JsonResult(result);
        }

        [HttpPost]
        public JsonResult GetAllMark()
        {
            Result result = new Result();
            var model = _markService.GetAllMark(false);

            return new JsonResult(model);
        }

        [HttpPost]
        public JsonResult AddMark([FromBody]GlobalViewModel model)
        {
            Result< TcpExample.Domain.DBModel.Mark> result = new Result<TcpExample.Domain.DBModel.Mark>();

            if (model.MarkCode != null && model.MarkCode.Length == 1 && model.MarkName != null && model.MarkName.Length > 0)
            {
                var mark = _markService.GetMark(model.MarkCode);
                if (mark.Data == null)
                {
                    TcpExample.Domain.DBModel.Mark newMark = new Domain.DBModel.Mark() { 
                        Code=model.MarkCode,
                        Name=model.MarkName
                    };
                    var addResult=_markService.AddMark(newMark);
                    return new JsonResult(addResult);

                }
                else {
                    result.Success = false;
                    result.Message = "This code is exists !!";
                }
            }
            else
            {
                result.Success = false;
                result.Message = "Invalid data !!";
            }
            return new JsonResult(result);
        }



        [HttpPost]
        public JsonResult EditMark([FromBody]GlobalViewModel model)
        {
            Result<TcpExample.Domain.DBModel.Mark> result = new Result<TcpExample.Domain.DBModel.Mark>();

            if (model.MarkCode != null && model.MarkCode.Length == 1 && model.MarkName != null && model.MarkName.Length > 0)
            {
                var mark = _markService.GetMark(model.MarkCode);
                if (mark.Data != null)
                {

                    mark.Data.Name = model.MarkName;
                    
                    var addResult = _markService.EditMark(mark.Data);
                    return new JsonResult(addResult);

                }
                else
                {
                    result.Success = false;
                    result.Message = "This mark is not exists !!";
                }
            }
            else
            {
                result.Success = false;
                result.Message = "Invalid data !!";
            }
            return new JsonResult(result);
        }




        public IActionResult Device(string markCode)
        {
            ViewBag.Mark = _markService.GetMark( markCode);
            var model = _deviceService.GetAllDeviceWithLatestSignal(markCode,true);
            
            return View(model.Data);
        }
        [HttpPost]
        public JsonResult DeleteDevice([FromBody]GlobalViewModel model)
        {
            Result result = new Result();
            long serialnumber;
            if (long.TryParse(model.SerialNumber, out serialnumber))
            {
                var device = _deviceService.GetDevice(model.MarkCode, serialnumber);
                if (device.Success)
                {

                    var deleteResult = _deviceService.RemoveDevice(device.Data);
                    if (deleteResult.Success)
                    {
                        result.Success = true;
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = deleteResult.Message;
                    }

                }
                else
                {
                    result.Success = false;
                    result.Message = "Device is not exists";
                }
            }
            else {
                result.Success = false;
                result.Message = "Invalid data";
            }
            return new JsonResult(result);
        }


        [HttpPost]
        public JsonResult AddDevice([FromBody]GlobalViewModel model)
        {
            Result<TcpExample.Domain.DBModel.Device> result = new Result<TcpExample.Domain.DBModel.Device>();
            long serialnumber;
            if (
                model.MarkCode != null && model.MarkCode.Length == 1 &&
                model.SerialNumber != null && long.TryParse(model.SerialNumber, out serialnumber) && serialnumber > 0 &&
                model.IpAddress!=null && model.IpAddress.Length > 0 &&
                model.Port != null && model.Port.Length > 0)
            {
                var mark = _markService.GetMark(model.MarkCode);
                if (mark.Data != null)
                {
                    TcpExample.Domain.DBModel.Device newDevice= new Domain.DBModel.Device()
                    {
                        SerialNumber=serialnumber,
                        MarkCode = model.MarkCode,
                        IpAddress = model.IpAddress,
                        Port=model.Port
                    };
                    var addResult = _deviceService.AddDevice(newDevice);
                    return new JsonResult(addResult);

                }
                else
                {
                    result.Success = false;
                    result.Message = "Slected MArk is not exists !!";
                }
            }
            else
            {
                result.Success = false;
                result.Message = "Invalid data !!";
            }
            return new JsonResult(result);
        }



        [HttpPost]
        public JsonResult EditDevice([FromBody]GlobalViewModel model)
        {
            Result<TcpExample.Domain.DBModel.Device> result = new Result<TcpExample.Domain.DBModel.Device>();
            long serialnumber;
            if (
                model.MarkCode != null && model.MarkCode.Length == 1 &&
                model.SerialNumber != null && long.TryParse(model.SerialNumber, out serialnumber) && serialnumber > 0 &&
                model.IpAddress != null && model.IpAddress.Length > 0 &&
                model.Port != null && model.Port.Length > 0)
            {
                var device = _deviceService.GetDevice(model.MarkCode,serialnumber);
                if (device.Data != null)
                {

                    device.Data.IpAddress = model.IpAddress;
                    device.Data.Port = model.Port;
                    var addResult = _deviceService.EditDevice(device.Data);
                    return new JsonResult(addResult);

                }
                else
                {
                    result.Success = false;
                    result.Message = "This mark is not exists !!";
                }
            }
            else
            {
                result.Success = false;
                result.Message = "Invalid data !!";
            }
            return new JsonResult(result);
        }


        public IActionResult OnlineTrack(string MarkCode, long SerialNumber)
        {

            var device=_deviceService.GetDevice(MarkCode,SerialNumber);
            ViewBag.Device = device.Data;
            var model = _dataCollectionService.GetData(device.Data);
            if (device.Success)
                return View(model.Data);
            else
                return RedirectToAction("index");
        }
    }
}
