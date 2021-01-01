using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Core : Singleton<Core>
{

    #region Variables

    [SerializeField] private Sprite[] diceImages;

    #endregion

    #region Public methods

    public void GoToMenu()
    {
        SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
    }

    public Sprite GetDiceImage(int number)
    {
        return diceImages[number - 1];
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
