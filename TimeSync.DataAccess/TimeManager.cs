using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSync.Model;

namespace TimeSync.DataAccess
{
    public class TimeManager
    {
        /// <summary>
        /// Stores timeregs that have been input by user in "DB"
        /// </summary>
        public void StoreTimereg()
        {
            //Logging?
            //Validation?
            //Call Repo
            throw new NotImplementedException();
        }
        /// <summary>
        /// Call Sharepoint and do clean up for stored timeregs
        /// </summary>
        public void Sync()
        {
            //Logging?
            //Get stored timeregs
            //Send to Sharepoint
            throw new NotImplementedException();
        }

        private void ValidateCustomerField(Timeregistration timereg)
        {
            throw new NotImplementedException();
        }
    }
}
