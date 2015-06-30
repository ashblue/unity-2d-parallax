using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Adnc.Parallax {
	public class ParallaxLayer : MonoBehaviour {
		static float repeatPadding = 1f; // How many units to spawn a repeated tile in advance
		Rect rect = new Rect(); // Used to monitor the boundary of repeating parallax elements
		[HideInInspector] public Vector3 originPos; // Captured original position in-case we want to reset it
		SpriteRenderer repeatSprite;

		[Tooltip("Draw a visual gizmo box around the boundary of this element")]
		public bool debug;

		[Tooltip("Multiply the parallax speed factor of this individual element.")]
		public Vector2 speedFactor = Vector2.one;

		[Header("Motorized")]
		[Tooltip("Should this layer move on its own?")]
		[SerializeField] bool motorized;

		[Tooltip("How fast should this unit move per second?")]
		[SerializeField] float moveSpeed = 0.7f;

		[Header("Repeating Image")]
		[Tooltip("Should the inner graphic be repeated in camera view?")]
		[SerializeField] bool repeat;

		List<GameObject> buddies = new List<GameObject>(); // List of all created buddies

		[Tooltip("Create a random amount of distance between each repeated item")]
		[SerializeField] bool randomDistance;
		[SerializeField] float minDistance = 2f;
		[SerializeField] float maxDistance = 7f;
//
//		[SerializeField] bool randomYOffset;
//		[SerializeField] float minYOffset = 0.2f;
//		[SerializeField] float maxYOffset = 5f;

		void Awake () {
			originPos = transform.position;
			Parallax2D.parallaxLayers.Add(this);
		}

		public void ParallaxSetup () {
			if (repeat) {
				repeatSprite = GetComponentInChildren<SpriteRenderer>();
				if (repeatSprite.gameObject == gameObject) repeatSprite = null; // Ignore parent element
				
				if (repeatSprite == null) {
					Debug.LogError("ParallaxLayer was marked as repeat, but no child element with SpriteRenderer was found to repeat. Disabling repeat.");
					repeat = false;
				} else {
					rect.width = repeatSprite.bounds.size.x;
					rect.height = repeatSprite.bounds.size.y;
					rect.center = repeatSprite.bounds.center;

					// Create a clone of the repeatSprite at the same location
					GameObject clone = Instantiate(repeatSprite.gameObject) as GameObject;
					clone.transform.position = repeatSprite.transform.position;
					buddies.Add(clone);

					// Hide the repeatSprite and use it for prefabs
					repeatSprite.gameObject.SetActive(false);
					
					// Make sure the repeat element repeats up until it shows on the current viewing window
					while (IsNewRightBuddy()) {
						AddRightBuddy(repeatSprite);
					}

					while (IsNewLeftBuddy()) {
						AddLeftBuddy(repeatSprite);
					}
				}
			}
		}

		Vector3 tmpPos; // Temp vars for storing loop values
		float tmpSpeed;
		public void ParallaxUpdate (Vector2 change) {
			if (motorized) {
				tmpPos = transform.position;
				tmpSpeed = moveSpeed * Time.deltaTime;

				tmpPos.x += tmpSpeed;

				transform.position = tmpPos;
				change.x += tmpSpeed;
			}

			if (repeat) {
				// Reposition the rectangle to wrap the elements correctly
				rect.position += change;

				if (IsNewRightBuddy()) {
					AddRightBuddy(repeatSprite);
				}
				
				if (IsNewLeftBuddy()) {
					AddLeftBuddy(repeatSprite);
				}

				if (debug) {
					ScreenRect.DrawBoundary(rect, Color.gray);
				}
			}
		}

		bool IsNewRightBuddy () {
			return Parallax2D.current.screen.rect.xMax + repeatPadding > rect.xMax;
		}
		
		bool IsNewLeftBuddy () {
			return Parallax2D.current.screen.rect.xMin - repeatPadding < rect.xMin;
		}

		void AddRightBuddy (SpriteRenderer sprite) {
			GameObject go = Instantiate(sprite.gameObject) as GameObject;
			go.transform.SetParent(transform);
			go.SetActive(true);
			buddies.Add(go);

			// History overflow check
			if (buddies.Count > Parallax2D.current.maxHistory) {
				Destroy(buddies[0]);
				buddies.RemoveAt(0);
				rect.xMin += sprite.bounds.size.x;
			}

			// Find the center placement of the new sprite
			Vector3 pos = rect.center;
			float distance = randomDistance ? Random.Range(minDistance, maxDistance) : 0f;
			pos.x = rect.xMax + sprite.bounds.extents.x + distance;
			go.transform.position = pos;

			// Update wrapping rectangle
			rect.xMax += sprite.bounds.size.x + distance;
		}

		void AddLeftBuddy (SpriteRenderer sprite) {
			GameObject go = Instantiate(sprite.gameObject) as GameObject;
			go.transform.SetParent(transform);
			go.SetActive(true);
			buddies.Insert(0, go);

			// History overflow check
			if (buddies.Count > Parallax2D.current.maxHistory) {
				Destroy(buddies[buddies.Count - 1]);
				buddies.RemoveAt(buddies.Count - 1);
				rect.xMax -= sprite.bounds.size.x;
			}
			
			// Set position
			Vector3 pos = rect.center;
			float distance = randomDistance ? Random.Range(minDistance, maxDistance) : 0f;
			pos.x = rect.xMin - sprite.bounds.extents.x - distance;
			go.transform.position = pos;
			
			// Update wrapping rectangle
			rect.xMin -= sprite.bounds.size.x - distance;
		}
	}
}

