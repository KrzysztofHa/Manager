using FluentAssertions;
using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Managers;
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
                PlayerList.Add(new Player { Id = i, IsActive = true, Name = "Player" + i + 1, Country = "Poland" });
            }
        }

        [Fact]
        public void MyTest()
        {
            var temp = "my class!";
            output.WriteLine("This is output from {0}", temp);
        }

        [Fact]
        public void CanAddNewPlayer()
        {
            //Arrange
            var mockIService = new Mock<IService<Player>>();
            mockIService.Setup(p => p.AddItem(It.IsAny<Player>()));
            mockIService.Setup(p => p.SaveList());
            mockIService.Setup(p => p.GetAllItem()).Returns(PlayerList);
            //var playerManager = new PlayerManager(new MenuActionService(), mockIService.Object);


            //Act
            //var returnPlayerManager = playerManager.AddNewPlayer();
            //Assert
            Assert.NotNull(output);
        }

        //[Fact]
        //public void CantFindPlayer()
        //{
        //    //Arrange
        //    var mockIService = new Mock<IService<Player>>();
        //    var playerManager = new PlayerManager(new MenuActionService(), mockIService.Object);
        //    //Act
        //    var returnFindPlayer = playerManager.FindPlayer();
        //    //Assert
        //}
       
    }
}
