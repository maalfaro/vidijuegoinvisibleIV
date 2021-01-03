using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

[System.Serializable]
public class ShieldCard : Card
{
    public int multiplier = 1;

    public override void Initialize() { }

    public override void Use(int number) {
        new OnAddShieldDice { amount = number * multiplier }.FireEvent();
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Debug/Cards/ShieldCard", false, 1001)]
    static void CreateCardAsset() {
        const string Folder = "Assets/Database/Cards";
        const string AssetPath = Folder + "/ShieldCard.asset";
        if (File.Exists(AssetPath))
            return;

        if (!Directory.Exists(Folder))
            Directory.CreateDirectory(Folder);

        var asset = CreateInstance<ShieldCard>();
        AssetDatabase.CreateAsset(asset, AssetPath);
    }


#endif
}
