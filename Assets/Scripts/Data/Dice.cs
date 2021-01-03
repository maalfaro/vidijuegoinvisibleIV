using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dice : PoolObject, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

    #region Inspector variables

    [SerializeField] private Image image;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Canvas canvas;
    [SerializeField] private float movement;

    #endregion

    #region Variables

    private Vector3 initialPos;
    private int number;
    public int Number => number;

    #endregion

    #region Public methods

    public void Initialize()  {
        Initialize(Random.Range(1, 7));
    }

    public void Initialize(int number) {
        this.number = number;
        image.sprite = Core.Instance.GetDiceImage(number);
        initialPos = transform.position;
        StartCoroutine(_Move(0.3f));
    }

    #endregion

    #region Handler methods implementation

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        initialPos = transform.position;
        canvas.sortingOrder = 2;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.position = initialPos;
        canvasGroup.blocksRaycasts = true;
        canvas.sortingOrder = 1;
    }

    #endregion

    #region Pool Object implementation

    public override void GetPoolObject()
    {
        gameObject.SetActive(true);
        image.enabled = true;
    }
    public override void ReleasePoolObject()
    {
        //gameObject.SetActive(false);
        image.enabled = false;
    }

    #endregion

    #region Coroutines

    private IEnumerator _Move(float animTime) {

        canvasGroup.alpha = 0;
        yield return new WaitForSeconds(0.4f);
        initialPos = transform.position;
        Vector3 initPos = initialPos + (Vector3.down * movement);
        float timer = 0f;
        while (timer < 1) {
            timer += Time.deltaTime / animTime;
            transform.position = Vector3.Lerp(initPos, initialPos, timer);
            canvasGroup.alpha = 1;
            yield return null;
        }
        canvasGroup.alpha = 1;
        transform.position = initialPos;
    }

    #endregion

}
