using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

public class PlayerUI : MonoBehaviour
{

    #region Inspector variables

    [SerializeField] private Image image;
    [SerializeField] private Slider sliderHealth;
    [SerializeField] private TMPro.TextMeshProUGUI nameText;
    [SerializeField] private TMPro.TextMeshProUGUI healthText;
    [SerializeField] private TMPro.TextMeshProUGUI stateText;

    #endregion

    #region Public methods

    public void Initialize() {
        PlayerData playerData = Core.Instance.PlayerData;
        SetImage(playerData.Image);
        SetName(playerData.Name);
        SetHealth(playerData.Health, playerData.MaxHealth);
    }

    public int ReceiveDamage(int damage) {
        PlayerData playerData = Core.Instance.PlayerData;
        playerData.Health -= damage;
        playerData.Health = playerData.Health <= 0 ? 0 : playerData.Health;
        SetHealth(playerData.Health, playerData.MaxHealth);

        return playerData.Health;
    }

    public void RecoveryHealth(int recoveryHealth) {
        PlayerData playerData = Core.Instance.PlayerData;
        playerData.Health += recoveryHealth;
        playerData.Health = playerData.Health > playerData.MaxHealth ? playerData.MaxHealth : playerData.Health;
        SetHealth(playerData.Health, playerData.MaxHealth);
    }

    #endregion

    #region Private methods

    private void SetImage(Sprite avatar) {
        image.sprite = avatar;
    }

    private void SetName(string name) {
        nameText.text = name;
    }

    private void SetHealth(int health, int totalHealth) {
        healthText.text = $"{health} / {totalHealth}";
        sliderHealth.value = (float)health / (float)totalHealth;
    }

    #endregion
}

[System.Serializable]
public class PlayerData : ScriptableObject {

    public string Name;
    public Sprite Image;
    public int MaxHealth;
    public int Health;
    public Card[] Cards;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Debug/Player", false, 1022)]
    static void CreatePlayerAsset() {
        const string Folder = "Assets/Database";
        const string AssetPath = Folder + "/Player.asset";
        if (File.Exists(AssetPath))
            return;

        if (!Directory.Exists(Folder))
            Directory.CreateDirectory(Folder);

        var playerAsset = CreateInstance<PlayerData>();
        AssetDatabase.CreateAsset(playerAsset, AssetPath);
    }
#endif
}
