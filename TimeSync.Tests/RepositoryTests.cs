using System;
using System.IO;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TimeSync.Model;
using TimeSync.DataAccess;

namespace TimeSync.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        private Repository<ToolkitUser> _repo;


        [TestInitialize]
        public void Init()
        {
            
        }

        [TestMethod]
        public void GetUserInfoFromRepoTest()
        {
            _repo = new Repository<ToolkitUser>();
            var userInfo = _repo.GetData();

            Assert.AreEqual(userInfo.Name, "MOMA");
            Assert.AreEqual(userInfo.Password, "");
            Assert.AreEqual(userInfo.Domain, "NCDMZ");
        }

        [TestMethod]
        public void SaveUserInfoToRepoTest()
        {
            _repo = new Repository<ToolkitUser>();
            var userInfo = new ToolkitUser
            {
                Name = "MOMA",
                Password = @"blablabla"
            };

            var success = _repo.SaveData(userInfo);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void SaveAndGetUserInfoToRepoTest()
        {
            _repo = new Repository<ToolkitUser>();
            var userInfo = new ToolkitUser
            {
                Name = "Testname",
                Password = @"o_v2\J;CRpiR/mx:hx1/3d=PveE&LijSr,+k@PXL'UTH)"
            };

            var success = _repo.SaveData(userInfo);

            var retrievedUser = _repo.GetData();

            Assert.IsTrue(success);
            Assert.AreEqual(userInfo.Name, retrievedUser.Name);
            Assert.AreEqual(userInfo.Password, retrievedUser.Password);
        }
    }
}
