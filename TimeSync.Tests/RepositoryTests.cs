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
            Model.ToolkitUser userInfo;
            _repo = new Repository<ToolkitUser>("TestRetrieveLocation");
            userInfo = _repo.GetData();

            Assert.AreEqual(userInfo.Name, "MOMA");
            Assert.AreEqual(userInfo.Password, "");
            Assert.AreEqual(userInfo.Domain, "NCDMZ");
        }

        [TestMethod]
        public void SaveUserInfoToRepoTest()
        {
            _repo = new Repository<ToolkitUser>("TestSaveLocation");
            Model.ToolkitUser userInfo = new Model.ToolkitUser();


            userInfo.Name = "MOMA";
            userInfo.Password = "blablabla";
;

            var success = _repo.SaveData(userInfo);

            Assert.IsTrue(success);


        }
    }
}
