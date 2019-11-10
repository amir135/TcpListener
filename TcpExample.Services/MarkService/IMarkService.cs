using System;
using System.Collections.Generic;
using System.Text;
using TcpExample.Domain.Common;
using TcpExample.Domain.DBModel;

namespace TcpExample.Services.MarkService
{
    public interface IMarkService
    {
        Result<int> getMarkCount();
        Result<Mark> AddMark(Mark mark);
        Result<Mark> EditMark(Mark mark);
        Result<Mark> RemoveMark(Mark mark);
        Result<Mark> GetMark(string markCode);
        Result< List<Mark>> GetAllMark(bool withDeactiv = false);
        Result< List<Mark>> GetAllMarkWithDevice(bool withDeactiv = false);

    }
}
