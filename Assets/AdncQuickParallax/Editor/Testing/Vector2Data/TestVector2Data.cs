using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Adnc.QuickParallax.Editors.Testing {
	public class TestVector2Data : TestBase {
		[Test]
		public void SetValueWithConstructor () {
			var v = new Vector2Data(4, 0);

			Assert.AreEqual(4, v.Value.x);
			Assert.AreEqual(0, v.Value.y);
		}

		[Test]
		public void GetValueEmpty () {
			var v = new Vector2Data();

			Assert.AreEqual(0, v.Value.x);
			Assert.AreEqual(0, v.Value.y);
		}

		[Test]
		public void GetValue () {
			var v = new Vector2Data();
			v.Value = Vector2.one;

			Assert.AreEqual(1, v.Value.x);
			Assert.AreEqual(1, v.Value.y);
		}

		[Test]
		public void GetValueFromVariable () {
			var variable = ScriptableObject.CreateInstance<Vector2Variable>();
			SetPrivateField(variable, "_value", new Vector2(3, 0));

			var v = new Vector2Data();
			SetPrivateField(v, "_variable", variable);

			Assert.AreEqual(3, v.Value.x);
			Assert.AreEqual(0, v.Value.y);
		}
	}
}

