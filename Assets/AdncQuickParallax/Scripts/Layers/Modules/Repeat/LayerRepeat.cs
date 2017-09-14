using System;
using System.Collections.Generic;
using Adnc.QuickParallax.Modules.Utilities;
using Adnc.Utility;
using UnityEngine;

namespace Adnc.QuickParallax.Modules {
    public class LayerRepeat : ParallaxLayerModuleSingleBase {
        private bool _isDebug;

        const int X_ORIGIN = 0;
        const int Y_ORIGIN = 0;

        private Dictionary<Vector2Int, LayerRepeatBuddy> _buddyCache = new Dictionary<Vector2Int, LayerRepeatBuddy>();
        private List<LayerRepeatBuddy> _buddyActive = new List<LayerRepeatBuddy>();
        private List<LayerRepeatBuddy> _buddyRecycle = new List<LayerRepeatBuddy>();
        List<LayerRepeatBuddy> _buddyGraveyard = new List<LayerRepeatBuddy>();

        private CameraBoundary _viewBoundary = new CameraBoundary();

        private SpriteRenderer _sprite;

        [InfoBox("Automatically repeats and recycles overflowing graphics based on the current camera position. Note the" +
                 " graphic must have the pivot set to center")]

        [Tooltip("Repeat elements on the corresponding axis")]
        [SerializeField]
        private LayerRepeatType _repeat;

        public bool IsDebug {
            get { return _isDebug; }
        }

        public CameraBoundary ViewBoundary {
            get { return _viewBoundary; }
        }

        public LayerRepeatType Repeat {
            get { return _repeat; }
        }

        protected override void OnSetup (ParallaxLayer layer) {
            _sprite = layer.SpriteData;
            Debug.Assert(_sprite != null, "Layer must have a sprite in order to repeat a graphic.");
            Debug.Assert(_sprite.transform.IsChildOf(layer.transform), "Repeat image must be a child of the ParallaxLayer");
            _sprite.gameObject.SetActive(false);
        }

        protected override void OnUpdateModule (ParallaxLayer layer) {
            _isDebug = layer.DebugEnabled;

            // Update graveyard first to prevent pops in textures
            if (_buddyGraveyard.Count > 0) {
                foreach (var buddy in _buddyGraveyard) {
                    RemoveBuddy(buddy);
                }

                _buddyGraveyard.Clear();
            }

            // If no visible layer buddies, repopulate center buddy
            if (_buddyActive.Count == 0 && IsValidKeyInView(layer)) {
                // Determine the current camera center index relative
                var centerKey = GetValidKeyInView(layer);
                AddBuddy(centerKey);
            }

            for (var i = 0; i < _buddyActive.Count; i++) {
                _buddyActive[i].UpdateBuddy();
            }
        }

        bool IsValidKeyInView (ParallaxLayer layer) {
            var camBounds = _viewBoundary.GetBounds();
            var bounds = layer.GetBounds();

            switch (_repeat) {
                case LayerRepeatType.XAxis:
                    var vertYMin = WorldToAxisKey(bounds, new Vector2(camBounds.center.x, camBounds.min.y));
                    var vertYMax = WorldToAxisKey(bounds, new Vector2(camBounds.center.x, camBounds.max.y));

                    return vertYMin.y <= Y_ORIGIN && vertYMax.y >= Y_ORIGIN;
                case LayerRepeatType.YAxis:
                    var vertXMin = WorldToAxisKey(bounds, new Vector2(camBounds.min.x, camBounds.center.y));
                    var vertXMax = WorldToAxisKey(bounds, new Vector2(camBounds.max.x, camBounds.center.y));

                    return vertXMin.x <= X_ORIGIN && vertXMax.x >= X_ORIGIN;
                case LayerRepeatType.Both:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        Vector2Int GetValidKeyInView (ParallaxLayer layer) {
            var camBounds = _viewBoundary.GetBounds();
            var bounds = layer.GetBounds();

            switch (_repeat) {
                case LayerRepeatType.XAxis:
                    var vertYMin = WorldToAxisKey(bounds, new Vector2(camBounds.center.x, camBounds.min.y));
                    return new Vector2Int(vertYMin.x, Y_ORIGIN);
                case LayerRepeatType.YAxis:
                    var vertXMin = WorldToAxisKey(bounds, new Vector2(camBounds.min.x, camBounds.center.y));
                    return new Vector2Int(X_ORIGIN, vertXMin.y);
                case LayerRepeatType.Both:
                    return WorldToAxisKey(layer.GetBounds(), _viewBoundary.GetBounds().center);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void AddBuddy (Vector2Int key) {
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

        public bool HasBuddy (Vector2Int key) {
            return _buddyCache.ContainsKey(key);
        }

        public void AddToGraveyard (LayerRepeatBuddy buddy) {
            _buddyGraveyard.Add(buddy);
        }

        public Bounds GetTileBounds (Vector2Int key) {
            var pos = GetWorldPosition(key);
            return new Bounds(pos, _sprite.bounds.size);
        }

        public Vector3 GetWorldPosition (Vector2Int key, float z = 0) {
            var x = _sprite.bounds.center.x + key.x * _sprite.bounds.size.x;
            var y = _sprite.bounds.center.y + key.y * _sprite.bounds.size.y;

            return new Vector3(x, y, z);
        }

        static Vector2Int WorldToAxisKey (Bounds origin, Vector2 point) {
            return new Vector2Int(WorldToAxisX(origin, point), WorldToAxisY(origin, point));
        }

        static int WorldToAxisX (Bounds origin, Vector2 point) {
            var headingX = origin.center.x < point.x ? 1 : -1;
            var originXOffset = headingX > 0 ? -origin.extents.x : origin.extents.x;
            var distanceX = origin.center.x + originXOffset - point.x;
            var unitsXRaw = distanceX / origin.size.x * -1;
            var unitsX = headingX > 0 ? Mathf.FloorToInt(unitsXRaw) : Mathf.CeilToInt(unitsXRaw);

            return unitsX;
        }

        static int WorldToAxisY (Bounds origin, Vector2 point) {
            var headingY = origin.center.y < point.y ? 1 : -1;
            var originYOffset = headingY > 0 ? -origin.extents.y : origin.extents.y;
            var distanceY = origin.center.y + originYOffset - point.y;
            var unitsYRaw = distanceY / origin.size.y * -1;
            var unitsY = headingY > 0 ? Mathf.FloorToInt(unitsYRaw) : Mathf.CeilToInt(unitsYRaw);

            return unitsY;
        }

        private void OnDrawGizmosSelected () {
            _viewBoundary.GizmoDrawBoundary();
        }
    }
}