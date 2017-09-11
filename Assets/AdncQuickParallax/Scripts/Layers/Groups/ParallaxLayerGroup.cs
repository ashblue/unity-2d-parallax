using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adnc.QuickParallax {
	public class ParallaxLayerGroup : MonoBehaviour {
		[SerializeField]
		private ParallaxLayer[] _layers;

		private void Awake () {
			if (_layers == null || _layers.Length == 0) {
				_layers = GetChildLayers();
			}
		}

		ParallaxLayer[] GetChildLayers () {
			return GetComponentsInChildren<ParallaxLayer>(true);
		}

		public void RestoreOriginPositions () {
			foreach (var parallaxLayer in _layers) {
				if (parallaxLayer == null) continue;
				parallaxLayer.RestoreOriginPosition();
			}
		}
	}
}

