using UnityEngine;
using System.Collections;

namespace Adnc {
	[System.Serializable]
	public class MinMaxFloat: MinMaxBase<float> {
		public override float GetRandom () {
			return Random.Range(min, max);
		}
	}
}

