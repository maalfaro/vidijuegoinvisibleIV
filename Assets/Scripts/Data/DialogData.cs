using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

[System.Serializable]
public class DialogData : ScriptableObject
{



#if UNITY_EDITOR
    [MenuItem("Assets/Create/Debug/DialogData", false, 1022)]
    static void CreatePlayerAsset() {
        const string Folder = "Assets/Dialogs";
        const string AssetPath = Folder + "/Dialog.asset";
        if (File.Exists(AssetPath))
            return;

        if (!Directory.Exists(Folder))
            Directory.CreateDirectory(Folder);

        var playerAsset = CreateInstance<DialogData>();
        AssetDatabase.CreateAsset(playerAsset, AssetPath);
    }

#endif

}
