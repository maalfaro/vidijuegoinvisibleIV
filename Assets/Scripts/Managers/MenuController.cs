using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    #region Variables

    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;

    #endregion

    #region Monobehaviour methods

    private void Start() {
        exitButton.onClick.AddListener(Application.Quit);
        playButton.onClick.AddListener(Core.Instance.StartGame);
    }

    #endregion


}
