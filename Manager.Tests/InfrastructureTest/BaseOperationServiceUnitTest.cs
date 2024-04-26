using FluentAssertions;
using Manager.Domain.Entity;
using Manager.Infrastructure.Common;
using Moq;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Xml;
using Xunit.Abstractions;

namespace Manager.Tests.Infrastructure
{
    public class BaseOperationServiceUnitTest
    {


        [Fact]
        public void CanSaveListToBase()
        {
            //Arrange                       
            List<Player> playerList = new List<Player>();
            for (int i = 1; i <= 10; i++)
            {
                playerList.Add(new Player { Id = i + 1, Active = true, Name = "Player" + i + 1, Country = "Poland" });
            }

            Directory.SetCurrentDirectory(@"c:temp");

            var mockBaseOperationService = new Mock<BaseOperationService<Player>>();
            BaseOperationService<Player> iBaseService = new BaseOperationService<Player>();
            iBaseService.ListOfElements = playerList;

            //Act
            var resultiBaseService = iBaseService.SaveListToBase();

            //Assert            
            Assert.True(resultiBaseService);
        }


    }
}