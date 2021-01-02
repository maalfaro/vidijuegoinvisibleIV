using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

[System.Serializable]
public class FlipDiceCard : Card
{

    public override void Initialize() { }

    public override void Use(int number) {

        switch (number) {
            case 1: new OnSplitDice { numbers = new List<int> { 6 } }.FireEvent(); break;
            case 2: new OnSplitDice { numbers = new List<int> { 5 } }.FireEvent(); break;
            case 3: new OnSplitDice { numbers = new List<int> { 4 } }.FireEvent(); break;
            case 4: new OnSplitDice { numbers = new List<int> { 3 } }.FireEvent(); break;
            case 5: new OnSplitDice { numbers = new List<int> { 2 } }.FireEvent(); break;
            case 6: new OnSplitDice { numbers = new List<int> { 1 } }.FireEvent(); break;
        }
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Debug/Cards/FlipDiceCard", false, 1001)]
    static void CreateCardAsset() {
        const string Folder = "Assets/Database/Cards";
        const string AssetPath = Folder + "/FlipDiceCard.asset";
        if (File.Exists(AssetPath))
            return;

        if (!Directory.Exists(Folder))
            Directory.CreateDirectory(Folder);

        var asset = CreateInstance<FlipDiceCard>();
        AssetDatabase.CreateAsset(asset, AssetPath);
    }


#endif
}
