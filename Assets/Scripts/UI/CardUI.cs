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

    #endregion

    #region Variables

    private Card cardData;
    public Card CardData => cardData;

    #endregion

    #region Public methods

    public void Initialize(Card cardData)
    {
        this.cardData = cardData;
        descriptionText.text = this.cardData.Description;
    }

    #endregion

    #region Interfaces implementation

    public void OnDrop(PointerEventData data)
    {
        if (data.pointerDrag != null)
        {
            Dice dice = data.pointerDrag.GetComponent<Dice>();
            gameObject.SetActive(false);
            new OnDiceUsed { Card = cardData, Dice = dice }.FireEvent();
            //data.pointerDrag.transform.parent = diceTransform;
            //data.pointerDrag.transform.localPosition = Vector3.zero;
            
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


