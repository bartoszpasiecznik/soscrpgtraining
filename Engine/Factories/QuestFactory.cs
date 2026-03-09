using Engine.Models;
namespace Engine.Factories;

internal static class QuestFactory
{
    private static List<Quest> _quests = new List<Quest>();

    static QuestFactory()
    {
        //Items needed to complete the quests along with the reward items
        List<ItemQuantity> itemsToComplete = new List<ItemQuantity>();
        List<ItemQuantity> rewardItems = new List<ItemQuantity>();
        
        itemsToComplete.Add(new ItemQuantity(9001, 5));
        rewardItems.Add(new ItemQuantity(1002, 1));
        
        //adding quests
        _quests.Add(new Quest(1,
            "Clear the herb garden",
            "Defeat snakes in the Herbalist's garden",
            itemsToComplete,
            25,
            10,
            rewardItems));
    }

    internal static Quest GetQuestByID(int id)
    {
        return _quests.FirstOrDefault(q => q.ID == id);
    }
}