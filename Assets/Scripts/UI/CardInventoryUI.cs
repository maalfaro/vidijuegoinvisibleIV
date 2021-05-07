using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class CardInventoryUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IDropHandler {

    #region Inspector variables

    [SerializeField] private Image image;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform diceTransform;
    [SerializeField] private TMPro.TextMeshProUGUI descriptionText;
    [SerializeField] private TMPro.TextMeshProUGUI conditionText;
    [SerializeField] private TMPro.TextMeshProUGUI titleText;

    #endregion

    #region Variables

    private Vector3 initialPos;

    private Card cardData;
    public Card CardData => cardData;

    public Vector3 Target => diceTransform.position;

    private bool isInventory;
    private int pos;

    #endregion

    #region Public methods

    public void Initialize(Card cardData, bool isInventory, int pos) {
        this.isInventory = isInventory;
        this.pos = pos;
        this.cardData = cardData;
        if (cardData != null) {
            this.cardData.Initialize();
            image.color = cardData.Color;
            diceTransform.gameObject.SetActive(true);
            UpdateUIData();
        } else {
            image.color = Color.grey;
            diceTransform.gameObject.SetActive(false);
            descriptionText.text = "Vacio";
            titleText.text = string.Empty;
        }

    }

    public void UpdateUIData() {
        descriptionText.text = cardData.Description;
        conditionText.text = GetConditionText();
        titleText.text = cardData.Title;
    }

    #endregion

    #region Private methods

    private string GetConditionText() {
        if (cardData.Condition == null) return string.Empty;
        if (cardData.Condition.type.Equals(ConditionTypes.None)) return string.Empty;

        switch (cardData.Condition.type) {
            case ConditionTypes.Even: return "PAR";
            case ConditionTypes.Odd: return "IMPAR";
            case ConditionTypes.Max: return $"MAX.\n{cardData.Condition.number}";
            case ConditionTypes.Min: return $"MIN.\n{cardData.Condition.number}";
        }
        return string.Empty;
    }

    #endregion

    #region Interfaces implementation

    public void OnDrop(PointerEventData data) {
        if (data.pointerDrag != null) {
            CardInventoryUI cardInventory = data.pointerDrag.GetComponent<CardInventoryUI>();

            // Se ha soltado sobre una carta vacia
            if (cardData == null) {
                // Si la carta es de tipo inventory es que se va a dejar en el inventario
                if (isInventory) {
                    // Si ya esta contenida en el inventario no hacemos nada, pero si no está en el inventario la sacamos de las cartas y la ponemos en inventario
                    if (!Core.Instance.PlayerData.Inventory.Contains(cardInventory.cardData)) {
                        Core.Instance.PlayerData.Inventory.Add(cardInventory.cardData);
                        Core.Instance.PlayerData.RemoveCard(cardInventory.cardData);
                        new OnUpdateInventory().FireEvent();
                    }
                } else {
                    // Si esta carta no es de inventario es que es una carta seleccionada por el jugador
                    // Si el jugador no contiene esta carta en las seleccionadas se la añadimos
                    if (!Core.Instance.PlayerData.ContainsCard(cardInventory.cardData)) {
                        Core.Instance.PlayerData.AddCard(cardInventory.cardData,pos);
                        Core.Instance.PlayerData.Inventory.Remove(cardInventory.cardData);
                        new OnUpdateInventory().FireEvent();
                    }
                }
            } else { // Se ha soltado sobre otra carta
                // Si la carta sobre la que se ha soltado es de tipo inventario entonces tenemos que comprobar si la carta que se ha soltado esta en 
                // la lista de cartas seleccinadas, porque si no esta es que es de tipo inventario tambien, si esta en la lista hacemos el cambio
                if (isInventory) {
                    if(Core.Instance.PlayerData.ContainsCard(cardInventory.cardData)) {
                        // Quitamos esta carta de las seleccionadas y la añadimos al inventario
                        Core.Instance.PlayerData.RemoveCard(cardInventory.cardData);

                        // Añadimos la nueva carta a la posicion indicada con el indice de la carta que vamos a cambiar
                        int index = Core.Instance.PlayerData.Inventory.IndexOf(cardData);
                        Core.Instance.PlayerData.Inventory.Insert(index, cardInventory.cardData);

                        // Después quitamos del inventario la nueva carta y la añadimos a seleccionadas
                        Core.Instance.PlayerData.Inventory.Remove(cardData);
                        Core.Instance.PlayerData.AddCard(cardData, cardInventory.pos);
                        new OnUpdateInventory().FireEvent();
                    }
                } else {
                    //Si esta carta no es de tipo inventario, pero la otra carta que tenemos seleccionada esta en el inventario entonces hacemos el cambio
                    if (Core.Instance.PlayerData.Inventory.Contains(cardInventory.cardData)) {
                        // Quitamos esta carta de las seleccionadas y la añadimos al inventario
                        Core.Instance.PlayerData.RemoveCard(cardData);

                        // Añadimos la nueva carta a la posicion indicada con el indice de la carta que vamos a cambiar
                        int index = Core.Instance.PlayerData.Inventory.IndexOf(cardInventory.cardData);
                        Core.Instance.PlayerData.Inventory.Insert(index, cardData);

                        // Después quitamos del inventario la nueva carta y la añadimos a seleccionadas
                        Core.Instance.PlayerData.Inventory.Remove(cardInventory.cardData);
                        Core.Instance.PlayerData.AddCard(cardInventory.cardData,pos); 
                        new OnUpdateInventory().FireEvent();
                    } else {
                        if(cardData!=null && cardInventory.cardData != null) {
                            //Sino las dos cartas son de las seleccionadas y cambiamos sus posiciones
                            Core.Instance.PlayerData.SwitchCards(cardData, cardInventory.cardData);
                            new OnUpdateInventory().FireEvent();
                        }
                    }
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (cardData != null) transform.position = eventData.position ;
    }

    public void OnPointerDown(PointerEventData eventData) {
        canvasGroup.blocksRaycasts = false;
        initialPos = transform.position;
        canvas.sortingOrder = 2;
    }

    public void OnPointerUp(PointerEventData eventData) {
        transform.position = initialPos;
        canvasGroup.blocksRaycasts = true;
        canvas.sortingOrder = 1;
    }

    #endregion
}
