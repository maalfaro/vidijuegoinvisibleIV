﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dice : PoolObject, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

    #region Inspector variables

    [SerializeField] private Image image;
    [SerializeField] private CanvasGroup canvasGroup;

    #endregion

    #region Variables

    private Vector3 initialPos;
    private int number;
    public int Number => number;

    #endregion

    #region Monobehaviour methods

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    #endregion

    #region Public methods

    public void Initialize()  {
        Initialize(Random.Range(1, 7));
    }

    public void Initialize(int number) {
        this.number = number;
        image.sprite = Core.Instance.GetDiceImage(number);
    }

    #endregion

    #region Handler methods implementation

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        initialPos = transform.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.position = initialPos;
        canvasGroup.blocksRaycasts = true;
    }

    #endregion

    #region Pool Object implementation

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
