using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

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
    private GameData gameData;

    public LevelData CurrentLevel => currentLevel; 
    public PlayerData PlayerData => playerData;
    public Card[] SelectedCards => playerData.Cards;


    #endregion

    #region Override methods

    protected override void InitInstance()
    {
        base.InitInstance();
        ReadGameData();
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
    }

    private void IntializeLevels() {
        levels = new List<LevelData>();
        List<Enemy> enemies = new List<Enemy>(enemiesPrefabs);

        do {
            enemies.Shuffle();
        } while (enemies.IndexOf(enemiesPrefabs[0]) < 4);

        enemies.Insert(0, MigsData[0]);
        for (int i = 0; i < enemies.Count; i++) {
            LevelData level = new LevelData();
            level.enemyData = (Enemy)enemies[i].Clone();
            level.enemyData.MaxHealth = gameData.GetMaxhealth(i);
            level.numDice = gameData.GetNumDices(i);
            levels.Add(level);
        }
        LevelData levelBoss = new LevelData();
        levelBoss.enemyData = (Enemy)MigsData[1].Clone();
        levelBoss.enemyData.MaxHealth = 30;
        levelBoss.numDice = 4;
        levels.Add(levelBoss);
    }

    private void ReadGameData() {
        string path = Path.Combine(Application.streamingAssetsPath, "gamedata.json");
#if UNITY_ANDROID
        StartCoroutine(GetAndroidStreamingAssets(path));
#else
        gameData = JsonUtility.FromJson<GameData>(File.ReadAllText(path));
#endif
    }


    private IEnumerator GetAndroidStreamingAssets(string filePath) {
        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();
        string dataAsJson = www.downloadHandler.text;
        gameData = JsonUtility.FromJson<GameData>(dataAsJson);
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

[System.Serializable] 
public class GameData {

    public int[] MaxHealthData;
    public int[] NumDiceData;

    public int GetMaxhealth(int level) {
        int healthIndex = level < MaxHealthData.Length ? level : MaxHealthData.Length - 1;
        return MaxHealthData[healthIndex];
    }

    public int GetNumDices(int level) {
        int diceIndex = level < NumDiceData.Length ? level : NumDiceData.Length - 1;
        return NumDiceData[diceIndex];
    }
}