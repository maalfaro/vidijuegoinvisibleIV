using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

[System.Serializable]
public class IncreaseDiceCard : Card
{
    public int increase = 1;
    public override void Initialize() { }

    public override void Use(int number) {

        if (number == 6) {
            new OnSplitDice { numbers = new List<int> { 6, 1 } }.FireEvent();
        } else {
            new OnSplitDice { numbers = new List<int> { number + increase } }.FireEvent();
        }
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Debug/Cards/IncreaseDiceCard", false, 1001)]
    static void CreateCardAsset() {
        const string Folder = "Assets/Database/Cards";
        const string AssetPath = Folder + "/IncreaseDiceCard.asset";
        if (File.Exists(AssetPath))
            return;

        if (!Directory.Exists(Folder))
            Directory.CreateDirectory(Folder);

        var asset = CreateInstance<IncreaseDiceCard>();
        AssetDatabase.CreateAsset(asset, AssetPath);
    }


#endif
}
