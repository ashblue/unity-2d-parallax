using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adnc.QuickParallax.Examples {
	public class ParallaxResetTest : MonoBehaviour {
		[Tooltip("Layers spawned upon reset")]
		[SerializeField]
		private GameObject _prefabParallaxLayers;

		[Tooltip("Container around all layers")]
		[SerializeField]
		private Transform _layerContainer;

		public void Start () {
			ParallaxLayerController.Current.Play();
		}

		public void Reset () {
			var ctrl = ParallaxLayerController.Current;

			ctrl.Reset();

			foreach (Transform t in _layerContainer) {
				Destroy(t.gameObject);
			}

			var layers = Instantiate(_prefabParallaxLayers);
			layers.transform.SetParent(_layerContainer);

			ParallaxLayerController.Current.Play();

		}
	}
}
