using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{

    #region Inspector variables

    [SerializeField] private Button rewardHealthButton;
    [SerializeField] private Button[] rewardButtons;
    [SerializeField] private Button claimRewardButton;
    [SerializeField] private CardUI[] cards;
    [SerializeField] private GameObject[] SelectedImgs;

    private Card selectedCard;

    #endregion

    #region Initialize

    public void Initialize(Enemy enemyData) {
        rewardHealthButton.onClick.AddListener(() => SelectReward(2, null));
        claimRewardButton.onClick.AddListener(() => ReclaimReward());
        rewardButtons[0].onClick.AddListener(() => SelectReward(0, enemyData.Inventory[0]));
        rewardButtons[1].onClick.AddListener(() => SelectReward(1, enemyData.Inventory[1]));

        for (int i = 0; i < cards.Length; i++) {
            if (i < enemyData.Inventory.Count) {
                cards[i].Initialize(enemyData.Inventory[i]);
                rewardButtons[i].gameObject.SetActive(true);
            } else {
                rewardButtons[i].gameObject.SetActive(false);
            }
        }

        InitializeSelectedImages();
        claimRewardButton.interactable = false;
    }

    #endregion

    #region Private methods

    private void InitializeSelectedImages() {
        for(int i=0;i< SelectedImgs.Length; i++) SelectedImgs[i].gameObject.SetActive(false);
    }

    private void SetSelectedImage(int index) {
        for (int i = 0; i < SelectedImgs.Length; i++) SelectedImgs[i].gameObject.SetActive(i == index);
    }

    private void SelectReward(int option, Card card) {
        SoundsManager.Instance.PlaySound("switchOn");
        claimRewardButton.interactable = true;
        selectedCard = card;
        SetSelectedImage(option);
    }

    private void ReclaimReward() {
        SoundsManager.Instance.PlaySound("switchOn");

        if (selectedCard == null) {
            ReclaimPlayerHealth();
            return;
        }

        PlayerData playerData = Core.Instance.PlayerData;
        bool added = false;

        //Compruebo todas las cartas y si alguna es nula le pongo la carta en la mano, sino al inventario
        for (int i=0;i< playerData.Cards.Length; i++) {
            if (playerData.Cards[i] == null) {
                playerData.Cards[i] = selectedCard;
                added = true;
                break;
            }
        }

        //Si no se la hemos añadido ya la mandamos al inventario
        if(!added) Core.Instance.PlayerData.Inventory.Add(selectedCard);
        gameObject.SetActive(false);
    }

    private void ReclaimPlayerHealth() {
        new OnRecoveryHealth { recoveryHealth = 10 }.FireEvent();
        gameObject.SetActive(false);
    }

    #endregion

}
