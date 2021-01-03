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
       
        for (int i = 0; i < cards.Length; i++) {
            if (i < enemyData.Inventory.Count) {
                cards[i].Initialize(enemyData.Inventory[i]);
                rewardButtons[i].onClick.AddListener(() => ReclaimReward(enemyData.Inventory[i]));
                rewardButtons[i].gameObject.SetActive(true);
            } else {
                rewardButtons[i].gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region Private methods

    private void ReclaimReward(Card card) {
        Core.Instance.PlayerData.Inventory.Add(card);
        gameObject.SetActive(false);
    }

    private void ReclaimPlayerHealth() {
        new OnRecoveryHealth { recoveryHealth = 10 }.FireEvent();
        gameObject.SetActive(false);
    }

    #endregion

}
