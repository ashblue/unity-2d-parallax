using System.Collections.Generic;
using Adnc.QuickParallax.Modules;
using Adnc.Utility;
using UnityEngine;

namespace Adnc.QuickParallax {
	public class ParallaxLayer : MonoBehaviour {
		private bool _isSetup;
		private Vector3 _originPosition;

		[Tooltip("If the sprite is left blank the layer will attempt to automatically find a child element")]
		[SerializeField]
		private SpriteRenderer _sprite;

		[Tooltip("Speed at which this layer moves. Can be overriden by groups with set relative speed")]
		public Vector2Data moveSpeed = new Vector2Data { Value = Vector2.one };

		[Tooltip("Set the speed relative to the assigned value. 0x0 means no movement, 1x1 means move at the assigned speed.")]
		[SerializeField]
		private Vector2Data _relativeSpeed = new Vector2Data { Value = Vector2.one };

		[Header("Debug")]

		[Tooltip("Paints a bounding box and highlights the center of this layer")]
		[SerializeField]
		private bool _debug;

		[Tooltip("Override the debug color of this layer with your own custom coloring")]
		[SerializeField]
		private bool _overrideDebugColor;

		[ShowToggle("_overrideDebugColor")]
		[Tooltip("Custom debug color")]
		[SerializeField]
		private Color _debugColor = Color.cyan;

		public event System.Action<ParallaxLayer> TriggerSetup;
		public event System.Action<ParallaxLayer> TriggerUpdate;

		public SpriteRenderer SpriteData {
			get { return _sprite; }
		}

		public bool DebugEnabled {
			get {
				return _debug || (ParallaxLayerController.Current != null && ParallaxLayerController.Current.DebugEnabled);
			}
		}

		public Color DebugColor {
			get {
				if (_overrideDebugColor) {
					return _debugColor;
				}

				return ParallaxSettings.Current.TileColor;
			}
		}

		public float Distance {
			get { return transform.position.z; }
			set {
				var pos = transform.position;
				pos.z = value;
				transform.position = pos;
			}
		}

		private void Awake () {
			_originPosition = transform.position;

			if (_sprite == null) {
				_sprite = GetComponentInChildren<SpriteRenderer>();
			}

			// @TODO Move this warning to relative parallax setup and off of the layer directly
			if (Mathf.Abs(transform.position.z) < 0.1f) {
				Debug.LogWarningFormat("The Z-Index of GameObject {0} is set to 0. Please set the Z-Index to a non-zero number. Disabling parallax on GameObject.", gameObject.name);
				return;
			}

			ParallaxLayerController.Current.AddLayer(this);
		}

		public void RestoreOriginPosition () {
			transform.position = _originPosition;
		}

		public void ParallaxUpdate (Vector2 change) {
			var speed = new Vector2(
				moveSpeed.Value.x * _relativeSpeed.Value.x,
				moveSpeed.Value.y * _relativeSpeed.Value.y);

			var pos = transform.position;
			pos.x += change.x * speed.x;
			pos.y += change.y * speed.y;

			transform.position = pos;

			if (TriggerUpdate != null) TriggerUpdate.Invoke(this);
		}

		public Bounds GetBounds () {
			if (_sprite == null) {
				return new Bounds(transform.position, Vector3.zero);
			}

			return _sprite.bounds;
		}

		public void Setup () {
			if (_isSetup) return;

			_isSetup = true;
			if (TriggerSetup != null) TriggerSetup.Invoke(this);
		}

		private void OnDrawGizmos () {
			if (!DebugEnabled) {
				return;
			}

			var c = Gizmos.color;
			Gizmos.color = DebugColor;

			if (_sprite != null) {
				Gizmos.DrawWireCube(_sprite.bounds.center, _sprite.bounds.size);
			}

			Gizmos.DrawCube(transform.position, new Vector3(0.2f, 0.2f, 0.2f));

			Gizmos.color = c;
		}

		private void OnDrawGizmosSelected () {
			if (!DebugEnabled) {
				return;
			}

#if UNITY_EDITOR
			UnityEditor.Handles.Label(
				transform.position,
				string.Format("Distance: {0}\nSpeed: {1}\nRelative Speed: {2}", Distance, moveSpeed, _relativeSpeed.Value));
#endif
		}
	}
}

