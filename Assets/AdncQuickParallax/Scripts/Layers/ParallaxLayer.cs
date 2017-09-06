using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adnc.QuickParallax {
	public class ParallaxLayer : MonoBehaviour {
		// Speed at which this layer moves (assigned via controller)
		[NonSerialized]
		public Vector2 moveSpeed;

		private Vector3 _originPosition;

		// @TODO Ability to add a color (conditional display)
		[Tooltip("Paints a bounding box and highlights the center of this layer")]
		[SerializeField]
		private bool _debug;

		[Tooltip("Set the speed relative to the automatically assigned value. 0x0 means no movement, 1x1 means move at the assigned speed.")]
		[SerializeField]
		private Vector2Data _relativeSpeed = new Vector2Data { Value = Vector2.one };

		public float Distance {
			get { return transform.position.z; }
		}

		private void Awake () {
			_originPosition = transform.position;

			if (Math.Abs(transform.position.z) < 0.1f) {
				Debug.LogWarningFormat("The Z-Index of GameObject {0} is set to 0. Please set the Z-Index to a non-zero number. Distabling parallax on GameObject.", gameObject.name);
				return;
			}

			ParallaxLayerController.Current.AddLayer(this);
		}

		public void RestoreOriginPosition () {
			transform.position = _originPosition;
		}

		public void ParallaxUpdate (Vector2 change) {

		}
	}
}

