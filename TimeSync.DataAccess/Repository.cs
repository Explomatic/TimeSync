using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Configuration;
using Microsoft.SharePoint.Client;

namespace TimeSync.DataAccess
{
    public class Repository<T> : IRepository<T> where T : new()
    {
        private FileStream _fileStream;
        private StreamWriter _streamWriter;
        private StreamReader _streamReader;
        private readonly string _saveLocation;
        private readonly string _appData = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\TimeSync";
//        private readonly Dictionary<string, string> _datafileNames = new Dictionary<string, string>
//        {
//            {"Timeregistration","Timeregistration.txt"},
//            {"Toolkit", "Toolkits.txt" },
//            {"ToolkitUser", "ToolkitUser.txt" }
//
//        };

        public Repository()
        {
            var repoType = typeof(T).Name;
            _saveLocation = $"{_appData}\\{repoType}.txt";
            if (!Directory.Exists(_appData))
            {
                Directory.CreateDirectory(_appData);
            }
        }

//        public Repository(string saveLocationConfigKey)
//        {
//            
//            _saveLocation = $"{_appData}\\{_datafileNames[saveLocationConfigKey]}";
//            if (!Directory.Exists(_appData))
//            {
//                Directory.CreateDirectory(_appData);
//            }
//        }

        //Type in
        //Save (add/update): {registrations: [{"date": 20102, "hours": 210391, ....}, {"date": 2+01023...}]}
        //Sync: Update stored regs to be equal to failed regs + new regs
        public bool SaveData(T data)
        {
            var mode = FileMode.Create;
            try
            {
                var jsonString = JsonConvert.SerializeObject(data);

                using (_fileStream = new FileStream(_saveLocation, mode))
                using (_streamWriter = new StreamWriter(_fileStream))
                {
                    _streamWriter.Write(jsonString);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public T GetData()
        {
            try
            {
                using (_fileStream = new FileStream(_saveLocation, FileMode.Open))
                using (_streamReader = new StreamReader(_fileStream))
                {
                    var storedJsonString = _streamReader.ReadToEnd();
                    return JsonConvert.DeserializeObject<T>(storedJsonString);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("No data was found at specified location. Returning empty data.");
                return new T();
            }
        }
    }
}
