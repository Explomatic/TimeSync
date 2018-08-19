using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Castle.Windsor;
using TimeSync.DataAccess;
using TimeSync.Model;
using TimeSync.UI.ViewModel;

namespace TimeSync.UI.View
{
    public partial class TimeregistrationsPage : Page
    {
        public TimeregistrationsPage()
        {
            InitializeComponent();
        }

        public TimeregistrationsPage(TimeregistrationViewModel vm) : this()
        {
            DataContext = vm;
        }

        public virtual void CboxToolkitSelected(object sender, EventArgs eventArgs)
        {
            var cbox = sender as ComboBox;
            var timereg = (Timeregistration) cbox?.DataContext;

            var tk = timereg?.Toolkits.FirstOrDefault(t => t.DisplayName == timereg.ToolkitDisplayName);

            if (tk == null) return;

            timereg.Teams.Clear();
            foreach (var team in tk.Teams)
            {
                if (timereg.Teams.Contains(team.Name)) continue;
                timereg.Teams.Add(team.Name);
            }
        }

        public virtual void CboxTeamSelected(object sender, EventArgs eventArgs)
        {
            var cbox = sender as ComboBox;
            var timereg = (Timeregistration) cbox?.DataContext;

            if (timereg?.Team == null) return;

            var tk = timereg.Toolkits.FirstOrDefault(t => t.DisplayName == timereg.ToolkitDisplayName);

            if (tk == null) return;

            var team = tk.Teams.Single(t => t.Name == timereg.Team);
            if (!team.UsesTimeslots)
            {
                timereg.Timeslots.Clear();
            }
            foreach (var timeslot in tk.Timeslots)
            {
                if (timereg.Timeslots.Contains(timeslot.TimeInterval.Interval)) continue;
                timereg.Timeslots.Add(timeslot.TimeInterval.Interval);
            }
        }
    }
}
