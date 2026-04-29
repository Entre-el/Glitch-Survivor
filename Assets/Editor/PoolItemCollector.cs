#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PoolItemCollector
{
    // 在 Unity 顶部菜单栏添加一个按钮
    [MenuItem("Tools/自动化/一键收集所有 PoolItem 到 SceneSO")]
    public static void CollectPoolItems()
    {
        // 1. 找到你想要保存数据的目标 SceneSO
        string soPath = "Assets/SO/Scenes/BattleSO.asset";
        SceneSO targetSO = AssetDatabase.LoadAssetAtPath<SceneSO>(soPath);

        if (targetSO == null)
        {
            Debug.LogError($"<color=red>找不到 SceneSO！请检查路径：{soPath}</color>");
            return;
        }

        // 清空旧数据，准备重新收集
        //targetSO.ClearRequiredItems();

        // 2. 定义你要扫描的文件夹路径 (可以定义多个)
        string[] searchFolders = new string[]
        {
            "Assets/Prefabs/Enemies",
            "Assets/Prefabs/Props",
            "Assets/Prefabs/Projectiles",
        };

        // 3. 使用 AssetDatabase 查找这些文件夹下的所有 Prefab
        // "t:Prefab" 表示只搜索预制体类型
        string[] guids = AssetDatabase.FindAssets("t:Prefab", searchFolders);

        int count = 0;
        foreach (string guid in guids)
        {
            // 将 GUID 转换回具体的文件路径
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // 加载预制体
            PoolItem poolItem = AssetDatabase.LoadAssetAtPath<PoolItem>(assetPath);
            if (
                poolItem != null
                && targetSO.requiredItems.Exists(item => item.name == poolItem.name) == false
            ) // 避免重复添加
            {
                targetSO.requiredItems.Add(poolItem);
                count++;
            }
        }

        // 5. 🌟 最关键的一步：标记 SO 已被修改，并强制保存！
        // 否则你重启 Unity 后，收集的数据会丢失
        EditorUtility.SetDirty(targetSO);
        AssetDatabase.SaveAssets();

        Debug.Log(
            $"<color=green>✅ 收集完成！共找到并添加了 {count} 个 PoolItem 到 {targetSO.name}。</color>"
        );
    }
}
#endif
