using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

[System.Serializable]
public class RerollCard : Card
{
    public int TotalReRolls;
    private int rerollRemains;
    public override void Initialize() {
        rerollRemains = TotalReRolls;
        Description = $"Reroll a Dice\n ({rerollRemains} usos)";
    }

    public override void Use(int number)
    {
        rerollRemains--;
        Description = $"Volver a lanzar\n ({rerollRemains} usos)";
        new OnRerollDice() { rerollRemains = rerollRemains}.FireEvent();
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Debug/Cards/ReRollCard", false, 1001)]
    static void CreateBasicAttackAsset() {
        const string Folder = "Assets/Database/Cards";
        const string AssetPath = Folder + "/ReRollCard.asset";
        if (File.Exists(AssetPath))
            return;

        if (!Directory.Exists(Folder))
            Directory.CreateDirectory(Folder);

        var asset = CreateInstance<RerollCard>();
        AssetDatabase.CreateAsset(asset, AssetPath);
    }


#endif
}

