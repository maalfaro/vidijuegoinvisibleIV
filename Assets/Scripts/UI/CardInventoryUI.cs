using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardInventoryUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

    #region Inspector variables

    [SerializeField] private Image image;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform diceTransform;
    [SerializeField] private TMPro.TextMeshProUGUI descriptionText;
    [SerializeField] private TMPro.TextMeshProUGUI conditionText;

    #endregion

    #region Variables

    private Vector3 initialPos;

    private Card cardData;
    public Card CardData => cardData;

    public Vector3 Target => diceTransform.position;

    #endregion

    #region Public methods

    public void Initialize(Card cardData) {
        if (cardData != null) {
            this.cardData = cardData;
            this.cardData.Initialize();
            UpdateUIData();
        } else {

        }

    }

    public void UpdateUIData() {
        descriptionText.text = cardData.Description;
        conditionText.text = GetConditionText();
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
            Dice dice = data.pointerDrag.GetComponent<Dice>();
            if (cardData.CheckCondition(dice.Number)) {
                gameObject.SetActive(false);
                new OnDiceUsed { Card = cardData, Dice = dice }.FireEvent();
            }
        }
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = eventData.position;
    }

    public void OnPointerDown(PointerEventData eventData) {
        canvasGroup.blocksRaycasts = false;
        initialPos = transform.position;
    }

    public void OnPointerUp(PointerEventData eventData) {
        transform.position = initialPos;
        canvasGroup.blocksRaycasts = true;
    }

    #endregion
}
