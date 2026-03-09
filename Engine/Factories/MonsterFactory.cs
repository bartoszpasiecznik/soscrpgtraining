using Engine.Models;
namespace Engine.Factories;

public static class MonsterFactory
{
    public static Monster GetMonster(int monsterID)
    {
        switch (monsterID)
        {
            case 1:
                Monster snake = new Monster("Snake", "Snake.png", 4, 4, 5, 1);
                AddLootItem(snake, 9001, 25);
                AddLootItem(snake, 9002, 75);
                return snake;
            
            case 2:
                Monster rat = new Monster("Rat", "Rat.png", 5, 5, 5, 1);
                AddLootItem(rat, 9003, 25);
                AddLootItem(rat, 9004, 75);
                return rat;
            
            case 3:
                Monster spider = new Monster("Spider", "GiantSpider.png", 10, 10, 10, 3);
                AddLootItem(spider, 9005, 25);
                AddLootItem(spider, 9006, 75);
                return spider;
            
            default:
                throw new ArgumentException(string.Format("MonsterType {0} does not exist", monsterID));
        }
    }

    private static void AddLootItem(Monster monster, int itemId, int percentage)
    {
        if (RandomNumberGenerator.NumberBetween(0, 100) < percentage)
        {
            monster.Inventory.Add(new ItemQuantity(itemId, 1));
        }
    }
}