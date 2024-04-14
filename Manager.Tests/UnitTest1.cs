using Manager.App;
using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Managers;
using Manager.Domain.Entity;
using Manager.Infrastructure.Abstract;
using Manager.Infrastructure.Common;
using Moq;

namespace Manager.Tests
{
    public class UnitTest1
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
            var mockIService = new Mock<IService<PlayerService>>();
           // var playerManager = new Mock(PlayerManager(new MenuActionService(), mockIService.Object));

            IBaseService<Player> mockIBaseService = new BaseOperationService<Player>();



            //Act

            //Asert

        }
    }
}