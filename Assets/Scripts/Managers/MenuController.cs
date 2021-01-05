using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    #region Variables

    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TMPro.TextMeshProUGUI initialText;
    [SerializeField] private CanvasGroup canvasGroupInitialText;

    #endregion

    #region Monobehaviour methods

    private void Start() {
        exitButton.onClick.AddListener(Application.Quit);
        playButton.onClick.AddListener(InitGame);
    }

    #endregion

    #region Private methods

    private void InitGame() {
        //StartCoroutine(_InitialText());
        Core.Instance.StartGame();
    }

    private IEnumerator _InitialText() {
        initialText.transform.parent.gameObject.SetActive(true);
        initialText.text = string.Empty;
        yield return _Fade(canvasGroupInitialText, 0, 1, 0.5f);
        yield return new WaitForSeconds(1f);
        initialText.text = "RULY: OH DIOS!!";
        yield return new WaitForSeconds(4f);
        initialText.text = "RULY: PERO QUE RESACA TENGO, CASI NI ME ACUERDO DE QUE HICIMOS ANOCHE...";
        yield return new WaitForSeconds(6f);
        initialText.text = "RULY: AUNQUE SI QUE RECUERDO QUE ESTABAMOS JUGANDO UNA PARTIDA DE ROL QUE ESTABA MASTEREANDO ZEKE";
        yield return new WaitForSeconds(6f);
        initialText.text = "RULY: UFFF NO SE SI FUE BUENA IDEA TOMARNOS UNOS WHISKITOS CON ÉL...";
        yield return new WaitForSeconds(6f);
        initialText.text = "RULY: AUNQUE RECUERDO QUE EN LA PARTIDA MIG MENCIONÓ ALGO DE CONVERTIR A LA GENTE EN DADOS Y ESO ME PARECIÓ MUY GRACIOSO (RENQUEO RENQUEO)";
        yield return new WaitForSeconds(8f);
        initialText.text = "RULY: AY!! MI CABEZA...";
        yield return new WaitForSeconds(4f);
        initialText.text = "(VOZ CONOCIDA): EH!.. RULY!... ¿ESTÁS DESPIERTO?... ABRE LOS OJOS HOMBRE";
        yield return new WaitForSeconds(5f);
        Core.Instance.StartGame();
    }

    private IEnumerator _Fade(CanvasGroup canvasGroup, float from, float to, float fadeTime) {
        float timer = 0.0f;
        canvasGroup.alpha = from;
        while (timer <= 1) {
            timer += Time.deltaTime / fadeTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, timer);
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    #endregion

}
