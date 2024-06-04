using FluentAssertions;
using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Managers;
using Manager.Consol.Abstract;
using Manager.Domain.Entity;
using Moq;
using Xunit;
using Xunit.Abstractions;


namespace Manager.Tests.ManagerAppTest.PlayerManagerTest
{
    public class PlayerManagerTest
    {
        private readonly ITestOutputHelper output;
        public List<Player> PlayerList { get; set; }

        public PlayerManagerTest(ITestOutputHelper output)
        {
            this.output = output;
            PlayerList = new List<Player>();
            for (int i = 1; i <= 10; i++)
            {
                PlayerList.Add(new Player { Id = i, IsActive = true, FirstName = "Player" + i + 1, Country = "Poland" });
            }
        }

        [Fact]
        public void CanAddNewPlayer()
        {
            //Arrange
            var mockIService = new Mock<IPlayerService>();
            mockIService.Setup(p => p.AddItem(It.IsAny<Player>()));
            mockIService.Setup(p => p.SaveList());
            mockIService.Setup(p => p.GetAllItem()).Returns(PlayerList);
            var mockIConsoleService = new Mock<IConsoleService>();
            
            var mockIUserService = new Mock<IUserService>();
            var playerManager = new PlayerManager(new MenuActionService(), mockIService.Object, mockIConsoleService.Object, mockIUserService.Object);


            //Act
            var returnPlayerManager = playerManager.AddOrUpdatePlayer();

            Assert.NotNull(output);
        }

    }
}
