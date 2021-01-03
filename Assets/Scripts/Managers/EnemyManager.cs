using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EnemyManager : MonoBehaviour
{

    public static System.Action OnEndTurn;

    #region Inspector variables

    [SerializeField] private Image image;
    [SerializeField] private Slider sliderHealth;
    [SerializeField] private TMPro.TextMeshProUGUI nameText;
    [SerializeField] private TMPro.TextMeshProUGUI healthText;
    [SerializeField] private TMPro.TextMeshProUGUI stateText;
    [SerializeField] private ShakeTransformS shakeTransform;

    [Header("Animaciones")]
    [SerializeField] private float moveTime = 0.3f;

    private Enemy enemyData;
    public Enemy EnemyData => enemyData;

    private Coroutine coroutine;

    #endregion

    #region Initialize methods

    public void Initialize(Enemy enemy) {
        enemyData = enemy;
        enemyData.Health = enemyData.MaxHealth;
        SetImage(enemyData.Image);
        SetName(enemyData.Name);
        SetHealth(enemyData.Health, enemyData.MaxHealth);
        SetState(enemyData.Shield, enemyData.Dodge);
    }

    #endregion

    #region Public methods

    public void UpdateState() {
        SetState(enemyData.Shield, enemyData.Dodge);
    }

    public void ResetShieldAndDodge() {
        enemyData.ResetDodge();
        enemyData.ResetShield();
    }

    public void DoAction(List<Dice> diceList, List<CardUI> cardsUI) {

        //Si no tenemos dados terminamos el turno
        if (diceList == null || diceList.Count <= 0) {
            OnEndTurn?.Invoke();
            return;
        }

        //Si no tenemos cartas terminamos el turno
        if (cardsUI==null  || cardsUI.Count<=0) {
            OnEndTurn?.Invoke();
            return;
        }
        
        diceList = diceList.OrderByDescending(x => x.Number).ToList();

        CardUI cardUI = CalculateNextCard(cardsUI, diceList);
        //Si no tenemos más cartas de ataque o dodge o defensa me salgo
        if (cardUI == null) {
            OnEndTurn?.Invoke();
            return;
        }

        //StopActions();
        //coroutine = StartCoroutine(_WaitFor(Random.Range(2f,4f),()=> DoAction(diceList, cardsUI)));
    }

    public void StopActions() {
        if (coroutine != null) {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public int ReceiveDamage(int damage) {

        if (enemyData.Dodge) {
            enemyData.ResetDodge();
            SetState(enemyData.Shield, enemyData.Dodge);
            return enemyData.Health;
        }

        shakeTransform.Begin();

        //Si el escudo es menor que el daño que recibimos quitamos al daño el escudo y hacemos el daño
        //sino le quitamos al escudo el daño que recibimos y salimos sin recibir daño.
        if (damage >= enemyData.Shield) {
            damage -= enemyData.Shield;
        } else {
            enemyData.Shield -= damage;
            SetState(enemyData.Shield, enemyData.Dodge);
            return enemyData.Health;
        }

        enemyData.Health -= damage;
        enemyData.Health = enemyData.Health <= 0 ? 0 : enemyData.Health;
        SetHealth(enemyData.Health, enemyData.MaxHealth);
        SetState(enemyData.Shield, enemyData.Dodge);

        return enemyData.Health;
    }

    public void RecoveryHealth(int recoveryHealth) {
        enemyData.Health += recoveryHealth;
        enemyData.Health = enemyData.Health > enemyData.MaxHealth ? enemyData.MaxHealth : enemyData.Health;
        SetHealth(enemyData.Health, enemyData.MaxHealth);
    }

    private IEnumerator _MoveTo(Dice dice, CardUI card) {

        float timer = 0.0f;
        Vector3 initPos = dice.transform.position;
        Vector3 finalPos = card.Target;
        while (timer < 1) {
            timer += Time.deltaTime / moveTime;
            dice.transform.position = Vector3.Lerp(initPos, finalPos, timer);
            yield return null;
        }
        dice.transform.position = card.Target;
        card.MoveCardForEnemy();
        new OnDiceUsed { Card = card.CardData, Dice =  dice}.FireEvent();
    }

    private IEnumerator _WaitFor(float waitTime, System.Action callback) {
        yield return new WaitForSeconds(waitTime);
        callback?.Invoke();
    }

    #endregion

    #region Private methods

    private CardUI CalculateNextCard(List<CardUI> cardsUI, List<Dice> diceList) {

        //Compruebo si tengo cartar de incrementar dado, reroll o flip 
        CardUI resultCard = cardsUI.Find(x => x.CardData.GetType().Equals(typeof(IncreaseDiceCard)) || x.CardData.GetType().Equals(typeof(RerollCard)) || x.CardData.GetType().Equals(typeof(FlipDiceCard)));

        //Comprobamos que la carta no sea nula
        if (resultCard != null) {
            //Ordenamos de mayor a menor y cogemos el más grande distinto de 6
            if (resultCard.CardData.GetType().Equals(typeof(IncreaseDiceCard))) {
                for(int i = 0; i < diceList.Count; i++) {
                    if (diceList[i].Number < 6) {
                        if (resultCard.CardData.CheckCondition(diceList[i].Number)) {
                            StartCoroutine(_MoveTo(diceList[i], resultCard));
                            diceList.RemoveAt(i);
                            cardsUI.Remove(resultCard);
                            return resultCard;
                        }
                    }
                }
            }else {
                Dice dice = diceList.Last();
                if (dice != null) {
                    if (resultCard.CardData.CheckCondition(dice.Number)) {
                        StartCoroutine(_MoveTo(dice, resultCard));
                        diceList.Remove(dice);
                        cardsUI.Remove(resultCard);
                        return resultCard;
                    }       
                }           
            }
        }
        
        // Buscamos si tenemos cartas de ataque, defensa o esquivar para jugar contra el jugador
        resultCard = cardsUI.Find(x => x.CardData.GetType().Equals(typeof(BasicAttackCard)) || x.CardData.GetType().Equals(typeof(ShieldCard)) || x.CardData.GetType().Equals(typeof(DodgeCard)));

        //En el random usamos la posibilidad para que el 40% de las veces use el dado de mayor valor y el resto haga un random
        Dice randomDice = Random.Range(0,10)>5 ? diceList[0] : diceList[Random.Range(0, diceList.Count)];

        if (resultCard != null) {
            //Si la carta no cumple la condicion la quitamos y buscamos la siguiente
            if (!resultCard.CardData.CheckCondition(randomDice.Number)) {
                cardsUI.Remove(resultCard);
                resultCard = cardsUI.Find(x => x.CardData.GetType().Equals(typeof(BasicAttackCard)) || x.CardData.GetType().Equals(typeof(ShieldCard)) || x.CardData.GetType().Equals(typeof(DodgeCard)));
            }

            if (resultCard != null) {
                // Si es una carta de dodge lanzamos un dado para ver si se usa o no
                if (resultCard.CardData.GetType().Equals(typeof(DodgeCard))) {
                    if (Random.Range(0, 10) > 5) {
                        if (resultCard.CardData.CheckCondition(randomDice.Number)) {
                            StartCoroutine(_MoveTo(randomDice, resultCard));
                            diceList.Remove(randomDice);
                            cardsUI.Remove(resultCard);
                            return resultCard;
                        }
                    }
                } else {
                    StartCoroutine(_MoveTo(randomDice, resultCard));
                    diceList.Remove(randomDice);
                    cardsUI.Remove(resultCard);
                    return resultCard;
                }
            }
        }

        resultCard = cardsUI[Random.Range(0, diceList.Count)];

        if (resultCard!=null && resultCard.CardData.CheckCondition(randomDice.Number)) {
            StartCoroutine(_MoveTo(randomDice, resultCard));
            diceList.Remove(randomDice);
            cardsUI.Remove(resultCard);
            return resultCard;
        }

        return null;
    }

    private void SetImage(Sprite avatar)
    {
        image.sprite = avatar;
    }

    private void SetName(string name)
    {
        nameText.text = name;
    }

    private void SetHealth(int health, int totalHealth)
    {
        healthText.text = $"{health} / {totalHealth}";
        sliderHealth.value = (float)health / (float)totalHealth;
    }

    private void SetState(int shield, bool dodge) {
        string text = string.Empty;
        if (shield > 0) {
            text += $"+{shield} Escudo ";
        }
        if (dodge) {
            text += $" +1 Esquivar";
        }
        stateText.text = text;
    }

    #endregion

    #region Listener methods



    #endregion

}
