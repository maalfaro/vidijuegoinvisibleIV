using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Shake : MonoBehaviour {

    #region Variables

    [Header("Settings")]
	[SerializeField] private Image image;
	[SerializeField] private float damageTime = 0.1f;
	[SerializeField] private Color damageColor;
	[SerializeField] private float shakeRange = 20f;

    #endregion

	public void BeginShake() {
		StopAllCoroutines();
		StartCoroutine(_Damage());
		StartCoroutine(_EnemyShake());
	}

	private IEnumerator _Damage() {
		Color originalColor = image.color;
		image.color = damageColor;
		yield return new WaitForSeconds(damageTime);
		image.color = originalColor;
	}

	private IEnumerator _EnemyShake() {

		float elapsed = 0.0f;
		Quaternion originalRotation = image.transform.rotation;

		while (elapsed < damageTime) {

			elapsed += Time.deltaTime;
			float z = Random.value * shakeRange - (shakeRange / 2);
			image.transform.eulerAngles = new Vector3(originalRotation.x, originalRotation.y, originalRotation.z + z);
			yield return null;
		}

		image.transform.rotation = originalRotation;
	}

}