using Adnc.Utility;
using UnityEngine;

namespace Adnc.QuickParallax.Modules {
    public class LayerBoundary : ParallaxLayerModuleMultiBase {
        private const string ACTION_TOOLTIP = "Action taken when a layer overflows outside the boundary. None = Do nothing." +
                                              " Contain = Keep the layer from overflowing outside. RecycleOnOppositeSide =" +
                                              " Recycles the element to the opposite side (requires a boundary on the opposite side)";

        private const string DISTANCE_TOOLTIP = "Distance of the axis before overflow";

        private Bounds _bounds;

        [Tooltip("The center of the boundary")]
        [SerializeField]
        private Vector2 _offset;

        [Tooltip(ACTION_TOOLTIP)]
        [SerializeField]
        private BoundaryOverride _xMinOverflow;

        [ShowToggle("_xMinOverflow", new[] { 0 }, ShowToggleDisplay.Show, ShowToggleDisplay.Hide)]
        [Tooltip(DISTANCE_TOOLTIP)]
        [SerializeField]
        private float _xMin = 10;

        [Tooltip(ACTION_TOOLTIP)]
        [SerializeField]
        private BoundaryOverride _xMaxOverflow;

        [ShowToggle("_xMaxOverflow", new[] { 0 }, ShowToggleDisplay.Show, ShowToggleDisplay.Hide)]
        [Tooltip(DISTANCE_TOOLTIP)]
        [SerializeField]
        private float _xMax = 10;

        [Tooltip(ACTION_TOOLTIP)]
        [SerializeField]
        private BoundaryOverride _yMinOverflow;

        [ShowToggle("_yMinOverflow", new[] { 0 }, ShowToggleDisplay.Show, ShowToggleDisplay.Hide)]
        [Tooltip(DISTANCE_TOOLTIP)]
        [SerializeField]
        private float _yMin = 10;

        [Tooltip(ACTION_TOOLTIP)]
        [SerializeField]
        private BoundaryOverride _yMaxOverflow;

        [ShowToggle("_yMaxOverflow", new[] { 0 }, ShowToggleDisplay.Show, ShowToggleDisplay.Hide)]
        [Tooltip(DISTANCE_TOOLTIP)]
        [SerializeField]
        private float _yMax = 10;

        public float XMin {
            get { return _xMinOverflow.IsOverride() ? _xMin : 0; }
        }

        public float XMax {
            get { return _xMaxOverflow.IsOverride() ? _xMax : 0; }
        }

        public float YMin {
            get { return _yMinOverflow.IsOverride() ? _yMin : 0; }
        }

        public float YMax {
            get { return _yMaxOverflow.IsOverride() ? _yMax : 0; }
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
            return UpdateBounds(new Bounds());
        }

        Bounds UpdateBounds (Bounds bounds) {
            var pos = transform.position;
            pos.x += _offset.x + (XMax - XMin) / 2;
            pos.y += _offset.y + (YMax - YMin) / 2;
            bounds.center = pos;

            bounds.size = new Vector2(Width, Height);

            return bounds;
        }

        protected override void OnUpdateModule (ParallaxLayer layer) {
            _bounds = UpdateBounds(_bounds);

            var pos = layer.transform.position;
            var b = layer.GetBounds();

            if (_xMinOverflow.IsOverride() && b.min.x < _bounds.min.x) {
                if (_xMinOverflow == BoundaryOverride.Contain) {
                    pos.x += _bounds.min.x - b.min.x;
                } else if (_xMinOverflow == BoundaryOverride.RecycleOnOppositeSide && _xMaxOverflow.IsOverride()) {
                    pos.x = _bounds.max.x;
                }
            }

            if (_xMaxOverflow.IsOverride() && b.max.x > _bounds.max.x) {
                if (_xMaxOverflow == BoundaryOverride.Contain) {
                    pos.x += _bounds.max.x - b.max.x;
                } else if (_xMaxOverflow == BoundaryOverride.RecycleOnOppositeSide && _xMinOverflow.IsOverride()) {
                    pos.x = _bounds.min.x;
                }
            }

            if (_yMinOverflow.IsOverride() && b.min.y < _bounds.min.y) {
                if (_yMinOverflow == BoundaryOverride.Contain) {
                    pos.y += _bounds.min.y - b.min.y;
                } else if (_yMinOverflow == BoundaryOverride.RecycleOnOppositeSide && _yMaxOverflow.IsOverride()) {
                    pos.y = _bounds.max.y;
                }
            }

            if (_yMaxOverflow.IsOverride() && b.max.y > _bounds.max.y) {
                if (_yMaxOverflow == BoundaryOverride.Contain) {
                    pos.y += _bounds.max.y - b.max.y;
                } else if (_yMaxOverflow == BoundaryOverride.RecycleOnOppositeSide && _yMinOverflow.IsOverride()) {
                    pos.y = _bounds.min.y;
                }
            }

            layer.transform.position = pos;
        }

        private void OnDrawGizmos () {
            if (ParallaxLayerController.Current == null || !ParallaxLayerController.Current.DebugEnabled) {
                return;
            }

            DrawBounds();
        }

        private void OnDrawGizmosSelected () {
            if (ParallaxLayerController.Current == null || !ParallaxLayerController.Current.DebugEnabled) {
                DrawBounds();
            }
        }

        void DrawBounds () {
            Gizmos.color = ParallaxSettings.Current.BoundaryColor;

            var b = GetBounds();
            const float MIN_LINE_LENGTH = 3f;

            if (_xMinOverflow.IsOverride()) {
                Gizmos.DrawLine(
                    new Vector2(b.min.x, b.center.y + MIN_LINE_LENGTH),
                    new Vector2(b.min.x, b.center.y - MIN_LINE_LENGTH));
            }

            if (_xMaxOverflow.IsOverride()) {
                Gizmos.DrawLine(
                    new Vector2(b.max.x, b.center.y + MIN_LINE_LENGTH),
                    new Vector2(b.max.x, b.center.y - MIN_LINE_LENGTH));
            }

            if (_yMinOverflow.IsOverride()) {
                Gizmos.DrawLine(
                    new Vector2(b.center.x + MIN_LINE_LENGTH, b.min.y),
                    new Vector2(b.center.x - MIN_LINE_LENGTH, b.min.y));
            }

            if (_yMaxOverflow.IsOverride()) {
                Gizmos.DrawLine(
                    new Vector2(b.center.x + MIN_LINE_LENGTH, b.max.y),
                    new Vector2(b.center.x - MIN_LINE_LENGTH, b.max.y));
            }
        }
    }
}