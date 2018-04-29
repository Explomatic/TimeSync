using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSync.Model;

namespace TimeSync.DataAccess
{
    public interface ISharepointClient
    {
        int GetUserIdFromToolkit(ToolkitUser toolkitUser, Toolkit toolkit);

        int MakeTimeregistration(Timeregistration timereg, ToolkitUser toolkitUser, Toolkit toolkit);

        List<TimeSlot> GetTimeSlotsFromToolkit(ToolkitUser toolkitUser, Toolkit toolkit);
        List<Team> CheckForTimeSlots(ToolkitUser toolkitUser, Toolkit tk);
        List<Team> GetTeamsFromToolkit(ToolkitUser toolkitUser, Toolkit tk);
    }
}
