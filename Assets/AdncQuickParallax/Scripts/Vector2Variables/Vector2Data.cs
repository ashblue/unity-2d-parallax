using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adnc.QuickParallax {
	[System.Serializable]
	public class Vector2Data {
		[SerializeField]
		private Vector2 _value;

		[SerializeField]
		private Vector2Variable _variable;

		public Vector2 Value {
			get {
				if (_variable == null) {
					return _value;
				}

				return _variable.Value;
			}

			set { _value = value; }
		}
	}
}

