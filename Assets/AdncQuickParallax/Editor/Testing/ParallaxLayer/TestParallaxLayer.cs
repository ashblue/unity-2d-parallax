using System.Collections;
using System.Collections.Generic;
using Adnc.QuickParallax.Editors.Testing;
using NUnit.Framework;
using UnityEngine;

namespace Adnc.QuickParallax {
    public class TestParallaxLayer : TestBase {
        private ParallaxLayer _layer;

        [SetUp]
        public void SetupParallaxLayer () {
            _layer = new GameObject("Layer").AddComponent<ParallaxLayer>();
        }

        [Test]
        public void ParallaxUpdateMovesBySpeed () {
            _layer.moveSpeed = new Vector2Data { Value = Vector2.one};
            _layer.ParallaxUpdate(Vector2.one);

            Assert.AreEqual(1, _layer.transform.position.x);
            Assert.AreEqual(1, _layer.transform.position.y);
        }
    }
}