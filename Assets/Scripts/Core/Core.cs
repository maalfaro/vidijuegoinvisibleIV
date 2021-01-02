using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Core : Singleton<Core>
{

    #region Variables

    [SerializeField] private Sprite[] diceImages;
    [SerializeField] private List<Card> inventoryCards;
    [SerializeField] private Card[] selectedCards;
    public Card[] SelectedCards => playerData.Cards;

    [SerializeField] private List<Enemy> enemies;

    [SerializeField] private PlayerData playerData;
    public PlayerData PlayerData => playerData;

    #endregion

    #region Override methods

    protected override void InitInstance()
    {
        base.InitInstance();
        //selectedCards = new Card[4];
        //selectedCards[0] = new BasicAttackCard();
        //selectedCards[1] = new RerollCard(3);
        playerData.Health = playerData.MaxHealth;

        GoToGameScene();
    }

    #endregion

    #region Public methods

    public void GoToMenu()
    {
        SceneManager.LoadSceneAsync("MenuScene", LoadSceneMode.Single);
    }

    public void GoToGameScene() {
        SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
    }

    public Sprite GetDiceImage(int number)
    {
        return diceImages[number - 1];
    }

    public Enemy GetNextEnemy()
    {
        Enemy nextEnemy = enemies[0];
        enemies.RemoveAt(0);
        return nextEnemy;
    }

    #endregion

    #region Utils

    /// <summary>
    /// Genera un ID unico para cada usuario
    /// </summary>
    /// <returns></returns>
    private string GenerateUserID()
    {
        return System.DateTime.Now.Ticks.ToString("x");
    }

    #endregion

}
