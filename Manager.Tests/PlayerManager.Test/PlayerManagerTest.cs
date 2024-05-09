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


        public PlayerManagerTest(ITestOutputHelper output)
        {
            this.output = output;
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
            List<Player> playerList = new List<Player>();
            for (int i = 1; i <= 10; i++)
            {
                playerList.Add(new Player { Id = i + 1, Active = true, Name = "Player" + i + 1, Country = "Poland" });
            }

            var mockIService = new Mock<IService<Player>>();
            mockIService.Setup(p => p.AddItem(It.IsAny<Player>()));
            mockIService.Setup(p => p.SaveList());
            mockIService.Setup(p => p.Items).Returns(playerList);
            var playerManager = new PlayerManager(new MenuActionService(), mockIService.Object);


            //Act
            var returnPlayerManager = playerManager.AddNewPlayer();
            //Assert
            Assert.NotNull(output);
        }
    }
}
