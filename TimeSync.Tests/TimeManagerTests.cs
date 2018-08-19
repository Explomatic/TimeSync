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
        private Mock<IRepository<List<Toolkit>>> _tkRepoMock;
        private Mock<IRepository<List<Timeregistration>>> _timeregRepoMock;
        private Mock<ISharepointClient> _spClientMock;
        private Mock<IEncryption> _encryptionManagerMock;

        [TestInitialize]
        public void Init()
        {
            _userRepoMock = new Mock<IRepository<ToolkitUser>>();
            _tkRepoMock = new Mock<IRepository<List<Toolkit>>>();
            _timeregRepoMock = new Mock<IRepository<List<Timeregistration>>>();
            _spClientMock = new Mock<ISharepointClient>();
            _encryptionManagerMock = new Mock<IEncryption>();

            _timeManager = new TimeManager(_userRepoMock.Object, _tkRepoMock.Object, _timeregRepoMock.Object
                , _spClientMock.Object, _encryptionManagerMock.Object);

            SetUpMocksToReturnEmpty();
        }

        private void SetUpMocksToReturnEmpty()
        {
            _userRepoMock.Setup(r => r.GetData()).Returns(new ToolkitUser());
            _tkRepoMock.Setup(r => r.GetData()).Returns(new List<Toolkit>());
            _timeregRepoMock.Setup(r => r.GetData()).Returns(new List<Timeregistration>());
            _spClientMock.Setup(r => r.GetUserIdFromToolkit(It.IsAny<ToolkitUser>(), It.IsAny<Toolkit>()))
                .Returns(999);
            _encryptionManagerMock.Setup(r => r.DecryptText(It.IsAny<string>())).Returns("Hest");
        }

        private void MockReturnsListOfTimeRegsWithVariousStatusses()
        {
            _spClientMock.SetupSequence(r => r.MakeTimeregistration(It.IsAny<Timeregistration>(),
                    It.IsAny<ToolkitUser>(), It.IsAny<Toolkit>()))
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
            _timeManager.SyncTimeregs(timeregs);

            //Assert
            Assert.AreEqual(2, timeregs.Count(tr => tr.IsSynchronized));
            Assert.AreEqual(1, timeregs.Count(tr => !tr.IsSynchronized));
        }


        [TestMethod]
        public void SaveToolkitCachesToolkitAndUser()
        {
            //Arrange
            _spClientMock.Setup(r => r.GetUserIdFromToolkit(It.IsAny<ToolkitUser>(), It.IsAny<Toolkit>())).Returns(1);
            var toolkits = new List<Toolkit>()
            {
                new Toolkit()
                {
                    CustomerName = "AKA",
                    Url = "Test",
                    UserId = 1
                },
            };
            var user = new ToolkitUser()
            {
                Name = "Morten",
                Password = "hest",
            };


            //Act
            _timeManager.SyncToolkits(user, toolkits);

            //Assert
            var privateObj = new PrivateObject(_timeManager);
            var privInfo = privateObj.GetField("_toolkitInfo");
            var privUser = privateObj.GetField("_toolkitUser");

            Assert.AreEqual(toolkits, privInfo);
            Assert.AreEqual(user, privUser);
        }

        
    }
}
