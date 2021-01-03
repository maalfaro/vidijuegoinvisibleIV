using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUI : MonoBehaviour
{

    #region Inspector variables

    [SerializeField] private Image image;
    [SerializeField] private Slider sliderHealth;
    [SerializeField] private TMPro.TextMeshProUGUI nameText;
    [SerializeField] private TMPro.TextMeshProUGUI healthText;
    [SerializeField] private TMPro.TextMeshProUGUI stateText;
    [SerializeField] private ShakeTransformS shakeTransform;

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

        shakeTransform.Begin();

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

