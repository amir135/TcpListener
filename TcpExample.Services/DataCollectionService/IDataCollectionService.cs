using System;
using System.Collections.Generic;
using System.Text;
using TcpExample.Domain.Common;
using TcpExample.Domain.DBModel;

namespace TcpExample.Services.DataCollectionService
{
  
    public interface IDataCollectionService
    {
        Result<int> getDataCollectionCount();
        Result<DataCollection> AddData(DataCollection data);
        Result<List<DataCollection>> GetData(Device data);
    }
}
