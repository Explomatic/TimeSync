using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TimeSync.Model;

namespace TimeSync.DataAccess
{
    public class Repository
    {

        private FileStream _fileStream;
        private StreamWriter _streamWriter;
        private StreamReader _streamReader;

        private string _saveLocation = "SavedData.txt";

        //Type in
        //Save (add/update): {registrations: [{"date": 20102, "hours": 210391, ....}, {"date": 2+01023...}]}
        //Sync: Update stored regs to be equal to failed regs + new regs
        public void SaveTimeregistration(Timeregistration timereg)
        {
            //Convert object to JSON (Newtonsoft)
            //Save JSON to DB
            throw new NotImplementedException();
        }

        public List<Timeregistration> GetTimeregistrations()
        {
            //Retrieve objects/read text file
            //Convert JSON to object (Newtonsoft)
            throw new NotImplementedException();
        }

        public void SaveUserInfo()
        {
            //info.Password = Encrypt(info.Password);
            throw new NotImplementedException();
        }

        public UserInfo GetUserInfo()
        {
            try
            {
                using (_fileStream = new FileStream(_saveLocation, FileMode.Open))
                using (_streamReader = new StreamReader(_fileStream))
                {
                    string storedJsonString = _streamReader.ReadToEnd();
                    return JsonConvert.DeserializeObject<UserInfo>(storedJsonString);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("No data was found at specified location. Returning empty data.");
                return new UserInfo();
            }
        }
    }
}
