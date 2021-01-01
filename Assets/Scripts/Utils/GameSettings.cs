using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

[System.Serializable]
public class GameSettings : ScriptableObject
{
    [Header("Escudos de los equipos")]
    [SerializeField] private StringSpriteSerializeDictionary teamImages;
    [Header("Imagenes de los jugadores")]
    [SerializeField] private StringSpriteSerializeDictionary playerImages;

    public Sprite GetTeamImage(string imageName)
    {        
        if (string.IsNullOrEmpty(imageName)) {
            return teamImages["None"];
        }

        if (teamImages.ContainsKey(imageName)) {
            return teamImages[imageName];
        }

        Debug.LogError($"No existe la imagen del equipo. ({imageName})");
        return teamImages["None"];
    }

    public Sprite GetPlayerImage(string imageName)
    {

        if (string.IsNullOrEmpty(imageName))
        {
            return playerImages["None"];
        }

        if (playerImages.ContainsKey(imageName))
        {
            return playerImages[imageName];
        }

        Debug.LogError($"No existe la imagen del jugador. ({imageName})");
        return playerImages["None"];
    }


#if UNITY_EDITOR
    [MenuItem("Assets/Create/Debug/GameSettings", false, 1022)]
    static void CreateGameSettingsAsset()
    {
        const string Folder = "Assets/NewScripts";
        const string AssetPath = Folder + "/GameSettings.asset";
        if (File.Exists(AssetPath))
            return;

        if (!Directory.Exists(Folder))
            Directory.CreateDirectory(Folder);

        var gameSettings = CreateInstance<GameSettings>();
        AssetDatabase.CreateAsset(gameSettings, AssetPath);
    }
#endif

}

[System.Serializable]
public class StringSpriteSerializeDictionary : SerializableDictionary<string, Sprite> { }
