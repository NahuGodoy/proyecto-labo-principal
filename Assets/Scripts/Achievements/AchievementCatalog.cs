using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementCatalog", menuName = "Game/Achievement Catalog")]
public class AchievementCatalog : ScriptableObject
{
    public List<AchievementDefinition> achievements = new List<AchievementDefinition>();
}