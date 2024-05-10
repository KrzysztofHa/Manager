using FluentAssertions;
using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Managers;
using Manager.Domain.Entity;
using Moq;
using Xunit.Abstractions;


namespace Manager.Tests.Infrastructure.Test
{
    public class PlayerManagerTest
    {
        private readonly ITestOutputHelper output;
        public List<Player> PlayerList { get; set; }

        public PlayerManagerTest(ITestOutputHelper output)
        {
            this.output = output;
            this.PlayerList = new List<Player>();
            for (int i = 1; i <= 10; i++)
            {
                PlayerList.Add(new Player { Id = i, Active = true, Name = "Player" + i + 1, Country = "Poland" });
            }
        }

        [Fact]
        public void MyTest()
        {
            var temp = "my class!";
            Console.WriteLine("This is output from {0}", temp);
        }

        [Fact]
        public void CanAddNewPlayer()
        {
            //Arrange
            var mockIService = new Mock<IService<Player>>();
            mockIService.Setup(p => p.AddItem(It.IsAny<Player>()));
            mockIService.Setup(p => p.SaveList());
            mockIService.Setup(p => p.Items).Returns(PlayerList);
            var playerManager = new PlayerManager(new MenuActionService(), mockIService.Object);


            //Act
            var returnPlayerManager = playerManager.AddNewPlayer();
            //Assert
            Assert.NotNull(output);
        }

        [Fact]
        public void CantGetPlayerOfID()
        {
            //Arrange
            var mockIService = new Mock<IService<Player>>();
            mockIService.Setup(p => p.Items).Returns(PlayerList);
            var playerManager = new PlayerManager(new MenuActionService(), mockIService.Object);

            //Act
            var playerOfId = playerManager.GetPlayerOfId(1);

            //Assert
            Assert.NotNull(playerOfId);
            Assert.True(playerOfId is Player);
            Assert.Equal("Player11", PlayerList[0].Name);
        }
    }
}
