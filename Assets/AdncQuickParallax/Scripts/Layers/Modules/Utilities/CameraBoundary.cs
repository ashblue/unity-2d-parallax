
using UnityEngine;

namespace Adnc.QuickParallax.Modules.Utilities {
    public class CameraBoundary {
        private int _frameCache;
        private Bounds _bounds = new Bounds();

        void UpdateBoundary () {
            var cam = Camera.main;

            if (cam == null) return;

            var vertExtent = cam.orthographicSize;
            var horzExtent = vertExtent * Screen.width / Screen.height;
            _bounds.size = new Vector3(horzExtent * 2, vertExtent * 2, 100);

            var pos = cam.transform.position;
            pos.z = 0;
            _bounds.center = pos;
        }

        public Bounds GetBounds () {
            if (_frameCache != Time.frameCount) {
                UpdateBoundary();
                _frameCache = Time.frameCount;
            }

            return _bounds;
        }

        public void GizmoDrawBoundary () {
            if (Application.isPlaying) {
                var b = GetBounds();
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(b.center, b.size);
            }
        }
    }
}