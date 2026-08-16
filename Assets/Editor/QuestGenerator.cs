using UnityEditor;
using UnityEngine;

public class QuestGenerator
{
    [MenuItem("Tools/Generate Test Quests")]
    public static void GenerateQuests()
    {
        string folderPath = "Assets/QuestData";

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder(
                "Assets",
                "QuestData");
        }

        for (int i = 1; i <= 20; i++)
        {
            QuestData quest =
                ScriptableObject.CreateInstance<QuestData>();

            quest.questID = $"Quest_{i}";

            quest.questName = $"Quest {i}";

            quest.description =
                $"This is automatically generated quest {i}.";

            quest.goldReward = Random.Range(50, 200);

            quest.xpReward = Random.Range(20, 100);

            AssetDatabase.CreateAsset(
                quest,
                $"{folderPath}/Quest_{i}.asset");
        }

        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();

        Debug.Log("Generated 20 quests.");
    }
}