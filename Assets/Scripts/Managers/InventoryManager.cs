using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{

    #region Variables

    [SerializeField] private CardInventoryUI[] cardsInInventory;
    [SerializeField] private List<CardInventoryUI> cardsInventoryList;
    [SerializeField] private Button exitButton;

    #endregion

    private void Awake() {
        OnUpdateInventory.RegisterListener(OnUpdateInventoryListener);
        exitButton.onClick.AddListener(CloseInventory);
    }

    private void OnDestroy() {
        OnUpdateInventory.UnregisterListener(OnUpdateInventoryListener);
    }


    public void Initialize()
    {
        InitializePlayerCards();
        InitializeInventoryList();
    }

    private void InitializePlayerCards() {
        PlayerData playerData = Core.Instance.PlayerData;
        for (int i = 1; i < cardsInInventory.Length+1; i++) {
            if (i < playerData.Cards.Length) {
                cardsInInventory[i-1].Initialize(playerData.Cards[i],false,i);
            } else {
                cardsInInventory[i-1].Initialize(null, false,i);
            }
        }
    }

    private void InitializeInventoryList() {
        PlayerData playerData = Core.Instance.PlayerData;
        for (int i = 0; i < cardsInventoryList.Count; i++) {
            if (i < playerData.Inventory.Count) {
                cardsInventoryList[i].Initialize(playerData.Inventory[i],true,i);
            } else {
                cardsInventoryList[i].Initialize(null, true,i);
            }
        }
    }

    private void OnUpdateInventoryListener(OnUpdateInventory data) {
        SoundsManager.Instance.PlaySound("flipCard", 1f);
        Initialize();
    }

    private void CloseInventory() {
        SoundsManager.Instance.PlaySound("click");
        gameObject.SetActive(false);
    }

}
