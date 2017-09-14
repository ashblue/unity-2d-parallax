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

		// Unity deserialization
		public Vector2Data () {}

		/// <summary>
		/// Constructor that sets the default `Vector2(x, y)` properties
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public Vector2Data (float x, float y) {
			_value.x = x;
			_value.y = y;
		}

		public Vector2 Value {
			get {
				if (_variable == null) {
					return _value;
				}

				return _variable.Value;
			}

			set { _value = value; }
		}

		public void SetValue (Vector2 value) {
			_variable = null;
			_value = value;
		}

		public void SetVariable (Vector2Variable variable) {
			_variable = variable;
		}
	}
}

