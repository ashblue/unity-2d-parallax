using UnityEngine;
using System.Collections;

namespace Adnc.Parallax {
	public class Parallax2D : MonoBehaviour {
		[Tooltip("Tag to auto parallax elements. Only runs at startup.")]
		[TagAttribute, SerializeField] public string autoParallaxTag;

		[Tooltip("Tag to auto parallax an element that repeats (must be in a container). Only runs at startup.")]
		[TagAttribute, SerializeField] public string autoParallaxRepeatTag;

		[Tooltip("Current camera to parallax from. Will default to using the main camera if no camera is provided.")]
		[SerializeField] GameObject camera;

		[Tooltip("Focal point of the camera (used for parallax effect positioning)")]
		[SerializeField] GameObject cameraTarget;

		[Tooltip("Multiplies the parallax scroll speed")]
		[SerializeField] Vector2 parallaxSpeed;

		[Tooltip("Maximum number of repeating elements allowed (excess will be deleted)")]
		[SerializeField] int maxHistory;

		void Awake () {
			
		}

		void Reset () {

		}

		void OnDestroy () {

		} 
	}
}

