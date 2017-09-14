using System.Collections.Generic;
using UnityEngine;

namespace Adnc.QuickParallax.Modules {
    /// <summary>
    /// Base class for handling a single nested layer
    /// </summary>
    public abstract class ParallaxLayerModuleSingleBase : ParallaxLayerModuleBase {
        private List<ParallaxLayer> _layers = new List<ParallaxLayer>();

        [Tooltip("Leave blank to auto-retrieve the nearest nested or sibling layer")]
        [SerializeField]
        private ParallaxLayer _layer;

        public override List<ParallaxLayer> Layers {
            get { return _layers; }
            protected set { _layers = value; }
        }

        protected override void Awake () {
            if (_layer == null) {
                _layer = GetComponentInChildren<ParallaxLayer>();
            }

            if (_layer != null) {
                _layers.Add(_layer);
            }

            base.Awake();
        }
    }
}