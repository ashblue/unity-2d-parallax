using UnityEngine;
using System.Collections;

namespace Adnc.Parallax {
	public class ParallaxLayer : MonoBehaviour {
		public Vector3 originPos; // Captured original position in-case we want to reset it

		void Awake () {
			originPos = transform.position;
			Parallax2D.parallaxLayers.Add(this);
		}
	}
}

