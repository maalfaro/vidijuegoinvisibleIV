using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    private void Start() {
        SoundsManager.Instance.StopMusic();
    }

    public void GoToMenu() {
        SoundsManager.Instance.PlaySound("click");
        Core.Instance.GoToMenu();
    }

    public void ExitGame() {
        SoundsManager.Instance.PlaySound("click");
        Application.Quit();
    }
}
