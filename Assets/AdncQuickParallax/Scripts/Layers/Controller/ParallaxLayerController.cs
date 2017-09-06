using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adnc.Utility;
using UnityEngine;

namespace Adnc.QuickParallax {
    public class ParallaxLayerController : MonoBehaviour {
        private static ParallaxLayerController _current;

        private Coroutine _loopLayers;
        private bool _isSetup;
        private Transform _trackedTransform;

        [Tooltip("Automatically setup this parallax system when it's activated")]
        [SerializeField]
        bool _autoInit = true;

        [Tooltip("Manually assign a tracking object. Leaving this blank will follow the current main camera when started")]
        [SerializeField]
        private Transform _trackingTarget;

        private List<ParallaxLayer> _layers = new List<ParallaxLayer>();

        [Tooltip("Override settings for what is considered the furthest layer")]
        [SerializeField]
        private ParallaxLayerDistance _furthestLayer = new ParallaxLayerDistance { maxSpeed = new Vector2Data(10, 0) };

        [Tooltip("Override settings for what is considered the closest layer")]
        [SerializeField]
        private ParallaxLayerDistance _closestLayer = new ParallaxLayerDistance { maxSpeed = new Vector2Data(-2, 0) };

        public static ParallaxLayerController Current {
            get { return _current; }
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

        public void Play () {
            Setup();

            _loopLayers = StartCoroutine(LoopLayers());
        }

        IEnumerator LoopLayers () {
            yield return null;

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
        private void Reset () {
            Stop();
            _layers.Clear();
            _isSetup = false;

            OnReset();
        }

        public void AddLayer (ParallaxLayer layer) {
            if (_isSetup) {
                Debug.LogError("Cannot add later after Setup is run");
                return;
            }

            _layers.Add(layer);
        }

        protected virtual void OnReset () {}

        private void Setup () {
            if (_isSetup) {
                return;
            }

            _isSetup = true;

            _trackedTransform = GetTrackedTransform();
            _furthestLayer.SetSpeedByFurthest(_layers);
            _closestLayer.SetSpeedByClosest(_layers);

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
                return Camera.main.transform;
            }

            return _trackingTarget;
        }

        private void OnDestroy () {
            if (_current == this) {
                _current = null;
            }
        }
    }
}


