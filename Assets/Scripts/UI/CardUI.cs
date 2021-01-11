using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class CardUI : PoolObject, IDropHandler
{

    #region Inspector variables

    [SerializeField] private Image image;
    [SerializeField] private Image diceImage;
    [SerializeField] private TMPro.TextMeshProUGUI titleText;
    [SerializeField] private TMPro.TextMeshProUGUI descriptionText;
    [SerializeField] private TMPro.TextMeshProUGUI conditionText;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private Sprite emptyDice;

    #endregion

    #region Variables

    private Card cardData;
    private Vector3 initialPos;

    public Card CardData => cardData;

    public Vector3 Target => diceImage.transform.position;

    #endregion

    #region Public methods

    private void Awake() {
        initialPos = transform.position;
    }

    public void Initialize(Card cardData) {
        this.cardData = cardData;
        this.cardData.Initialize();
        diceImage.sprite = emptyDice;
        UpdateUIData();
    }
    
    public void UpdateUIData() {
        titleText.text = cardData.Title;
        descriptionText.text = cardData.Description;
        image.color = CardData.Color;
        diceImage.sprite = emptyDice;
        conditionText.text = GetConditionText(); 
    }

    public void MoveCardForEnemy(int number) {
        diceImage.sprite = Core.Instance.GetDiceImage(number);
        StartCoroutine(_MoveCard(0.5f, -CardData.movement));
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
                diceImage.sprite = Core.Instance.GetDiceImage(dice.Number);
                StartCoroutine(_MoveCard(0.5f, CardData.movement));
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

    #region Coroutines

    private IEnumerator _MoveCard(float animTime, float movement) {

        float timer = 0f;
        SoundsManager.Instance.PlaySound("woosh",1f,0.75f);
        while (timer < 1) {
            timer += Time.deltaTime / animTime;
            transform.position = initialPos + (Vector3.up * animCurve.Evaluate(timer) * movement);
            yield return null;
        }

        transform.position = initialPos;
        gameObject.SetActive(false);
    }

    #endregion

}
