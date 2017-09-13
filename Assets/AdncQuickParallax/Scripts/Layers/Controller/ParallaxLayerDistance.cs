using System.Collections.Generic;
using System.Linq;
using Adnc.Utility;
using UnityEngine;

namespace Adnc.QuickParallax {
    [System.Serializable]
    public class ParallaxLayerDistance {
        [Tooltip("Override the max element's distance (Z axis). Doing so will calculate all speeds relative to that instead of the max element.")]
        [SerializeField]
        public bool overrideMaxDistance;

        [ShowToggle("overrideMaxDistance")]
        [Tooltip("Manually inject distance of the max element")]
        [SerializeField]
        public float maxDistance;

        [Tooltip("Speed of the max parallax layer. All background parallaxing speeds will be relative to this")]
        [SerializeField]
        public Vector2Data maxSpeed = new Vector2Data();

        public void SetSpeedByFurthest (List<ParallaxLayer> layers) {
            var filter = layers.Where(l => l != null && l.transform.position.z > 0)
                .OrderBy(l => l.transform.position.z)
                .ToList();

            SetSpeeds(filter);
        }

        public void SetSpeedByClosest (List<ParallaxLayer> layers) {
            var filter = layers.Where(l => l != null && l.transform.position.z < 0)
                .OrderByDescending(l => l.transform.position.z)
                .ToList();

            SetSpeeds(filter);
        }

        void SetSpeeds (List<ParallaxLayer> layers) {
            if (layers.Count == 0) return;

            var distance = maxDistance;
            if (!overrideMaxDistance) {
                distance = layers.Last().transform.position.z;
            }

            layers.ForEach(layer => {
                var percent = Mathf.Abs(layer.Distance / distance);
                var x = percent * maxSpeed.Value.x;
                var y = percent * maxSpeed.Value.y;

                layer.moveSpeed.SetValue(new Vector2(x, y));
            });
        }
    }
}