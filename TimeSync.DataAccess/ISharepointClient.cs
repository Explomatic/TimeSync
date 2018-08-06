using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using TimeSync.Model;

namespace TimeSync.DataAccess
{
    public interface ISharepointClient
    {
        int GetUserIdFromToolkit(ToolkitUser toolkitUser, Toolkit toolkit);
        int MakeTimeregistration(Timeregistration timereg, ToolkitUser toolkitUser, Toolkit toolkit);
        void MakeTimeregistrations(IEnumerable<Timeregistration> timeregs, ToolkitUser toolkitUser, Toolkit toolkit);
        Toolkit GetTimeslotInformationFromToolkit(ToolkitUser toolkitUser, Toolkit tk);
        List<Team> GetTeamsFromToolkit(ToolkitUser toolkitUser, Toolkit tk);
    }
}
