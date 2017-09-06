using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adnc.QuickParallax.Editors.CustomEditors {
	[CustomEditor(typeof(ParallaxSettings))]
	public class ParallaxSettingsEditor : Editor {
		private ParallaxSettingsListVector2Variable _vector2Variables;

		private void OnEnable () {
			_vector2Variables = new ParallaxSettingsListVector2Variable(this, "_vector2Variables", "Vector2 Variables");
		}

		public override void OnInspectorGUI () {
			base.OnInspectorGUI();

			EditorGUILayout.HelpBox("These Vector2 variables can be customized and re-used", MessageType.Info);
			_vector2Variables.Update();
		}
	}
}

