using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TcpExample.DataAccessCore;
using TcpExample.Domain.DBModel;
using TcpExample.Domain.ViewModel;
using TcpExample.Services.DataCollectionService;
using TcpExample.Services.DeviceService;
using TcpExample.Services.MarkService;

namespace TcpListener
{
    class Program
    {
        const int PORT_NO = 8091;
        const string SERVER_IP = "127.0.0.1";
        static IMarkService  markService;
        static IDeviceService deviceService;
        static IDataCollectionService dataCollectionService;
        static HubConnection connection;
        static void Main(string[] args)
        {
            ConnctToHubAsync().Wait();


            InitializeApplication();
 
            // start listener
            StartServer();
        }
        static async Task<bool> ConnctToHubAsync()
        {
            try
            {
                connection = new HubConnectionBuilder()
                         .WithUrl("http://localhost:33111/DataHub")
                         .Build();
                connection.Closed += async (error) =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await connection.StartAsync();
                   
                };
                try
                {
                    await connection.StartAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
               
            }
            catch {
                return false;
            }
        }
        private static void InitializeApplication()
        {
            // Detecte appsetting.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            
            IConfigurationRoot configuration = builder.Build();

            // get connection string from json file
            var connectionString = configuration.GetConnectionString("db");

            // create builder option object for initialize EntityFrameWork
            var optionsBuilder = new DbContextOptionsBuilder<TcpExampleDBContext>();
            optionsBuilder.UseSqlServer(connectionString);


            var serviceProvider = new ServiceCollection().AddLogging()
                .AddSingleton<IMarkService, MarkService>()
                .AddSingleton<IDeviceService, DeviceService>()
                .AddSingleton<IDataCollectionService, DataCollectionService>()

                .AddSingleton<ITcpExampleDBContext>(provider => new TcpExampleDBContext(optionsBuilder.Options))
                .BuildServiceProvider();

            // get instanse of services
            markService = serviceProvider.GetService<IMarkService>();
            deviceService = serviceProvider.GetService<IDeviceService>();
            dataCollectionService = serviceProvider.GetService<IDataCollectionService>();
        }

        public static void StartServer()
        {
            
            IPAddress ipAddress = IPAddress.Parse(SERVER_IP);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, PORT_NO);
            Socket listener=null;
            Socket handler = null;
            try
            { 
                listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(10);
                Console.WriteLine("Waiting for a connection...");
               // = listener.Accept();

                byte[] bytes = null;

                while (true)
                {
                    bytes = new byte[2048];
                    handler = listener.Accept();
                    
                    int bytesRec = handler.Receive(bytes);
                    if (bytes[0] == 2)
                    {
                        string hexLength = bytes[2].ToString("X2") + bytes[1].ToString("X2");
                        StringBuilder hex = new StringBuilder(bytes.Length * 2);
                        int length = int.Parse(hexLength, System.Globalization.NumberStyles.HexNumber);
                        if (bytes[length + 2] == 3)
                        {
                            string serialNumber = "";
                            string mark = "";
                            string signal = "";
                            string senderIpAdress = handler.LocalEndPoint.ToString().Split(":")[0];
                            string senderPortNumber = handler.LocalEndPoint.ToString().Split(":")[1];
                            for (int i = 3; i < length + 2; i++)
                            {
                                if (i == 3)
                                {
                                    mark = System.Convert.ToChar(bytes[i]).ToString();
                                    continue;
                                }
                                if (i >= length )
                                {
                                    signal += System.Convert.ToChar(bytes[i]).ToString();
                                    continue;
                                }
                                serialNumber += System.Convert.ToChar(bytes[i]).ToString();
                            }
                            DeviceDataRecive newData = null;
                            try
                            {
                                newData = new DeviceDataRecive()
                                {
                                    MarkCode = mark,
                                    DateTime = DateTime.Now,
                                    SerialNumber = long.Parse(serialNumber),
                                    Signal = int.Parse(signal),
                                    IpAddress=senderIpAdress,
                                    Port=senderPortNumber
                                };
                            }
                            catch { 
                                // todo log wrong format of message
                            }
                            if (newData != null)
                            {
                                StoreDataAsync(newData);
                            }
                            //todo save in data base
                            Console.WriteLine("Text received : {0}\nMark : {1}\nSerial : {2}\nSignal : {3} From : {4}", DateTime.Now,mark,serialNumber,signal,senderIpAdress);
                            DateTime utcNow = DateTime.UtcNow;
                            long utcNowAsLong = utcNow.ToBinary();
                            byte[] utcNowBytes = BitConverter.GetBytes(utcNowAsLong);

                            List<byte> returnMessage = new List<byte>();
                            // start byte
                            returnMessage.Add(2);
                            // message length bytes
                            byte[] messageLength = BitConverter.GetBytes((Int16)utcNowBytes.Length);
                           //if (BitConverter.IsLittleEndian)
                           //     Array.Reverse(messageLength);
                            returnMessage.AddRange(messageLength);
                            // timestam bytes
                            returnMessage.AddRange(utcNowBytes);
                            //finish bytes
                            returnMessage.Add(3);
                            //byte[] msg = returnMessage.ToArray();

                            handler.Send(returnMessage.ToArray());
                           
                        }
                       
                    }
                    
                }
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
        }

        public static async Task<bool> StoreDataAsync(DeviceDataRecive model)
        {
            try
            {
                if (!isMarkExists(model.MarkCode))
                    throw new Exception("Mark not found");
                if (!DeviceFindOrCreate(model))
                    throw new Exception("Error with create new device");

                DataCollection dataCollection = new DataCollection()
                {
                    Id=Guid.NewGuid(),
                    MarkCode = model.MarkCode,
                    SerialNumber = model.SerialNumber,
                    Signal = model.Signal
                };
                var result=dataCollectionService.AddData(dataCollection);
                if (result.Success)
                {
                    var status = HubConnectionState.Connected;
                    if (connection == null || connection.State != HubConnectionState.Connected)
                    {
                        if (! await ConnctToHubAsync()) {
                            status = HubConnectionState.Disconnected;
                        }
                    }
                    if (status == HubConnectionState.Connected)
                    {
                        try {
                            connection.InvokeAsync("ReciveMessage",
                    result.Data.MarkCode, result.Data.SerialNumber.ToString(), result.Data.Signal.ToString(), result.Data.DateCreated.ToString()).Wait();
                        }
                        catch (Exception e) {
                            Console.WriteLine("signalR Error: " + e.Message);
                        }

                    }
                   
                    // todo signalr
                    return true;
                }
                else
                {
                    throw new Exception("Error with store data");
                }
            }
            catch (Exception err)
            {
                // todo log error
                return false;
            }
            
        }
        private static bool isMarkExists(string markCode)
        {
            var result=markService.GetMark(markCode);
            if (result.Success && result.Data != null)
                return true;
            else
                return false;
        }

        private static bool DeviceFindOrCreate(DeviceDataRecive model)
        {
            var result = deviceService.GetDevice(model.MarkCode, model.SerialNumber);
            if (result.Success && result.Data != null)
                return true;
            else
            {
                Device device = new Device()
                {
                    Port = model.Port,
                    IpAddress = model.IpAddress,
                    MarkCode = model.MarkCode,
                    SerialNumber = model.SerialNumber
                };
                var addResult= deviceService.AddDevice(device);
                return addResult.Success;
            }
        }
    }
}
