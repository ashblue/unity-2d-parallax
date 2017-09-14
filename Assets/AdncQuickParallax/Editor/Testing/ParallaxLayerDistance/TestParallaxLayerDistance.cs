using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Adnc.QuickParallax.Editors.Testing {
    public class TestParallaxLayerDistance : TestBase {
        private ParallaxLayerDistance _layerDistance;

        [SetUp]
        public void SetupParallaxLayerDistance () {
            _layerDistance = new ParallaxLayerDistance();
        }

        [Test]
        public void SetSpeedByFurthestSetsLayersToCorrectSpeed () {
            const float speed = 10;
            _layerDistance.maxSpeed = new Vector2Data(speed, speed);

            var layers = GetLayer(new List<float> { 3, 1, 7 });
            _layerDistance.SetSpeedByFurthest(layers);

            layers.ForEach(l => {
                Assert.AreEqual(speed * (l.Distance / 7), l.moveSpeed.Value.x);
                Assert.AreEqual(speed * (l.Distance / 7), l.moveSpeed.Value.y);
            });
        }

        [Test]
        public void SetSpeedByFurthestOverrideMaxDistance () {
            const float speed = 10;
            const float maxDistance = 11;
            _layerDistance.maxSpeed = new Vector2Data(speed, speed);
            _layerDistance.overrideMaxDistance = true;
            _layerDistance.maxDistance = maxDistance;

            var layers = GetLayer(new List<float> { 3, 1, 7 });
            _layerDistance.SetSpeedByFurthest(layers);

            layers.ForEach(l => {
                Assert.AreEqual(Mathf.Round(speed * (l.Distance / maxDistance)), Mathf.Round(l.moveSpeed.Value.x));
                Assert.AreEqual(Mathf.Round(speed * (l.Distance / maxDistance)), Mathf.Round(l.moveSpeed.Value.y));
            });
        }

        [Test]
        public void SetSpeedByClosestOverrideMaxDistance () {
            const float speed = -10;
            const float maxDistance = -11;
            _layerDistance.maxSpeed = new Vector2Data(speed, speed);
            _layerDistance.overrideMaxDistance = true;
            _layerDistance.maxDistance = maxDistance;

            var layers = GetLayer(new List<float> { -3, -1, -7 });
            _layerDistance.SetSpeedByClosest(layers);

            layers.ForEach(l => {
                Assert.AreEqual(Mathf.Round(speed * Mathf.Abs(l.Distance / maxDistance)), Mathf.Round(l.moveSpeed.Value.x));
                Assert.AreEqual(Mathf.Round(speed * Mathf.Abs(l.Distance / maxDistance)), Mathf.Round(l.moveSpeed.Value.y));
            });
        }

        [Test]
        public void SetSpeedByClosestSetsLayersToCorrectSpeed () {
            const float speed = -10;
            _layerDistance.maxSpeed = new Vector2Data(speed, speed);

            var layers = GetLayer(new List<float> { -3, -1, -7 });
            _layerDistance.SetSpeedByClosest(layers);

            layers.ForEach(l => {
                Assert.AreEqual(speed * Mathf.Abs(l.Distance / 7), l.moveSpeed.Value.x);
                Assert.AreEqual(speed * Mathf.Abs(l.Distance / 7), l.moveSpeed.Value.y);
            });
        }

        List<ParallaxLayer> GetLayer (List<float> distances) {
            return distances.Select(d => GetLayer(d)).ToList();
        }

        ParallaxLayer GetLayer (float distance) {
            var layer = new GameObject("Layer").AddComponent<ParallaxLayer>();
            layer.Distance = distance;

            return layer;
        }
    }
}
