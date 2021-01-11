﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Core : Singleton<Core>
{

    #region Variables

    #region Inspector variables

    [SerializeField] private Sprite[] diceImages;
    [SerializeField] private Card[] initialCards;
    [SerializeField] private List<Enemy> enemiesPrefabs;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private List<Enemy> MigsData;

    #endregion

    private List<LevelData> levels;
    private LevelData currentLevel;

    public LevelData CurrentLevel => currentLevel; 
    public PlayerData PlayerData => playerData;
    public Card[] SelectedCards => playerData.Cards;

    #endregion

    #region Override methods

    protected override void InitInstance()
    {
        base.InitInstance();
        IntializePlayerData();
        GoToMenu();
    }

    #endregion

    #region Public methods

    public void GoToMenu() {
        SoundsManager.Instance.StopMusic();
        SceneManager.LoadSceneAsync("MenuScene", LoadSceneMode.Single);
    }

    public void GoToNextLevel() {
        if (levels.Count > 0) {
            currentLevel = levels[0];
            levels.RemoveAt(0);
            SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
        } else {
            //Ir al nivel fin del juego
            SceneManager.LoadSceneAsync("EndScene", LoadSceneMode.Single);
        }
    }

    public void StartGame() {
        IntializePlayerData();
        IntializeLevels();
        GoToNextLevel();
    }

    public Sprite GetDiceImage(int number)
    {
        return diceImages[number - 1];
    }

    #endregion

    #region Private methods

    private void IntializePlayerData() {
        playerData.Health = playerData.MaxHealth;
        playerData.Dodge = false;
        playerData.Shield = 0;
        playerData.Inventory = new List<Card>();
        playerData.Cards = new Card[4];
        playerData.Cards[0] = initialCards[0];
        //playerData.Cards[1] = initialCards[1];
    }

    private void IntializeLevels() {
        levels = new List<LevelData>();
        enemiesPrefabs.Shuffle();
        enemiesPrefabs.Insert(0, MigsData[0]);
        for (int i = 0; i < enemiesPrefabs.Count; i++) {
            LevelData level = new LevelData();
            level.enemyData = (Enemy) enemiesPrefabs[i].Clone();
            level.enemyData.MaxHealth = i < 1 ? 6 : i < 3 ? 10 : i < 7 ? 15 : 20;
            level.numDice = i < 1 ? 1 : i < 3 ? 2 : i < 7 ? 3 : 4;
            levels.Add(level);
        }
        LevelData levelBoss = new LevelData();
        levelBoss.enemyData = (Enemy)MigsData[1].Clone();
        levelBoss.enemyData.MaxHealth = 30;
        levelBoss.numDice = 4;
        levels.Add(levelBoss);
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

[System.Serializable]
public class LevelData {

    public Enemy enemyData;
    public int numDice;

}