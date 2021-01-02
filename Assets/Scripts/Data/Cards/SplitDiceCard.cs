using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

[System.Serializable]
public class SplitDiceCard : Card {

    public override void Initialize() { }

    public override void Use(int number) {

        List<int> numbers = new List<int>();
        if (number > 1) {
            numbers.Add(number / 2);
            numbers.Add(number - numbers[0]);
        } else {
            numbers = new List<int> { 1, 1};
        }

        new OnSplitDice { numbers = numbers }.FireEvent();
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Debug/Cards/SplitDiceCard", false, 1001)]
    static void CreateCardAsset() {
        const string Folder = "Assets/Database/Cards";
        const string AssetPath = Folder + "/SplitDiceCard.asset";
        if (File.Exists(AssetPath))
            return;

        if (!Directory.Exists(Folder))
            Directory.CreateDirectory(Folder);

        var asset = CreateInstance<SplitDiceCard>();
        AssetDatabase.CreateAsset(asset, AssetPath);
    }


#endif
}
