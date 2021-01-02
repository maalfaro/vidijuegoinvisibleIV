using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

[System.Serializable]
public class Enemy : PlayerData {

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Debug/Enemy", false, 1022)]
    static void CreateEnemyAsset()
    {
        const string Folder = "Assets/Database/Enemies";
        const string AssetPath = Folder + "/Enemy.asset";
        if (File.Exists(AssetPath))
            return;

        if (!Directory.Exists(Folder))
            Directory.CreateDirectory(Folder);

        var enemyAsset = CreateInstance<Enemy>();
        AssetDatabase.CreateAsset(enemyAsset, AssetPath);
    }
#endif
}
