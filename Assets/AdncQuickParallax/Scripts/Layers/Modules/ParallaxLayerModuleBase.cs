using System.Collections.Generic;
using UnityEngine;

namespace Adnc.QuickParallax.Modules {
    public abstract class ParallaxLayerModuleBase : MonoBehaviour {
        // @TODO This needs to be able to target multiple child layers, could this be done with an event that fires update?
        [Tooltip("Leave blank to auto-retrieve the corresponding layer")]
        [SerializeField]
        private ParallaxLayer[] _layers;

        private void Awake () {
            if (_layers == null || _layers.Length == 0) {
                _layers = GetComponentsInChildren<ParallaxLayer>();
            }

            if (_layers == null || _layers.Length == 0) {
                Debug.LogError("A ParallaxLayer is required to use this module");
                return;
            }

            foreach (var layer in _layers) {
                layer.TriggerSetup += Setup;
                layer.TriggerUpdate += UpdateModule;
            }
        }

        public void Setup (ParallaxLayer layer) {
            OnSetup(layer);
        }

        protected virtual void OnSetup (ParallaxLayer layer) {
        }

        public void UpdateModule (ParallaxLayer layer) {
            OnUpdateModule(layer);
        }

        protected virtual void OnUpdateModule (ParallaxLayer layer) {
        }
    }
}