using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    #region Variables

    [SerializeField] private CardInventoryUI[] cardsInInventory;
    [SerializeField] private List<CardInventoryUI> cardsInventoryList;

    #endregion

    public void Initialize()
    {
        InitializePlayerCards();
        InitializeInventoryList();
    }

    private void InitializePlayerCards() {
        PlayerData playerData = Core.Instance.PlayerData;
        for (int i = 0; i < playerData.Cards.Length; i++) {
            if (i < playerData.Cards.Length) {
                cardsInInventory[i].Initialize(playerData.Cards[i]);
            } else {
                cardsInInventory[i].Initialize(null);
            }
        }
    }

    private void InitializeInventoryList() {
        PlayerData playerData = Core.Instance.PlayerData;
        for (int i = 0; i < playerData.Inventory.Count; i++) {
            if (i < playerData.Cards.Length) {
                cardsInInventory[i].Initialize(playerData.Cards[i]);
            } else {
                cardsInInventory[i].Initialize(null);
            }
        }
    }

}
