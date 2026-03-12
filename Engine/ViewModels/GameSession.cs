using System.ComponentModel;
using System.Runtime.CompilerServices;
using Engine.EventArgs;
using Engine.Models;
using Engine.Factories;
namespace Engine.ViewModels;

public class GameSession : BaseNotificationClass
{
    public event EventHandler<GameMessageEventArgs> OnMessageRaised;

    #region Properties
    
    private Location _currentLocation;
    private Monster _currentMonster;
    private Trader _currentTrader;
    public Player CurrentPlayer { get; set; }

    public Location CurrentLocation
    {
        get{return _currentLocation;}
        set{_currentLocation = value;
            OnPropertyChanged(nameof(CurrentLocation));
            OnPropertyChanged(nameof(HasLocationToNorth));
            OnPropertyChanged(nameof(HasLocationToWest));
            OnPropertyChanged(nameof(HasLocationToEast));
            OnPropertyChanged(nameof(HasLocationToSouth));

            CompleteQuestAtLocation();
            GivePlayerQuestsAtLocation();
            GetMonsterAtLocation();
            HealPlayerWhenHome();

            CurrentTrader = CurrentLocation.TraderHere;

        }
    }

    public Monster CurrentMonster
    {
        get { return _currentMonster;} 
        set 
        {
            _currentMonster = value;
            OnPropertyChanged(nameof(CurrentMonster));
            OnPropertyChanged(nameof(HasMonster));

            if (CurrentMonster != null)
            {
                RaiseMessage("");
                RaiseMessage($"You see a {CurrentMonster.Name}!");
            }
            
        }
    }
    
    public bool HasMonster => CurrentMonster != null;

    public Trader CurrentTrader
    {
        get { return _currentTrader; }
        set
        {
            _currentTrader = value;
            OnPropertyChanged(nameof(CurrentTrader));
            OnPropertyChanged(nameof(HasTrader));
        }
    }
    
    public bool HasTrader => CurrentTrader != null;
    
    public Weapon CurrentWeapon{get; set;}
    
    public bool HasLocationToNorth => CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1) != null;
    public bool HasLocationToWest => CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate) != null;
    public bool HasLocationToEast => CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate) != null;
    public bool HasLocationToSouth => CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1) != null;
    
    public World CurrentWorld { get; set; }
    
    #endregion
    
    public GameSession()
    {
        CurrentPlayer = new Player
        {
            Name = "The Bill",
            CharacterClass = "Fajter", 
            HitPoints = 10, 
            Gold = 100000, 
            ExperiencePoints = 0, 
            Level = 1
        };

        if (!CurrentPlayer.Weapons.Any())
        {
            CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(1001));
        }

        CurrentWorld = WorldFactory.CreateWorld();
        CurrentLocation = CurrentWorld.LocationAt(0, 0);
        
        
    }

    public void MoveNorth()
    {
        if (HasLocationToNorth)
        {
            CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1);
        }
    }
    public void MoveWest()
    {
        if (HasLocationToWest)
        {
            CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate);
        }
    }
    public void MoveEast()
    {
        if (HasLocationToEast)
        {
            CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate);
        }
    }
    public void MoveSouth()
    {
        if (HasLocationToSouth)
        {
            CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1);
        }
    }

    private void CompleteQuestAtLocation()
    {
        foreach (Quest quest in CurrentLocation.QuestsAvailableHere)
        {
            QuestStatus questToComplete = CurrentPlayer.Quests.FirstOrDefault(q =>q.PlayerQuest.ID == quest.ID && !q.IsCompleted);
            if (questToComplete != null)
            {
                if (CurrentPlayer.HasAllTheseItems(quest.ItemsToComplete))
                {
                    foreach (ItemQuantity itemQuantity in quest.ItemsToComplete)
                    {
                        for (int i = 0; i < itemQuantity.Quantity; i++)
                        {
                            CurrentPlayer.RemoveItemFromInventory(CurrentPlayer.Inventory.First(item => item.ItemTypeID == itemQuantity.ItemID));
                        }
                    }
                    RaiseMessage("");
                    RaiseMessage($"Congratulations! You completed the '{quest.Name}' quest!");
                    
                    //rewards
                    CurrentPlayer.ExperiencePoints += quest.RewardExperiencePoints;
                    RaiseMessage($"You receive {quest.RewardExperiencePoints}  Experience Points!");
                    
                    CurrentPlayer.Gold += quest.RewardGold;
                    RaiseMessage($"You receive {quest.RewardGold} Gold!");

                    foreach (ItemQuantity itemQuantity in quest.RewardItems)
                    {
                        GameItem rewardItem = ItemFactory.CreateGameItem(itemQuantity.ItemID);
                        CurrentPlayer.AddItemToInventory(rewardItem);
                        RaiseMessage($"You receive {rewardItem.Name}!");
                    }
                    
                    //Mark quest as completed
                    questToComplete.IsCompleted = true;
                }
            }
        }
    }
    
    private void GivePlayerQuestsAtLocation()
    {
        foreach (Quest quest in CurrentLocation.QuestsAvailableHere )
        {
            if (!CurrentPlayer.Quests.Any(q => q.PlayerQuest.ID == quest.ID))
            {
                CurrentPlayer.Quests.Add(new QuestStatus(quest));
                RaiseMessage("");
                RaiseMessage($"You received the {quest.Name} quest!");
                RaiseMessage(quest.Description);
                
                RaiseMessage("Return with:");
                foreach (ItemQuantity itemQuantity in quest.ItemsToComplete)
                {
                    RaiseMessage($"{itemQuantity.Quantity} {ItemFactory.CreateGameItem(itemQuantity.ItemID).Name}!");
                }
                RaiseMessage("And you will recieve:");
                RaiseMessage($"{quest.RewardExperiencePoints} Experience Points!");
                RaiseMessage($"{quest.RewardGold} Gold!");
                foreach (ItemQuantity itemQuantity in quest.RewardItems)
                {
                    RaiseMessage($"{itemQuantity.Quantity} {ItemFactory.CreateGameItem(itemQuantity.ItemID).Name}!");
                }
            }
        }
    }

    private void GetMonsterAtLocation()
    {
        CurrentMonster = CurrentLocation.GetMonster();
    }

    public void AttackCurrentMonster()
    {
        if (CurrentWeapon == null)
        {
            RaiseMessage("No weapon, select one to perform an attack ");
        }
        
        //Determine dmg
        int damageToMonster = RandomNumberGenerator.NumberBetween(CurrentWeapon.MinimumDamage, CurrentWeapon.MaximumDamage);

        if (damageToMonster == 0)
        {
            RaiseMessage($"You missed the {CurrentMonster.Name}!");
        }
        else
        {
            CurrentMonster.HitPoints -= damageToMonster;
            RaiseMessage($"You hit the {CurrentMonster.Name} for {damageToMonster} damage!");
        }
        
        //Monster kill
        if (CurrentMonster.HitPoints <= 0)
        {
            RaiseMessage("");
            RaiseMessage($"You killed the {CurrentMonster.Name}!");

            CurrentPlayer.ExperiencePoints += CurrentMonster.RewardExperiencePoints;
            RaiseMessage($"You gained {CurrentMonster.RewardExperiencePoints} experience!");
            
            CurrentPlayer.Gold += CurrentMonster.RewardGold;
            RaiseMessage($"You gained {CurrentMonster.RewardGold} gold!");

            foreach (ItemQuantity itemQuantity in CurrentMonster.Inventory)
            {
                GameItem item = ItemFactory.CreateGameItem(itemQuantity.ItemID);
                CurrentPlayer.Inventory.Add(item);
                RaiseMessage($"You Recieve {itemQuantity.Quantity} {item.Name}!");
            }
            
            //Spawn another monster
            GetMonsterAtLocation();
        }
        else
        {
            //Monster attack
            int damageToPlayer = RandomNumberGenerator.NumberBetween(CurrentMonster.MinimumDamage, CurrentMonster.MaximumDamage);
            if (damageToPlayer == 0)
            {
                RaiseMessage($"{CurrentMonster.Name} Missed it's attack!");
            }
            else
            {
                CurrentPlayer.HitPoints -= damageToPlayer;
                RaiseMessage($"{CurrentMonster.Name} Hit you for {damageToPlayer} damage!");
            }
            
            //Player killed, move them to their home
            if (CurrentPlayer.HitPoints <= 0)
            {
                RaiseMessage("");
                RaiseMessage($"{CurrentMonster.Name} killed you :(");
                
                CurrentLocation = CurrentWorld.LocationAt(0, -1); //Move to home
                CurrentPlayer.HitPoints = CurrentPlayer.Level * 10; //Heal the player
            }
        }
        
    }

    private void HealPlayerWhenHome()
    {
        if (CurrentLocation == CurrentWorld.LocationAt(0, -1))
        {
            CurrentPlayer.HitPoints = CurrentPlayer.Level * 10; //Heal the player
        }
    }
    
    private void RaiseMessage(string message)
    {
        OnMessageRaised?.Invoke(this, new GameMessageEventArgs(message));
    }
    
}