using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    #region Variables

    [Header("Managers")]
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private Pool<Dice> enemyDicePool;
    [SerializeField] private Transform enemyDiceParent;
    [SerializeField] private PlayerUI playerUI;

    [Header("Propiedades del tablero")]
    [SerializeField] private CardUI[] cardPool;
    [SerializeField] private Pool<Dice> dicePool;
    [SerializeField] private Dice dicePrefab;
    [SerializeField] private Transform diceParent;

    [Header("Botones")]
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button gameOverButton;
    [SerializeField] private Button exitButton;

    [Header("Popups")]
    [SerializeField] private GameObject winPopUp;
    [SerializeField] private GameObject losePopUp;

    private bool playerTurn;

    #endregion

    #region Initialize methods

    private void Start() {
        SuscribeEvents();
        InitializeButtons();

        playerTurn = true;

        dicePool = new Pool<Dice>(dicePrefab, diceParent, 6);
        enemyDicePool = new Pool<Dice>(dicePrefab, enemyDiceParent, 6);
        
        enemyManager.Initialize(Core.Instance.GetNextEnemy());
        playerUI.Initialize();

        InitData();
    }

    private void OnDestroy() {
        UnsuscribeEvents();
    }

    private void InitializeButtons() {
        endTurnButton.onClick.AddListener(EndTurn);
        gameOverButton.onClick.AddListener(() => Core.Instance.GoToMenu());
        exitButton.onClick.AddListener(() => Core.Instance.GoToMenu());
    }

    private void SuscribeEvents() {
        OnDiceUsed.RegisterListener(OnDiceUsedListener);
        OnRerollDice.RegisterListener(OnRerollDiceListener);
        OnDamageReceived.RegisterListener(OnDamageReceivedListener);
        OnRecoveryHealth.RegisterListener(OnRecoveryHealthListener);
        OnSplitDice.RegisterListener(OnSplitDiceListener);
        EnemyManager.OnEndTurn += EndTurn;
    }

    private void UnsuscribeEvents() {
        OnDiceUsed.UnregisterListener(OnDiceUsedListener);
        OnRerollDice.UnregisterListener(OnRerollDiceListener);
        OnDamageReceived.UnregisterListener(OnDamageReceivedListener);
        OnRecoveryHealth.UnregisterListener(OnRecoveryHealthListener);
        OnSplitDice.UnregisterListener(OnSplitDiceListener);
        EnemyManager.OnEndTurn -= EndTurn;
    }

    #endregion

    #region Private methods

    private void InitializeCards() {
        ClearCards();
        for (int i = 0; i < Core.Instance.SelectedCards.Length; i++) {
            Card cardData = Core.Instance.SelectedCards[i];
            CardUI cardUI = cardPool[i];
            cardUI.gameObject.SetActive(cardData != null);
            if (cardData != null) {
                cardUI.Initialize(cardData);
            } 
        }
    }

    private List<CardUI> InitializeEnemyCards() {
        ClearCards();
        List<CardUI> cardUIs = new List<CardUI>();
        for (int i = 0; i < enemyManager.EnemyData.Cards.Length; i++) {
            Card cardData = enemyManager.EnemyData.Cards[i];
            CardUI cardUI = cardPool[i];
            cardUI.gameObject.SetActive(cardData != null);
            if (cardData != null) {
                cardUI.Initialize(cardData);
                cardUIs.Add(cardUI);
            }
        }
        return cardUIs;
    }

    private void ClearCards() {
        for(int i = 0; i < cardPool.Length; i++) {
            cardPool[i].gameObject.SetActive(false);
        }
    }

    private void InitData()
    {
        InitializeCards();

        dicePool.ClearPool();
        for (int i = 0; i < 2; i++) {
            Dice dice = dicePool.GetPoolObject();
            dice.Initialize();
        }
    }

    private List<Dice> InitializeEnemyDices() {
        List<Dice> enemyDicesNumbers = new List<Dice>();
        enemyDicePool.ClearPool();
        for (int i = 0; i < 2; i++) {
            Dice dice = enemyDicePool.GetPoolObject();
            dice.Initialize();
            enemyDicesNumbers.Add(dice);
        }
        return enemyDicesNumbers;
    }

    private void EndTurn()
    {
        playerTurn = !playerTurn;
        if (playerTurn) {
            //Preparamos los datos del jugador
            enemyDicePool.ClearPool();
            InitData();
        } else {
            //Preparamos los datos del enemigo
            List<CardUI> cardUIs = InitializeEnemyCards();
            dicePool.ClearPool();
            List<Dice> enemyDices = InitializeEnemyDices();
            StartCoroutine(_WaitFor(2f,()=> enemyManager.DoAction(enemyDices, cardUIs)));
        }
        
    }

    private IEnumerator _WaitFor(float waitTime, System.Action callback) {

        yield return new WaitForSeconds(waitTime);
        callback?.Invoke();
    }

    private void PlayerWins() {
        winPopUp.SetActive(true);
        //Paramos el juego
        StartCoroutine(_WaitFor(5f, () => Core.Instance.GoToGameScene()));
    }

    #endregion

    #region Listeners

    private void OnDiceUsedListener(OnDiceUsed data) {
        if (data.Card != null && data.Dice != null) {

            if (playerTurn) {
                dicePool.ReleasePoolObject(data.Dice);
            } else {
                enemyDicePool.ReleasePoolObject(data.Dice);
            }
           
            data.Card.Use(data.Dice.Number);

            if (enemyManager.EnemyData.Health <= 0) {
                Debug.LogError("GANAMOS");
            }
               
        }
    }

    private void OnRerollDiceListener(OnRerollDice data) {

        if (playerTurn) {
            Dice dice = dicePool.GetPoolObject();
            dice.Initialize();
        } else {
            Dice dice = enemyDicePool.GetPoolObject();
            dice.Initialize();
        }

        if (data.rerollRemains > 0) {
            for(int i = 0; i < cardPool.Length; i++)
            {
                if (cardPool[i].CardData == null) continue;
                if (cardPool[i].CardData.GetType().Equals(typeof(RerollCard)))
                {
                    cardPool[i].gameObject.SetActive(true);
                    cardPool[i].UpdateUIData();
                    break;
                }
            }
        }

    }

    private void OnRecoveryHealthListener(OnRecoveryHealth data) {
        if (playerTurn) {
            playerUI.RecoveryHealth(data.recoveryHealth);
        } else {
            enemyManager.RecoveryHealth(data.recoveryHealth);
        }
    }

    private void OnDamageReceivedListener(OnDamageReceived data) {
        if (playerTurn) {
            int enemyHealth = enemyManager.ReceiveDamage(data.damage);
            if (enemyHealth <= 0) {
                PlayerWins();
            }
        } else {
            //Hacerle daño al player
            int playerHealth = playerUI.ReceiveDamage(data.damage);
            if (playerHealth <= 0) {
                losePopUp.SetActive(true);
            }
        }
    }

    private void OnSplitDiceListener(OnSplitDice data) {
        if (playerTurn) {
            for(int i = 0; i < data.numbers.Count; i++) {
                Dice dice = dicePool.GetPoolObject();
                dice.Initialize(data.numbers[i]);
            }
        } else {
            for (int i = 0; i < data.numbers.Count; i++) {
                Dice dice = enemyDicePool.GetPoolObject();
                dice.Initialize(data.numbers[i]);
            }
        }
    }

    #endregion

}
