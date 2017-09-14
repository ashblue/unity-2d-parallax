using System.Collections;
using System.Collections.Generic;
using Adnc.Utility;
using UnityEngine;

namespace Adnc.QuickParallax {
	[RequireComponent(typeof(BoxCollider2D))]
	public class ResetParallaxLayers : MonoBehaviour {
		[InfoBox("Resets all target layers on the specified trigger event")]

		[Tooltip("What triggers the reset?")]
		[SerializeField]
		private ResetTriggerAction _trigger = ResetTriggerAction.OnTriggerEnter;

		[Header("Targets")]

		[Tooltip("Globally reset all parallax layers to their starting origin")]
		[SerializeField]
		private bool _globalReset;

		[Tooltip("Reset parallax layers to their original starting origin")]
		[SerializeField]
		private ParallaxLayer[] _layers;

		[Tooltip("Reset parallax layer groups to their original starting origin")]
		[SerializeField]
		private ParallaxLayerGroup[] _groups;

		private void OnTriggerEnter2D (Collider2D other) {
			if (!_trigger.IsTriggerEnter()) return;

			ResetLayers();
		}

		private void OnTriggerExit2D (Collider2D other) {
			if (!_trigger.IsTriggerExit()) return;

			ResetLayers();
		}

		public void ResetLayers () {
			if (_globalReset) {
				ParallaxLayerController.Current.RestoreOriginPositions();
				return;
			}

			if (_layers != null) {
				foreach (var parallaxLayer in _layers) {
					parallaxLayer.RestoreOriginPosition();
				}
			}

			if (_groups != null) {
				foreach (var parallaxLayerGroup in _groups) {
					parallaxLayerGroup.RestoreOriginPositions();
				}
			}
		}
	}
}
