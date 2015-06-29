using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Adnc.Parallax {
	public class Parallax2D : MonoBehaviour {
		[SerializeField] bool debug;

		[Tooltip("Tag to auto parallax elements. Only runs at startup.")]
		[TagAttribute, SerializeField] public string autoParallaxTag;

		[Tooltip("Tag to auto parallax an element that repeats (must be in a container). Only runs at startup.")]
		[TagAttribute, SerializeField] public string autoParallaxRepeatTag;

		[Tooltip("Multiplies the parallax scroll speed")]
		[SerializeField] Vector2 parallaxSpeedFactor = new Vector2(1f, 1f);

		[Tooltip("Maximum number of repeating elements allowed (excess will be deleted)")]
		[SerializeField] int maxHistory;

		[Header("Camera")]
		[Tooltip("Current camera to parallax from. Will default to using the main camera if no camera is provided.")]
		[SerializeField] Camera cam;
		
		[Tooltip("Optional focal point of the camera (all elements are parallaxed relative to this). If left blank Focus Distance will be used.")]
		[SerializeField] Transform focusTarget;

		[Tooltip("If no camera target is provided, this will be used as the z axis to relatively parallax elements from.")]
		[SerializeField] float focusDistance = 0f;

		[Header("Overrides")]
		[Tooltip("You can override what is considered the furthest away Z index. All distant elements will be parallaxed " +
			"relative this one. Leave at 0 to allow an auto max distance to be generated at run-time.")]
		[SerializeField] float defaultMaxZDistance;
		[SerializeField] float defaultMinZDistance;

		[Tooltip("Set the minimum layer speed of the background elements. 0 will mean no movement, 1 will result in same speed as the cameraTarget.")]
		[SerializeField, Range(0f, 1f)] float backgroundMinSpeed = 0f;

		[Tooltip("Set the maximum layer speed of the foreground elements. 1 is the speed of the target, 2 would be twice its speed.")]
		[SerializeField, Range(0f, 1f)] float foregroundMaxSpeed = 0.2f;


		float maxZDistance = 0f; // Furthest away element
		float minZDistance = 0f; // Nearest element
		bool loop = true; // Is parallax currently active?
		Vector3 prevPos; // Location of the camera last frame
		public static List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>(); // Record of all current parallax layers

		void Start () {
			Restart();

			// @TODO We should try to auto calculate a default speed based on max distances, need to have a fallback that can just parallax with one element
		}

		public void Restart () {
			parallaxLayers.Clear();

			foreach (GameObject go in GameObject.FindGameObjectsWithTag(autoParallaxTag)) {
				go.AddComponent(typeof(ParallaxLayer));
			}

			foreach (GameObject go in GameObject.FindGameObjectsWithTag(autoParallaxRepeatTag)) {
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
			float speed;
			float speedX;
			float speedY;
			Vector3 pos;

			prevPos = cam.transform.position;

			while (loop) {
				targetSpeed = cam.transform.position - prevPos;

				foreach (ParallaxLayer layer in parallaxLayers) {
					if (layer.transform.position.z > 0f) {
						// Background element
						speed = Mathf.Lerp(1f, backgroundMinSpeed, layer.transform.position.z / maxZDistance);
					} else {
						// Foreground element
						speed = Mathf.Lerp(1f, foregroundMaxSpeed, layer.transform.position.z / minZDistance) * -1f;
					}

					speedX = targetSpeed.x * speed;
					speedY = targetSpeed.y * speed;

					pos = layer.transform.position;
					pos.x += speedX;
					pos.y += speedY;
					layer.transform.position = pos;
				}
				
				prevPos = cam.transform.position;

				yield return null;
			}
		}

		public void Stop () {

		}

		void OnDestroy () {
			parallaxLayers.Clear();
		}
	}
}

