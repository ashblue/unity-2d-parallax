using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Adnc.Parallax {
	public class ParallaxLayer : MonoBehaviour {
		static float repeatPadding = 1f; // How many units to spawn a repeated tile in advance
		[HideInInspector] public Vector3 originPos; // Captured original position in-case we want to reset it

		SpriteRenderer[] repeatSprite;
		Rect rect = new Rect(); // Used to monitor the boundary of repeating parallax elements

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
		public bool repeat;

		List<List<GameObject>> buddies = new List<List<GameObject>>(); // List of all created buddies

		[Tooltip("Create a random amount of distance between each repeated item")]
		[SerializeField] bool randomDistance;
		[SerializeField] float minDistance = 2f;
		[SerializeField] float maxDistance = 7f;

		[Tooltip("Add a random offset between each repeated tile")]
		[SerializeField] bool randomYOffset;
		[SerializeField] float minYOffset = 0.2f;
		[SerializeField] float maxYOffset = 5f;

		void Awake () {
			originPos = transform.position;
			Parallax2D.parallaxLayers.Add(this);
		}

		public void ParallaxSetup () {
			if (repeat) {
				repeatSprite = GetComponentsInChildren<SpriteRenderer>();

				if (repeatSprite.Length == 0) {
					Debug.LogErrorFormat("{0} ParallaxLayer was marked as repeat, but no child element with SpriteRenderer was found to repeat. Disabling repeat.", gameObject.name);
					repeat = false;
				} else {
					Rect size = GetSize(repeatSprite);
					rect.position = size.position;
					rect.width = size.width;
					rect.height = size.height;

					List<GameObject> group = new List<GameObject>();
					foreach (SpriteRenderer sprite in repeatSprite) {
						// Create a clone of the repeatSprite at the same location
						GameObject clone = Instantiate(sprite.gameObject) as GameObject;
						clone.transform.position = sprite.transform.position;
						clone.transform.SetParent(transform);

						group.Add(clone);
						
						// Hide the sprite and use it for prefabs
						sprite.gameObject.SetActive(false);
					}
				
					buddies.Add(group);
					
					// Make sure the repeat element repeats up until it shows on the current viewing window
					while (IsNewRightBuddy()) {
						AddRightBuddy();
					}

					while (IsNewLeftBuddy()) {
						AddLeftBuddy();
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
					AddRightBuddy();
				}
				
				if (IsNewLeftBuddy()) {
					AddLeftBuddy();
				}

				if (debug) {
					ScreenRect.DrawBoundary(rect, Color.gray);
				}
			}
		}

		Rect GetSize (SpriteRenderer[] sprites) {
			float xMin = sprites[0].bounds.min.x;
			float xMax = sprites[0].bounds.max.x;
			float yMin = sprites[0].bounds.min.y;
			float yMax = sprites[0].bounds.max.y;
			
			foreach (SpriteRenderer sprite in sprites) {
				if (sprite.bounds.min.x < xMin) 
					xMin = sprite.bounds.min.x;
				if (sprite.bounds.min.y < yMin)
					yMin = sprite.bounds.min.y;
				if (sprite.bounds.max.x > xMax)
					xMax = sprite.bounds.max.x;
				if (sprite.bounds.max.y > yMax)
					yMax = sprite.bounds.max.y;
			}
			
			return Rect.MinMaxRect(xMin, yMax, xMax, yMin);
		}

		bool IsNewRightBuddy () {
			return Parallax2D.current.screen.rect.xMax + repeatPadding > rect.xMax;
		}
		
		bool IsNewLeftBuddy () {
			return Parallax2D.current.screen.rect.xMin - repeatPadding < rect.xMin;
		}

		void AddRightBuddy () {
			List<GameObject> group = new List<GameObject>();

			// Distance and offset are kept outside the loop to keep spacing consistent between grouped elements
			float distance = randomDistance ? Random.Range(minDistance, maxDistance) : 0f;
			float offset = (maxYOffset - minYOffset) / 2f; 
			Rect size = GetSize(repeatSprite);
			
			foreach (SpriteRenderer sprite in repeatSprite) {
				GameObject go = Instantiate(sprite.gameObject) as GameObject;
				go.transform.SetParent(transform);
				go.SetActive(true);
				group.Add(go);

				// Find the offset x position
				float xPos = sprite.transform.position.x - size.xMin;

				// Find the center placement of the new sprite
				Vector3 pos = sprite.transform.position;

				pos.x = rect.xMax + xPos + distance;
				pos.y += randomYOffset ? Random.Range(-offset, offset) : 0f;
				go.transform.position = pos;
			}

			buddies.Add(group);

			// History overflow check
			if (buddies.Count > Parallax2D.current.maxHistory) {
				foreach (GameObject sprite in buddies[0]) {
					Destroy(sprite);
				}
				
				buddies.RemoveAt(0);
				
				rect.xMin += size.width;
			}

			// Update wrapping rectangle
			rect.xMax += size.width + distance;
		}
		
		void AddLeftBuddy () {
			List<GameObject> group = new List<GameObject>();
			
			// Distance and offset are kept outside the loop to keep spacing consistent between grouped elements
			float distance = randomDistance ? Random.Range(minDistance, maxDistance) : 0f;
			float offset = (maxYOffset - minYOffset) / 2f; 
			Rect size = GetSize(repeatSprite);
			
			foreach (SpriteRenderer sprite in repeatSprite) {
				GameObject go = Instantiate(sprite.gameObject) as GameObject;
				go.transform.SetParent(transform);
				go.SetActive(true);
				group.Add(go);
				
				// x position offset
				float xPos = size.xMax - sprite.transform.position.x;
				
				// Find the center placement of the new sprite
				Vector3 pos = sprite.transform.position;
				
				pos.x = rect.xMin - xPos - distance;
				pos.y += randomYOffset ? Random.Range(-offset, offset) : 0f;
				go.transform.position = pos;
			}
			
			buddies.Insert(0, group);
			
			// History overflow check
			if (buddies.Count > Parallax2D.current.maxHistory) {
				foreach (GameObject sprite in buddies[buddies.Count - 1]) {
					Destroy(sprite);
				}
				
				buddies.RemoveAt(buddies.Count - 1);
				
				rect.xMax -= size.width;
			}
			
			// Update wrapping rectangle
			rect.xMin -= size.width - distance;
		}
	}
}

