using System.Collections.Generic;
using System.Linq;
using Adnc.QuickParallax.Modules.Utilities;
using Adnc.Utility;
using UnityEngine;

namespace Adnc.QuickParallax.Modules {
    public class LayerRepeat : ParallaxLayerModuleSingleBase {
        private Dictionary<Vector2Int, LayerRepeatBuddy> _buddyCache = new Dictionary<Vector2Int, LayerRepeatBuddy>();
        private List<LayerRepeatBuddy> _buddyActive = new List<LayerRepeatBuddy>();
        private List<LayerRepeatBuddy> _buddyRecycle = new List<LayerRepeatBuddy>();

        private CameraBoundary _boundary = new CameraBoundary();

        private SpriteRenderer _sprite;

        [InfoBox("Automatically repeats and recycles overflowing graphics based on the current camera position")]

        [Tooltip("Repeat elements on the corresponding axis")]
        [SerializeField]
        private LayerRepeatType _repeat;

        protected override void OnSetup (ParallaxLayer layer) {
            _sprite = layer.SpriteData;
            _sprite.gameObject.SetActive(false);
        }

        protected override void OnUpdateModule (ParallaxLayer layer) {
            var camBounds = _boundary.GetBounds();
            var graphicBounds = layer.GetBounds();

            // Update all layer buddies
            // If no visible layer buddies, repopulate center buddy
            if (_buddyActive.Count != 0) return;

            // Determine the current camera center index relative
            var centerKey = WorldToAxisKey(graphicBounds, camBounds.center);
            AddBuddy(centerKey);

            // New buddy is responsible for populating all nearby buddies

            // ***** NOTES
            // Determine the index of the visible units
            // * Calculate by figuring out the number of units relative to the origin
            // * Turn min and max camera boundaries into an index range for x and y
            // Put all non-visible elements in recycling

            // Place visible units on-screen
        }

        void AddBuddy (Vector2Int key) {
            if (HasBuddy(key)) {
                return;
            }

            LayerRepeatBuddy buddy;
            if (_buddyRecycle.Count > 0) {
                buddy = _buddyRecycle[0];
                _buddyRecycle.Remove(buddy);
            } else {
                buddy = Instantiate(_sprite).gameObject.AddComponent<LayerRepeatBuddy>();
            }

            buddy.Setup(this, key);
            _buddyActive.Add(buddy);
            _buddyCache[key] = buddy;
        }

        void RemoveBuddy (LayerRepeatBuddy buddy) {
            _buddyActive.Remove(buddy);
            _buddyCache.Remove(buddy.Id);
            buddy.Recycle();

            _buddyRecycle.Add(buddy);
        }

        bool HasBuddy (Vector2Int key) {
            return _buddyCache.ContainsKey(key);
        }

        public Vector3 GetWorldPosition (Vector2Int key, float z = 0) {
            var x = _sprite.bounds.center.x + key.x * _sprite.bounds.size.x;
            var y = _sprite.bounds.center.y + key.y * _sprite.bounds.size.y;

            return new Vector3(x, y, z);
        }

        Vector2Int WorldToAxisKey (Bounds origin, Vector2 point) {
            var headingX = origin.center.x < point.x ? 1 : -1;
            var originXOffset = headingX > 0 ? -origin.extents.x : origin.extents.x;
            var distanceX = origin.center.x + originXOffset - point.x;
            var unitsXRaw = distanceX / origin.size.x * -1;
            var unitsX = headingX > 0 ? Mathf.FloorToInt(unitsXRaw) : Mathf.CeilToInt(unitsXRaw);

            var headingY = origin.center.y < point.y ? 1 : -1;
            var originYOffset = headingY > 0 ? -origin.extents.y : origin.extents.y;
            var distanceY = origin.center.y + originYOffset - point.y;
            var unitsYRaw = distanceY / origin.size.y * -1;
            var unitsY = headingY > 0 ? Mathf.FloorToInt(unitsYRaw) : Mathf.CeilToInt(unitsYRaw);

            return new Vector2Int(unitsX, unitsY);
        }
    }
}