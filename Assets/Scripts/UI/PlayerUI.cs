using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
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
        SetState(playerData.Shield, playerData.Dodge);
    }

    public int ReceiveDamage(int damage) {
        PlayerData playerData = Core.Instance.PlayerData;

        if (playerData.Dodge) {
            playerData.ResetDodge();
            SetState(playerData.Shield, playerData.Dodge);
            return playerData.Health;
        }

        //Si el escudo es menor que el daño que recibimos quitamos al daño el escudo y hacemos el daño
        //sino le quitamos al escudo el daño que recibimos y salimos sin recibir daño.
        if (damage >= playerData.Shield) {
            damage -= playerData.Shield;
        } else {
            playerData.Shield -= damage;
            SetState(playerData.Shield, playerData.Dodge);
            return playerData.Health;
        }

        playerData.Health -= damage;
        playerData.Health = playerData.Health <= 0 ? 0 : playerData.Health;
        SetHealth(playerData.Health, playerData.MaxHealth);
        SetState(playerData.Shield, playerData.Dodge);

        return playerData.Health;
    }

    public void RecoveryHealth(int recoveryHealth) {
        PlayerData playerData = Core.Instance.PlayerData;
        playerData.Health += recoveryHealth;
        playerData.Health = playerData.Health > playerData.MaxHealth ? playerData.MaxHealth : playerData.Health;
        SetHealth(playerData.Health, playerData.MaxHealth);
    }

    public void ResetShieldAndDodge() {
        Core.Instance.PlayerData.ResetDodge();
        Core.Instance.PlayerData.ResetShield();
    }

    public void UpdateState() {
        PlayerData playerData = Core.Instance.PlayerData;
        SetState(playerData.Shield, playerData.Dodge);
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

    private void SetState(int shield, bool dodge) {
        string text = string.Empty;
        if (shield > 0) {
            text += $"+{shield} Escudo ";
        }
        if (dodge) {
            text += $" +1 Esquivar";
        }
        stateText.text = text;
    }

    #endregion
}

[System.Serializable]
public class PlayerData : ScriptableObject, ICloneable {

    public string Name;
    public Sprite Image;
    public int MaxHealth;
    public int Health;
    public int Shield;
    public bool Dodge;
    public Card[] Cards;

    public List<Card> Inventory;

    public void ResetShield() {
        Shield = 0;
    }

    public void ResetDodge() {
        Dodge = false;
    }

    public object Clone() {
        return this.MemberwiseClone();
    }

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
