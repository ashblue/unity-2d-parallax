using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Adnc.QuickParallax.Editors.CustomEditors {
	[CustomEditor(typeof(ParallaxLayerGroup))]
	public class ParallaxLayerGroupEditor : Editor {
		public override void OnInspectorGUI () {
			base.OnInspectorGUI();

			if (!Application.isPlaying) return;

			var t = (ParallaxLayerGroup)target;

			GUILayout.Label("Debug", EditorStyles.boldLabel);

			if (GUILayout.Button("Restore origin positions")) {
				t.RestoreOriginPositions();
			}

			if (GUILayout.Button("Move to parallax target")) {
				t.SimulateLayersFromOriginToDestination(t.transform.position, ParallaxLayerController.Current.TrackingTarget.transform.position);
			}
		}
	}
}
