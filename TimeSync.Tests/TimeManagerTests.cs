using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TimeSync.DataAccess;
using TimeSync.Model;

namespace TimeSync.Tests
{
    [TestClass]
    public class TimeManagerTests
    {
        private TimeManager _timeManager;
        private Mock<IRepository<ToolkitUser>> _userRepoMock;
        private Mock<IRepository<ToolkitInfo>> _infoRepoMock;
        private Mock<IRepository<List<Timeregistration>>> _timeregRepoMock;
        private Mock<ISharepointClient> _spClientMock;

        [TestInitialize]
        public void Init()
        {
            _userRepoMock = new Mock<IRepository<ToolkitUser>>();
            _infoRepoMock = new Mock<IRepository<ToolkitInfo>>();
            _timeregRepoMock = new Mock<IRepository<List<Timeregistration>>>();
            _spClientMock = new Mock<ISharepointClient>();

            _timeManager = new TimeManager(_userRepoMock.Object, _infoRepoMock.Object, _timeregRepoMock.Object
                , _spClientMock.Object);


            SetUpMocksToReturnEmpty();
        }

        private void SetUpMocksToReturnEmpty()
        {
            _userRepoMock.Setup(r => r.GetData()).Returns(new ToolkitUser());
            _infoRepoMock.Setup(r => r.GetData()).Returns(new ToolkitInfo());
            _timeregRepoMock.Setup(r => r.GetData()).Returns(new List<Timeregistration>());
            _spClientMock.Setup(r => r.GetUserIdFromToolkit(It.IsAny<ToolkitUser>(), It.IsAny<Toolkit>()))
                .Returns(999);
        }

        private void MockReturnsListOfTimeRegsWithVariousStatusses()
        {
            _spClientMock.SetupSequence(r => r.MakeTimeregistration(It.IsAny<Timeregistration>(),
                    It.IsAny<ToolkitUser>(), It.IsAny<ToolkitInfo>()))
                .Returns(1)
                .Returns(-1)
                .Returns(1);
        }

        [TestMethod]
        public void SyncUpdatesTimeregStatus()
        {
            //Arrange
            MockReturnsListOfTimeRegsWithVariousStatusses();
            var timeregs = new List<Timeregistration>()
            {
                new Timeregistration(),
                new Timeregistration(),
                new Timeregistration()
            };

            //Act
            _timeManager.Sync(timeregs);

            //Assert
            Assert.AreEqual(2, timeregs.Count(tr => tr.IsSynchronized));
            Assert.AreEqual(1, timeregs.Count(tr => !tr.IsSynchronized));
        }


        [TestMethod]
        public void SaveToolkitCachesToolkitAndUser()
        {
            //Arrange
            _spClientMock.Setup(r => r.GetUserIdFromToolkit(It.IsAny<ToolkitUser>(), It.IsAny<Toolkit>())).Returns(1);
            var info = new ToolkitInfo()
            {
                Toolkits = new Dictionary<string, Toolkit>()
                {
                    {
                        "Customer", new Toolkit()
                        {
                            UserId = 1,
                            Url = "Test",
                        }
                    }
                }
            };
            var user = new ToolkitUser()
            {
                Name = "Morten",
                Password = "hest",
            };


            //Act
            _timeManager.SaveToolkitInfo(user, info);

            //Assert
            var privateObj = new PrivateObject(_timeManager);
            var privInfo = privateObj.GetField("_toolkitInfo");
            var privUser = privateObj.GetField("_toolkitUser");

            Assert.AreEqual(info, privInfo);
            Assert.AreEqual(user, privUser);
        }

        
    }
}
