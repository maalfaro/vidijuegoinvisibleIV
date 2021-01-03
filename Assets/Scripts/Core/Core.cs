using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Core : Singleton<Core>
{

    #region Variables

    [SerializeField] private Sprite[] diceImages;

    public Card[] SelectedCards => playerData.Cards;

    [SerializeField] private List<Enemy> enemiesPrefabs;
    [SerializeField] private List<LevelData> levels;

    [SerializeField] private LevelData currentLevel;
    public LevelData CurrentLevel => currentLevel; 

 
    [SerializeField] private PlayerData playerData;
    public PlayerData PlayerData => playerData;

    #endregion

    #region Override methods

    protected override void InitInstance()
    {
        base.InitInstance();

        IntializePlayerData();
        IntializeLevels();

        GoToNextLevel();
    }

    #endregion

    #region Public methods

    public void GoToMenu()
    {
        SceneManager.LoadSceneAsync("MenuScene", LoadSceneMode.Single);
    }

    public void GoToNextLevel() {

        if (levels.Count > 0) {
            currentLevel = levels[0];
            levels.RemoveAt(0);
            SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
        } else {
            //Ir al nivel fin del juego
        }
    }

    public void StartGame() {
        IntializeLevels();
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
        //playerData.Cards = new Card[playerData.Inventory.Count];
        //for(int i=0;i< playerData.Cards.Length;i++) {
        //    playerData.Cards[i] = playerData.Inventory[i];
        //}
    }

    private void IntializeLevels() {
        levels = new List<LevelData>();
        enemiesPrefabs.Shuffle();

        for(int i = 0; i < enemiesPrefabs.Count; i++) {
            LevelData level = new LevelData();
            level.enemyData = (Enemy) enemiesPrefabs[i].Clone();
            level.numDice = i < 2 ? 2 : i < 6 ? 3 : 4;
            levels.Add(level);
        }
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