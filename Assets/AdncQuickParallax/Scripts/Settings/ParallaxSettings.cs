using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adnc.QuickParallax {
	[CreateAssetMenu(fileName = "ParallaxSettings", menuName = "ADNC/Parallax/Settings", order = 1)]
	public class ParallaxSettings : ScriptableObject {
		private static ParallaxSettings _current;
		private const string RESOURCE_PATH = "ParallaxSettings";

		[Tooltip("Color used to outline individual sprite tiles")]
		public Color tileColor = Color.magenta;

		[Tooltip("Color painted around the camera's boundary")]
		public Color cameraColor = Color.yellow;

		[HideInInspector]
		[SerializeField]
		private List<Vector2Variable> _vector2Variables = new List<Vector2Variable>();

		public List<Vector2Variable> Vector2Variables {
			get { return _vector2Variables; }
		}

		public static ParallaxSettings Current {
			get {
				if (_current == null) {
					_current = Resources.Load<ParallaxSettings>(RESOURCE_PATH);
					Debug.AssertFormat(
						_current != null,
						"Could not load {1}. Please verify a {1} object is at `Resources/{0}'. If not please create one.",
						RESOURCE_PATH,
						typeof(ParallaxSettings).FullName);
				}

				return _current;
			}
		}
	}
}
