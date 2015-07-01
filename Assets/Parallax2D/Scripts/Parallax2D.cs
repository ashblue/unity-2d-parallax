using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Adnc.Parallax {
	/// <summary>
	/// Note that you need to adjust the script execution order and place this element higher than the ParallaxLayer element
	/// </summary>
	public class Parallax2D : MonoBehaviour {
		public static Parallax2D current;
		[SerializeField] bool debug;
		[SerializeField] bool autoInit = true;

		[Tooltip("Tag to auto parallax elements. Only runs at startup.")]
		[TagAttribute, SerializeField] public string autoParallaxTag;

		[Tooltip("Multiplies the parallax scroll speed")]
		[SerializeField] Vector2 parallaxSpeedFactor = new Vector2(1f, 1f);

		[Tooltip("Maximum number of repeating elements allowed (excess will be deleted)")]
		public int maxHistory = 20;

		[Header("Camera")]
		[Tooltip("Current camera to parallax from. Will default to using the main camera if no camera is provided.")]
		public Camera cam;

		[Header("Overrides")]
		[Tooltip("You can override what is considered the furthest away Z index. All distant elements will be parallaxed " +
			"relative this one. Leave at 0 to allow an auto max distance to be generated at run-time.")]
		[SerializeField] float defaultMaxZDistance;
		[SerializeField] float defaultMinZDistance;

		[Tooltip("Set the minimum layer speed of the background elements. 0 will mean no movement, 1 will result in same speed as the cameraTarget.")]
		[SerializeField, Range(0f, 1f)] float backgroundMaxSpeed = 1f;

		[Tooltip("Set the maximum layer speed of the foreground elements. 1 is the speed of the target, 0 would be no movement.")]
		[SerializeField, Range(0f, 1f)] float foregroundMaxSpeed = 0.2f;

		float maxZDistance = 0f; // Furthest away element
		float minZDistance = 0f; // Nearest element
		bool loop = true; // Is parallax currently active?
		Vector3 prevPos; // Location of the camera last frame
		public ScreenRect screen = new ScreenRect();
		static public List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>(); // Record of all current parallax layers

		void Awake () {
			if (current != null) {
				Debug.LogError("Only 1x Parallax2D script may be active at a time. Delete the current Parallax2D script before creating a new one. Aborting.");
				Destroy(gameObject);
				return;
			}
			
			current = this;
		}

		void Start () {
			if (autoInit) {
				Init();
			}
		}

		public void Init () {
			if (cam == null) cam = Camera.main;

			Play();
		}

		/// <summary>
		/// Clear out all elements we are parallaxing
		/// </summary>
		public void Reset () {
			parallaxLayers.Clear();
		}

		public void Play () {
			foreach (GameObject go in GameObject.FindGameObjectsWithTag(autoParallaxTag)) {
				go.AddComponent(typeof(ParallaxLayer));
			}
			
			// Loop through and discover all relative parallax distances
			foreach (ParallaxLayer layer in parallaxLayers) {
				if (layer.transform.position.z > maxZDistance) maxZDistance = layer.transform.position.z;
				if (layer.transform.position.z < minZDistance) minZDistance = layer.transform.position.z;
			}
			
			// Override discoverd distances if defaults have been provided
			if (defaultMaxZDistance != 0f) maxZDistance = defaultMaxZDistance;
			if (defaultMinZDistance != 0f) minZDistance = defaultMinZDistance;
			
			if (debug) Debug.LogFormat("Max Min: {0}, Max: {1}", minZDistance, maxZDistance);

			StopAllCoroutines();
			StartCoroutine(FollowLoop());
		}

		IEnumerator FollowLoop () {
			Vector3 targetSpeed;
			Vector2 speed;
			float speedFactor;
			Vector3 pos;

			loop = true;
			prevPos = cam.transform.position;

			// Run setup after updating the camera boundary
			screen.UpdateBoundary(cam);
			foreach (ParallaxLayer layer in parallaxLayers) {
				layer.ParallaxSetup();
			}

			if (debug) Debug.LogFormat("Layer count: {0}", parallaxLayers.Count);

			while (loop) {

				// Update our camera boundary records
				screen.UpdateBoundary(cam);
				
				if (debug) {
					ScreenRect.DrawBoundary(screen.rect, Color.green);
				}

				// Set proper speed for all elements
				targetSpeed = cam.transform.position - prevPos;

				foreach (ParallaxLayer layer in parallaxLayers) {
					if (layer.transform.position.z > 0f) {
						// Background element
						speedFactor = Mathf.Lerp(0f, backgroundMaxSpeed, layer.transform.position.z / maxZDistance);
					} else {
						// Foreground element
						speedFactor = Mathf.Lerp(0f, foregroundMaxSpeed, layer.transform.position.z / minZDistance) * -1f;
					}

					speed.x = targetSpeed.x * speedFactor * parallaxSpeedFactor.x * layer.speedFactor.x;
					speed.y = targetSpeed.y * speedFactor * parallaxSpeedFactor.y * layer.speedFactor.y;

					pos = layer.transform.position;
					pos.x += speed.x;
					pos.y += speed.y;
					layer.transform.position = pos;

					layer.ParallaxUpdate(speed);
				}

				prevPos = cam.transform.position;

				yield return null;
			}
		}

		public void RestorePositions () {
			foreach (ParallaxLayer layer in parallaxLayers) {
				layer.transform.position = layer.originPos;
			}
			// @TODO Loop through all parallax elements and restore their origin position
		}

		public void Stop () {
			loop = false;
		}

		void OnDestroy () {
			if (current == this) {
				Reset();
				current = null;
			}
		}
	}
}

