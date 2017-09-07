using Adnc.Utility;
using UnityEngine;

namespace Adnc.QuickParallax.Modules {
    public class LayerBoundary : ParallaxLayerModuleBase {
        private Bounds _bounds;

        [Tooltip("The center of the boundary")]
        [SerializeField]
        private Vector2 _offset;

        [Tooltip("Prevent the layer from overflowing outside the x min")]
        [SerializeField]
        private bool _limitXMin;

        [ShowToggle("_limitXMin")]
        [Tooltip("Value for the x min overflow")]
        [SerializeField]
        private float _xMin = 10;

        [SerializeField]
        private bool _limitXMax;

        [ShowToggle("_limitXMax")]
        [SerializeField]
        private float _xMax = 10;

        [SerializeField]
        private bool _limitYMin;

        [ShowToggle("_limitYMin")]
        [SerializeField]
        private float _yMin = 10;

        [SerializeField]
        private bool _limitYMax;

        [ShowToggle("_limitYMax")]
        [SerializeField]
        private float _yMax = 10;

        public float XMin {
            get { return _limitXMin ? _xMin : 0; }
        }

        public float XMax {
            get { return _limitXMax ? _xMax : 0; }
        }

        public float YMin {
            get { return _limitYMin ? _yMin : 0; }
        }

        public float YMax {
            get { return _limitYMax ? _yMax : 0; }
        }

        public float Width {
            get { return XMin + XMax; }
        }

        public float Height {
            get { return YMin + YMax; }
        }

        protected override void Awake () {
            _bounds = GetBounds();

            base.Awake();
        }

        Bounds GetBounds () {
            var bounds = new Bounds(transform.position, new Vector2(Width, Height));
            var pos = bounds.center;
            pos.x += _offset.x + (XMax - XMin) / 2;
            pos.y += _offset.y + (YMax - YMin) / 2;
            bounds.center = pos;

            return bounds;
        }

        // @TODO make sure this works on layers without a sprite
        protected override void OnUpdateModule (ParallaxLayer layer) {
            // @TODO Recycle bounds if possible (seems to crash on size change)
            _bounds = GetBounds();

            var pos = layer.transform.position;
            var b = layer.GetBounds();

            if (_limitXMin && b.min.x < _bounds.min.x) {
                pos.x += _bounds.min.x - b.min.x;
            }

            if (_limitXMax && b.max.x > _bounds.max.x) {
                pos.x += _bounds.max.x - b.max.x;
            }

            if (_limitYMin && b.min.y < _bounds.min.y) {
                pos.y += _bounds.min.y - b.min.y;
            }

            if (_limitYMax && b.max.y > _bounds.max.y) {
                pos.y += _bounds.max.y - b.max.y;
            }

            layer.transform.position = pos;
        }

        // @TODO Color should be settable from settings
        private void OnDrawGizmosSelected () {
            Gizmos.color = Color.green;

            var b = GetBounds();
            const float MIN_LINE_LENGTH = 3f;

            if (_limitXMin) {
                Gizmos.DrawLine(
                    new Vector2(b.min.x, b.center.y + MIN_LINE_LENGTH),
                    new Vector2(b.min.x, b.center.y - MIN_LINE_LENGTH));
            }

            if (_limitXMax) {
                Gizmos.DrawLine(
                    new Vector2(b.max.x, b.center.y + MIN_LINE_LENGTH),
                    new Vector2(b.max.x, b.center.y - MIN_LINE_LENGTH));
            }

            if (_limitYMin) {
                Gizmos.DrawLine(
                    new Vector2(b.center.x + MIN_LINE_LENGTH, b.min.y),
                    new Vector2(b.center.x - MIN_LINE_LENGTH, b.min.y));
            }

            if (_limitYMax) {
                Gizmos.DrawLine(
                    new Vector2(b.center.x + MIN_LINE_LENGTH, b.max.y),
                    new Vector2(b.center.x - MIN_LINE_LENGTH, b.max.y));
            }
        }
    }
}