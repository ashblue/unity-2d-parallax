using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxLoadTest : MonoBehaviour {
	[SerializeField] Adnc.Parallax.Parallax2D parallaxPrefab;
	[SerializeField] UnityStandardAssets._2D.Camera2DFollow cam;

	GameObject current;
	List<Transform> ignore = new List<Transform>();

	void Awake () {
		foreach (Transform t in UnityEngine.Object.FindObjectsOfType<Transform>()) {
			ignore.Add(t);
		}
	}

	public void LoadScene (string scene) {
		StartCoroutine(LoadSceneLoop(scene));
	}

	IEnumerator LoadSceneLoop (string scene) {
		cam.gameObject.SetActive(false);

		foreach (Transform t in UnityEngine.Object.FindObjectsOfType<Transform>()) {
			if (!ignore.Contains(t)) {
				Destroy(t.gameObject);
			}
		}
		
		Application.LoadLevelAdditive(scene);

		yield return null;

		current = new GameObject();
		current.name = "Scene";
		
		foreach (Transform t in UnityEngine.Object.FindObjectsOfType<Transform>()) {
			if (!ignore.Contains(t) && t.parent == null) {
				t.SetParent(current.transform);
			}
		}
		
		// Set camera position
		GameObject player = GameObject.Find("Player");
		cam.target = player.transform;
		cam.transform.position = player.transform.position;

		cam.gameObject.SetActive(true);
		
		// No parallax 2d? Load a new one.
		if (UnityEngine.Object.FindObjectOfType<Adnc.Parallax.Parallax2D>() == null) {
			Instantiate(parallaxPrefab.gameObject);
		}

		Adnc.Parallax.Parallax2D.current.Init();
	}
}
