using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Adnc.QuickParallax.Modules {
    /// <summary>
    /// Base class for handling multiple nested layers
    /// </summary>
    public abstract class ParallaxLayerModuleMultiBase : ParallaxLayerModuleBase {
        [Tooltip("Leave blank to auto-retrieve nested layers")]
        [SerializeField]
        private List<ParallaxLayer> _layers = new List<ParallaxLayer>();

        public override List<ParallaxLayer> Layers {
            get { return _layers; }
            protected set { _layers = value; }
        }

        protected override void Awake () {
            if (_layers == null || _layers.Count == 0) {
                _layers = GetComponentsInChildren<ParallaxLayer>().ToList();
            }

            base.Awake();
        }
    }
}