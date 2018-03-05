﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSync.Model;

namespace TimeSync.DataAccess
{
    public class TimeManager
    {
        public UserInfo UserInfo;

        public TimeManager()
        {
            UserInfo = new UserInfo();
        }
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

        public void AddToolkit(UserInfo userInfo, ToolkitInfo toolkitInfo)
        {
            toolkitInfo.UserId = SharepointClient.GetUserIdFromToolkit(toolkitInfo, userInfo);
            userInfo.ToolkitInfos.Add(toolkitInfo);
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
