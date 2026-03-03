using Engine.Models;
namespace Engine.ViewModels;

public class GameSession
{
    Player CurrentPlayer { get; set; }

    public GameSession()
    {
        CurrentPlayer = new Player();
        CurrentPlayer.Name = "The Bill";
        CurrentPlayer.Gold = 1000000;
    }
}