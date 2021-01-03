using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private TMPro.TextMeshProUGUI playerText;
    [SerializeField] private TMPro.TextMeshProUGUI enemyText;

    private Coroutine coroutine;

    #endregion

    #region Public methods

    public void Initialize() {
        HideAllText();
    }

    public void ShowText(float time, string text, bool isPlayer) {
        StopAllTexts();
        coroutine = StartCoroutine(_ShowText(time, text, isPlayer));
    }

    public void StopAllTexts() {
        if (coroutine != null) {
            StopCoroutine(coroutine);
        }
        coroutine = null;
    }

    #endregion

    #region Private methods

    private void ShowPlayerText(string text) {
        HideAllText();
        playerText.text = text;
        playerText.transform.parent.gameObject.SetActive(true);
    }

    private void ShowEnemyText(string text) {
        HideAllText();
        enemyText.text = text;
        enemyText.transform.parent.gameObject.SetActive(true);
    }

    private void HideAllText() {
        playerText.text = string.Empty;
        playerText.transform.parent.gameObject.SetActive(false);
        enemyText.text = string.Empty;
        enemyText.transform.parent.gameObject.SetActive(false);
    }

    #endregion

    #region Coroutines

    private IEnumerator _ShowText(float time,string text, bool isPlayer) {

        if (isPlayer) ShowPlayerText(text);
        else ShowEnemyText(text);

        yield return new WaitForSeconds(time);
        HideAllText();
    }

    #endregion

}
