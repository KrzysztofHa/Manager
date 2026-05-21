using Manager.Domain.Entity;

namespace Manager.App.Abstract;

public interface IPlayerService : IService<Player>
{
    public List<Player> ListOfActivePlayers();

    public List<Player> SearchPlayer(string searchString);

    public Player AddPlayerAddress(Player player, Address address);

    public string GetPlayerDetailToView(Player player);

    public void DeletePlayer(Player player);

    public string GetFirtLastNameById(int Id);

    Address GetPlayerAddress(Player player);
}