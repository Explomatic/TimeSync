# TimeSync
TimeSync is an application intended for making synchronizing time spent on work packages or cases across different toolkits easy, smooth and painless. 

**Credits**

Project Management: Morten Madsen and asras.

Programming: Morten Madsen and asras.

## How to use it
The application consists of three different pages:
- Account information
- Toolkit information
- Timeregistrations

### Account information
In the account information page you input your NCDMZ username (initials only!) and password. If you wish the password can be saved to the database, but this is currently done in cleartext. General storage of the password in memory is done as a SecureString.
Remember to click sava in order to load the information into the program.

### Toolkit information
Once you've added a username and password you can start adding toolkits to the list of available toolkits. For each toolkit you specify
- a name,
- a URL, and
- whether you want to include all teams from the Teams list. If not, then only teams with an active SLA will be included!
You are free to pick a name that you associate with the given toolkit. The URL must not include the typical "/default.aspx" at the end.

**example**

Say you have a toolkit which when opened has the URL https://goto.netcompany.com/cases/GTO31415/NCPI/default.aspx. An entry in the toolkits page could be

| Toolkit name | URL | All teams |
| :---: | :---: | :---: |
| PiToolkit | https://goto.netcompany.com/cases/GTO31415/NCPI | <ul><li>[ ] <!-- This is commented out. --></li></ul> |

Remember to synchronize the toolkit, otherwise you cannot use it for registering time!
There is also the ability to simply save the current entries without fetching data from GOTO.

### Timeregistrations
When you've synchronized one or more toolkits it is time to create one or more timeregistrations. For each timeregistration you specify
- a toolkit (dropdown with Toolkit names),
- a team (dropdown with team names from GOTO),
- the case or work package ID you want to register time on,
- whether it is a work package or not (checkbox),
- The amount of hours you've worked (can be given in several ways, see below),
- a timeslot (dropdown with available timeslots if applicable for the selected team),
- a date for the timeregistration.

#### Amount of hours
The amount of hours that you wish to register can be given either as an absolute value with either **"."** or **","** as the decimal separator or it can be given as a time interval.

**example**

Say we wish to register three timeregs in the NCPI toolkit for three different days. The entries could look as follows

| Toolkit | Team | ID | WP | Hours | Timeslot | Date |
| :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| PiToolkit | PiTeam | 31415 | <ul><li>[ ] <!-- This is commented out. --></li></ul> | 08:00 - 14:15 | | 21-07-2018 |
| PiToolkit | PiTeam | 31415 | <ul><li>[ ] <!-- This is commented out. --></li></ul> | 4,25 | | 22-07-2018 |
| PiToolkit | PiTeam | 31415 | <ul><li>[ ] <!-- This is commented out. --></li></ul> | 6.75 | | 23-07-2018 |
| PiToolkit | PiTeam | 3141592 | <ul><li>[x] <!-- This is commented out. --></li></ul> | 10:15-16:45 | | 24-07-2018 |

Remember to synchronize the timeregistrations. If you aren't done filling out your timeregistrations and need to close down simply save the data and continue where you left off later!
