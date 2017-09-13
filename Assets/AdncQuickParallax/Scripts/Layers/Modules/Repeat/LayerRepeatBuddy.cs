using System.Collections.Generic;
using UnityEngine;

namespace Adnc.QuickParallax.Modules {
    public class LayerRepeatBuddy : MonoBehaviour {
        private SpriteRenderer _image;
        private LayerRepeat _ctrl;
        private List<Vector2Int> _neighbors;
        public Vector2Int Id { get; private set; }

        public bool IsVisible {
            get {
                return _image.bounds.Intersects(_ctrl.ViewBoundary.GetBounds());
            }
        }

        public void Setup (LayerRepeat ctrl, Vector2Int id) {
            _ctrl = ctrl;
            Id = id;

            _neighbors = GetNeighbors();
            _image = GetComponent<SpriteRenderer>();

            transform.SetParent(ctrl.transform);
            transform.localPosition = Vector3.zero;
            transform.position = ctrl.GetWorldPosition(Id, transform.position.z);
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Polled from the parent LayerRepeat class
        /// </summary>
        public void UpdateBuddy () {
            if (!IsVisible) {
                _ctrl.AddToGraveyard(this);
                return;
            }

            foreach (var key in _neighbors) {
                if (_ctrl.HasBuddy(key)) continue;

                if (_ctrl.GetTileBounds(key).Intersects(_ctrl.ViewBoundary.GetBounds())) {
                    _ctrl.AddBuddy(key);
                }
            }
        }

        /// <summary>
        /// Triggered each time this element goes into the recycling bin
        /// </summary>
        public void Recycle () {
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos () {
            if (_ctrl.IsDebug) {
                Gizmos.color = ParallaxSettings.Current.TileColor;
                Gizmos.DrawWireCube(_image.bounds.center, _image.bounds.size);
            }
        }

        List<Vector2Int> GetNeighbors () {
            var neighbors = new List<Vector2Int>();

            if (_ctrl.Repeat == LayerRepeatType.XAxis || _ctrl.Repeat == LayerRepeatType.Both) {
                neighbors.Add(new Vector2Int(Id.x - 1, Id.y));
                neighbors.Add(new Vector2Int(Id.x + 1, Id.y));
            }

            if (_ctrl.Repeat == LayerRepeatType.YAxis || _ctrl.Repeat == LayerRepeatType.Both) {
                neighbors.Add(new Vector2Int(Id.x, Id.y - 1));
                neighbors.Add(new Vector2Int(Id.x, Id.y + 1));
            }

            return neighbors;
        }
    }
}