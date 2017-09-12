using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adnc.QuickParallax {
    public class ParallaxLayerController : MonoBehaviour {
        private static ParallaxLayerController _current;

        private Coroutine _loopLayers;
        private bool _isSetup;
        private Transform _trackedTransform;

        [Tooltip("Enable debug mode for all layers")]
        [SerializeField]
        private bool _debug;

        [Tooltip("Automatically setup this parallax system when it's activated")]
        [SerializeField]
        bool _autoInit = true;

        [Tooltip("Manually assign a tracking object. Leaving this blank will follow the current main camera when started")]
        [SerializeField]
        private Transform _trackingTarget;

        private List<ParallaxLayer> _layers = new List<ParallaxLayer>();

        public static ParallaxLayerController Current {
            get { return _current; }
        }

        public Transform TrackingTarget {
            get { return _trackingTarget; }
        }

        public bool DebugEnabled {
            get { return _debug; }
        }

        private void Awake () {
            if (_current != null) {
                Debug.LogErrorFormat(
                    "Only 1x {0} script may be active at a time. Delete the current {0} script before creating a new one. Aborting.",
                    typeof(ParallaxLayerController).FullName);
                return;
            }

            _current = this;
        }

        private void Start () {
            if (_autoInit) {
                Play();
            }
        }

        private void OnEnable () {
            // Let Start() run the first instance instead of OnEnable
            if (_autoInit && _isSetup) {
                Play();
            }
        }

        private void OnDisable () {
            Stop();
        }

        public void Play () {
            Stop();
            Setup();

            _loopLayers = StartCoroutine(LoopLayers());
        }

        IEnumerator LoopLayers () {
            var prevPos = _trackedTransform.position;

            while (true) {
                var change = _trackedTransform.position - prevPos;
                prevPos = _trackedTransform.position;

                foreach (var parallaxLayer in _layers) {
                    parallaxLayer.ParallaxUpdate(change);
                }

                yield return null;
            }
        }

        public void Stop () {
            if (_loopLayers != null) {
                StopCoroutine(_loopLayers);
                _loopLayers = null;
            }
        }

        /// <summary>
        /// Resets everything to factory default and runs Stop.
        /// </summary>
        public void Reset () {
            Stop();
            _layers.Clear();
            _isSetup = false;
        }

        public void AddLayer (ParallaxLayer layer) {
            if (_isSetup) {
                Debug.LogError("Cannot add later after Setup is run");
                return;
            }

            _layers.Add(layer);
        }

        private void Setup () {
            if (_isSetup) {
                return;
            }

            _isSetup = true;

            _trackedTransform = GetTrackedTransform();

            foreach (var parallaxLayer in _layers) {
                parallaxLayer.Setup();
            }

            OnSetup();
        }

        protected virtual void OnSetup () {}

        /// <summary>
        /// Restore all layer positions to their original starting point
        /// </summary>
        public void RestoreOriginPositions () {
            foreach (var parallaxLayer in _layers) {
                if (parallaxLayer == null) continue;
                parallaxLayer.RestoreOriginPosition();
            }
        }

        /// <summary>
        /// The object that will be tracked for changes to determine parallax movement amount
        /// </summary>
        /// <returns></returns>
        protected virtual Transform GetTrackedTransform () {
            if (_trackingTarget == null) {
                _trackingTarget = Camera.main.transform;
            }

            return _trackingTarget.transform;
        }

        private void OnDestroy () {
            if (_current == this) {
                _current = null;
            }
        }
    }
}


