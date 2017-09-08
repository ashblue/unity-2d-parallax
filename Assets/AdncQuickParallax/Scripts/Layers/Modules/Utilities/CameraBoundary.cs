
using UnityEngine;

namespace Adnc.QuickParallax.Modules.Utilities {
    public class CameraBoundary {
        private int _frameCache;
        private Bounds _bounds;

        void UpdateBoundary () {
            var cam = Camera.main;
            var vertExtent = cam.orthographicSize;
            var horzExtent = vertExtent * Screen.width / Screen.height;

            _bounds.size = new Vector3(horzExtent, vertExtent);
            _bounds.center = cam.transform.position;
        }

        public Bounds GetBounds () {
            if (_frameCache != Time.frameCount) {
                UpdateBoundary();
                _frameCache = Time.frameCount;
            }

            return _bounds;
        }
    }
}