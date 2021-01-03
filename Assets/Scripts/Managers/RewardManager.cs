using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{

    #region Inspector variables

    [SerializeField] private Button rewardHealthButton;
    [SerializeField] private Button[] rewardButtons;
    [SerializeField] private CardUI[] cards;

    #endregion

    #region Initialize

    public void Initialize(Enemy enemyData) {
        rewardHealthButton.onClick.AddListener(ReclaimPlayerHealth);
        rewardButtons[0].onClick.AddListener(() => ReclaimReward(enemyData.Inventory[0]));
        rewardButtons[1].onClick.AddListener(() => ReclaimReward(enemyData.Inventory[1]));



        for (int i = 0; i < cards.Length; i++) {
            if (i < enemyData.Inventory.Count) {
                cards[i].Initialize(enemyData.Inventory[i]);
                rewardButtons[i].gameObject.SetActive(true);
            } else {
                rewardButtons[i].gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region Private methods

    private void ReclaimReward(Card card) {
        PlayerData playerData = Core.Instance.PlayerData;
        bool added = false;

        //Compruebo todas las cartas y si alguna es nula le pongo la carta en la mano, sino al inventario
        for (int i=0;i< playerData.Cards.Length; i++) {
            if (playerData.Cards[i] == null) {
                playerData.Cards[i] = card;
                added = true;
                break;
            }
        }

        //Si no se la hemos añadido ya la mandamos al inventario
        if(!added) Core.Instance.PlayerData.Inventory.Add(card);
        gameObject.SetActive(false);
    }

    private void ReclaimPlayerHealth() {
        new OnRecoveryHealth { recoveryHealth = 10 }.FireEvent();
        gameObject.SetActive(false);
    }

    #endregion

}
