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

    [Header("Animaciones")]
    [SerializeField] private float moveTime = 0.3f;

    private Enemy enemyData;
    public Enemy EnemyData => enemyData;

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

        diceList = diceList.OrderByDescending(x=>x.Number).ToList();

        StartCoroutine(_MoveTo(diceList[0], cardsUI[0]));
       
        diceList.RemoveAt(0);
        cardsUI.RemoveAt(0);

        StartCoroutine(_WaitFor(Random.Range(2f,4f),()=> DoAction(diceList, cardsUI)));
    }

    public int ReceiveDamage(int damage) {

        if (enemyData.Dodge) {
            enemyData.ResetDodge();
            SetState(enemyData.Shield, enemyData.Dodge);
            return enemyData.Health;
        }

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
        card.gameObject.SetActive(false);
        //Debug.LogError($"Usada la carta {card.CardData.Description} y el numero {dice.Number}");
        new OnDiceUsed { Card = card.CardData, Dice =  dice}.FireEvent();
    }

    private IEnumerator _WaitFor(float waitTime, System.Action callback) {
        yield return new WaitForSeconds(waitTime);
        callback?.Invoke();
    }

    #endregion

    #region Private methods

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
