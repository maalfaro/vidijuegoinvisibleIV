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
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private RewardManager rewardManager;
    [SerializeField] private InventoryManager inventoryManager;

    [Header("Propiedades del tablero")]
    [SerializeField] private CardUI[] cardPool;
    [SerializeField] private Pool<Dice> dicePool;
    [SerializeField] private Dice dicePrefab;
    [SerializeField] private Transform diceParent;
    [SerializeField] private GameObject blocker;

    [Header("Propiedades del tablero (Enemigo)")]
    [SerializeField] private Dice enemyDicePrefab;
    [SerializeField] private Transform enemyDiceParent;

    [Header("Botones")]
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button gameOverButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button inventoryButton;

    [Header("Popups")]
    [SerializeField] private GameObject winPopUp;
    [SerializeField] private GameObject losePopUp;

    [Header("Layouts")]
    [SerializeField] private Transform playerLayout;
    [SerializeField] private Transform enemyLayout;
    [SerializeField] private CanvasGroup cardsCanvasGroup; 

    private bool playerTurn;

    #endregion

    #region Initialize methods

    private void Start() {
        SuscribeEvents();
        InitializeButtons();

        playerTurn = true;

        dicePool = new Pool<Dice>(dicePrefab, diceParent, 6);
        enemyDicePool = new Pool<Dice>(enemyDicePrefab, enemyDiceParent, 6);
        
        enemyManager.Initialize(Core.Instance.CurrentLevel.enemyData);
        playerUI.Initialize();

        InitializePlayerDices();
    }

    private void OnDestroy() {
        UnsuscribeEvents();
    }

    private void InitializeButtons() {
        endTurnButton.onClick.AddListener(EndTurn);
        gameOverButton.onClick.AddListener(Core.Instance.GoToMenu);
        exitButton.onClick.AddListener(Core.Instance.GoToMenu);
        nextLevelButton.onClick.AddListener(Core.Instance.GoToNextLevel);
        inventoryButton.onClick.AddListener(ShowInventory);
        nextLevelButton.gameObject.SetActive(false);
        inventoryButton.gameObject.SetActive(false);
    }

    private void SuscribeEvents() {
        OnDiceUsed.RegisterListener(OnDiceUsedListener);
        OnRerollDice.RegisterListener(OnRerollDiceListener);
        OnDamageReceived.RegisterListener(OnDamageReceivedListener);
        OnRecoveryHealth.RegisterListener(OnRecoveryHealthListener);
        OnSplitDice.RegisterListener(OnSplitDiceListener);
        OnAddShieldDice.RegisterListener(OnAddShieldDiceListener);
        OnAddDodgeDice.RegisterListener(OnAddDodgeDiceListener);
        EnemyManager.OnEndTurn += EndTurn;
    }

    private void UnsuscribeEvents() {
        OnDiceUsed.UnregisterListener(OnDiceUsedListener);
        OnRerollDice.UnregisterListener(OnRerollDiceListener);
        OnDamageReceived.UnregisterListener(OnDamageReceivedListener);
        OnRecoveryHealth.UnregisterListener(OnRecoveryHealthListener);
        OnSplitDice.UnregisterListener(OnSplitDiceListener);
        OnAddShieldDice.UnregisterListener(OnAddShieldDiceListener);
        OnAddDodgeDice.UnregisterListener(OnAddDodgeDiceListener);
        EnemyManager.OnEndTurn -= EndTurn;
    }

    #endregion

    #region Private methods

    private void InitializeCards() {
        ClearCards();
        for (int i = 0; i < Core.Instance.SelectedCards.Length; i++) {
            Card cardData = Core.Instance.SelectedCards[i];
            if (cardData == null) continue;
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

    private void InitializePlayerDices()
    {
        InitializeCards();

        dicePool.ClearPool();
        for (int i = 0; i < Core.Instance.CurrentLevel.numDice; i++) {
            Dice dice = dicePool.GetPoolObject();
            dice.Initialize();
        }
    }

    private List<Dice> InitializeEnemyDices() {
        List<Dice> enemyDicesNumbers = new List<Dice>();
        enemyDicePool.ClearPool();
        for (int i = 0; i < Core.Instance.CurrentLevel.numDice; i++) {
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
            InitializePlayerDices();
            playerUI.ResetShieldAndDodge();
            playerUI.UpdateState();
            blocker.SetActive(false);
        } else {
            blocker.SetActive(true);
            enemyManager.ResetShieldAndDodge();
            enemyManager.UpdateState();

            //Preparamos los datos del enemigo
            enemyCardUIs = InitializeEnemyCards();
            dicePool.ClearPool();
            enemyDices = InitializeEnemyDices();
            StartCoroutine(_WaitFor(2f,()=> enemyManager.DoAction(enemyDices, enemyCardUIs)));
        }
        
    }

    List<CardUI> enemyCardUIs;
        List < Dice > enemyDices;

    private IEnumerator _WaitFor(float waitTime, System.Action callback) {

        yield return new WaitForSeconds(waitTime);
        callback?.Invoke();
    }

    private void PlayerWins() {
        StartCoroutine(_WaitFor(0.5f, () => {
            winPopUp.SetActive(true);
            StartCoroutine(_MoveAnim(playerLayout, playerLayout.position + (Vector3.down * Screen.height), 0.5f));
            StartCoroutine(_MoveAnim(enemyLayout, playerLayout.position + (Vector3.up * Screen.height), 0.5f));
            StartCoroutine(_Fade(cardsCanvasGroup,1,0,0.5f));
        }));

        //Paramos el juego
        StartCoroutine(_WaitFor(3f, () => {
            rewardManager.Initialize(Core.Instance.CurrentLevel.enemyData);
            rewardManager.gameObject.SetActive(true);
            nextLevelButton.gameObject.SetActive(true);
            inventoryButton.gameObject.SetActive(true);
        }));
    }

    private void PlayerLoses() {
        enemyManager.StopActions();
        StartCoroutine(_WaitFor(0.5f, () => {
            losePopUp.SetActive(true);
            StartCoroutine(_MoveAnim(playerLayout, playerLayout.position + (Vector3.down * Screen.height), 0.5f));
            StartCoroutine(_MoveAnim(enemyLayout, playerLayout.position + (Vector3.up * Screen.height), 0.5f));
            StartCoroutine(_Fade(cardsCanvasGroup, 1, 0, 0.5f));
        }));
    }
    private void ShowInventory() {
        inventoryManager.Initialize();
        inventoryManager.gameObject.SetActive(true);
    }

    #endregion

    #region Listeners

    private void OnDiceUsedListener(OnDiceUsed data) {
        if (data.Card != null && data.Dice != null) {
            if (playerTurn) {
                dicePool.ReleasePoolObject(data.Dice);
            } else {
                enemyDicePool.ReleasePoolObject(data.Dice);
                enemyDices.Remove(data.Dice);
                enemyCardUIs.RemoveAll(x => x.CardData.Equals(data.Card));
                StartCoroutine(_WaitFor(3f, () => enemyManager.DoAction(enemyDices, enemyCardUIs)));
            }

            StartCoroutine(_WaitFor(0.5f, () => data.Card.Use(data.Dice.Number)));
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
                PlayerLoses();
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
                enemyDices.Add(dice);
            }
        }
    }

    private void OnAddShieldDiceListener(OnAddShieldDice data) {
        if (playerTurn) {
            Core.Instance.PlayerData.Shield += data.amount;
            playerUI.UpdateState();
        } else {
            enemyManager.EnemyData.Shield += data.amount;
            enemyManager.UpdateState();
        }
    }

    private void OnAddDodgeDiceListener(OnAddDodgeDice data) {
        if (playerTurn) {
            Core.Instance.PlayerData.Dodge = true;
            playerUI.UpdateState();
        } else {
            enemyManager.EnemyData.Dodge = true;
            enemyManager.UpdateState();
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator _MoveAnim(Transform transform, Vector3 target, float animTime) {

        float timer = 0.0f;
        Vector3 initialPos = transform.position;
        while (timer <= 1) {
            timer += Time.deltaTime / animTime;
            transform.position = Vector3.Lerp(initialPos,target,timer);
            yield return null;
        }

    }

    private IEnumerator _Fade(CanvasGroup canvasGroup, float from, float to, float fadeTime) {
        float timer = 0.0f;
        canvasGroup.alpha = from;
        while (timer <= 1) {
            timer += Time.deltaTime / fadeTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, timer);
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    #endregion
}
