using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{

    #region Variables

    [SerializeField] private CardUI[] cardPool;
    [SerializeField] private CardUI cardPrefab;
    [SerializeField] private Transform cardsParent;
    [SerializeField] private Pool<Dice> dicePool;
    [SerializeField] private Dice dicePrefab;
    [SerializeField] private Transform diceParent;

    [SerializeField] private Button endTurnButton;

    #endregion


    protected override void InitInstance()
    {
        base.InitInstance();
        OnDiceUsed.RegisterListener(OnDiceUsedListener);
        OnRerollDice.RegisterListener(OnRerollDiceListener);

        endTurnButton.onClick.AddListener(EndTurn);

        //cardPool = new Pool<CardUI>(cardPrefab, cardsParent, 2);
        dicePool = new Pool<Dice>(dicePrefab, diceParent, 3);

        InitData();
    }

    #region Private methods

    private void InitializeCards()
    {
        for (int i = 0; i < 2; i++)
        {
            CardUI cardUI = cardPool[i];
            //cardUI.transform.position = cardUI.transform.position + (Vector3.right * (i * 450));
            cardUI.gameObject.SetActive(true);
            if (i == 0)
            {
                cardUI.Initialize(new AttackCard());
            }
            else
            {
                cardUI.Initialize(new RerollCard(3));
            }
        }
    }

    private void InitData()
    {
        InitializeCards();

        dicePool.ClearPool();
        for (int i = 0; i < 3; i++) {
            Dice dice = dicePool.GetPoolObject();
            dice.transform.position = dice.transform.position + (Vector3.right * (i * 200));
            dice.Initialize();
        }
    }

    private void EndTurn()
    {
        InitData();
    }

    #endregion

    private void OnDiceUsedListener(OnDiceUsed data) {
        if (data.Card != null && data.Dice != null) {
            dicePool.ReleasePoolObject(data.Dice);
            data.Card.Use(data.Dice.Number);
        }
    }

    private void OnRerollDiceListener(OnRerollDice data) {

        Dice dice = dicePool.GetPoolObject();
        dice.Initialize();

        if (data.rerollRemains > 0) {
            for(int i = 0; i < cardPool.Length; i++)
            {
                if (cardPool[i].CardData == null) continue;
                if (cardPool[i].CardData.GetType().Equals(typeof(RerollCard)))
                {
                    cardPool[i].gameObject.SetActive(true);
                    cardPool[i].Initialize(new RerollCard(data.rerollRemains));
                    break;
                }
            }
        }

    }

}
