using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class CardUI : PoolObject, IDropHandler
{

    #region Inspector variables

    [SerializeField] private Image image;
    [SerializeField] private Transform diceTransform;
    [SerializeField] private TMPro.TextMeshProUGUI descriptionText;
    [SerializeField] private TMPro.TextMeshProUGUI conditionText;

    #endregion

    #region Variables

    private Card cardData;
    public Card CardData => cardData;

    public Vector3 Target => diceTransform.position;

    #endregion

    #region Public methods

    public void Initialize(Card cardData)
    {
        this.cardData = cardData;
        this.cardData.Initialize();
        UpdateUIData();
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

    public void OnDrop(PointerEventData data)
    {
        if (data.pointerDrag != null)
        {
            Dice dice = data.pointerDrag.GetComponent<Dice>();
            if (cardData.CheckCondition(dice.Number)) {
                gameObject.SetActive(false);
                new OnDiceUsed { Card = cardData, Dice = dice }.FireEvent();
            }        
        }
    }

    public override void GetPoolObject()
    {
        gameObject.SetActive(true);
    }
    public override void ReleasePoolObject()
    {
        gameObject.SetActive(false);
    }

    #endregion

}


