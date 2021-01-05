using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private TMPro.TextMeshProUGUI playerText;
    [SerializeField] private TMPro.TextMeshProUGUI enemyText;

    private List<DialogData> dialogs;
    private Coroutine coroutine;

    #endregion

    #region Public methods

    public void Initialize(List<DialogData> dialogs) {
        HideAllText();
        this.dialogs = new List<DialogData>(dialogs);
        ShowNextText();
    }

    public void StopAllTexts() {
        HideAllText();
        if (coroutine != null) {
            StopCoroutine(coroutine);
        }
        coroutine = null;
    }

    #endregion

    #region Private methods

    private void ShowNextText() {
        StopAllTexts();
        if (dialogs != null && dialogs.Count > 0) {
            DialogData dialog = dialogs[0];
            dialogs.RemoveAt(0);
            ShowText(dialog.WaitTime, dialog.TextTime, dialog.Dialog, dialog.IsPlayer);
        }
    }

    private void ShowText(float waitTime, float time, string text, bool isPlayer) {
        StopAllTexts();
        coroutine = StartCoroutine(_ShowText(waitTime, time, text, isPlayer));
    }

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

    private IEnumerator _ShowText(float waitTime ,float time,string text, bool isPlayer) {

        yield return new WaitForSeconds(waitTime);

        if (isPlayer) ShowPlayerText(text);
        else ShowEnemyText(text);

        yield return new WaitForSeconds(time); 
        coroutine = null;
        ShowNextText();
    }

    #endregion

}
