using Manager.App.Concrete;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;

namespace Manager.App.Managers;

public class TurnamentsManager
{
    private readonly MenuActionService _actionService;
    private readonly PlayerManager _playerManager;

    public TurnamentsManager(MenuActionService actionService, PlayerManager playerManager)
    {
        _actionService = actionService;
        _playerManager = playerManager;
    }
    public void SparringOptionView()
    {
        var optionPlayerMenu = _actionService.GetMenuActionsByName("Tournaments");
        while (true)
        {
            ConsoleService.WriteTitle("Tournaments");
            for (int i = 0; i < optionPlayerMenu.Count; i++)
            {
                ConsoleService.WriteLineMessage($"{i + 1}. {optionPlayerMenu[i].Name}");
            }

            var operation = ConsoleService.GetIntNumberFromUser("Enter Option");
            switch (operation)
            {
                case 1:
                    //NewOneDeyTournament();
                    break;
                case 2:
                    
                    ConsoleService.WriteLineMessageActionSuccess("Press Any Key..");
                    break;
                case 3:
                    operation = null;
                    break;
                default:
                    if (operation != null)
                    {
                        ConsoleService.WriteLineErrorMessage("Enter a valid operation ID");
                    }
                    break;
            }

            if (operation == null)
            {
                break;
            }
        }
    }
}
