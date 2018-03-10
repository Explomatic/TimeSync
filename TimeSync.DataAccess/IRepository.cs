using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSync.DataAccess
{
    public interface IRepository<T>
    {
        //string SaveLocation { get; set; }
        T GetData();

        bool SaveData(T data);

    }
}
