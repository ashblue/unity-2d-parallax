using System.Collections.Generic;
using UnityEngine;

namespace Adnc.QuickParallax.Modules {
    public abstract class ParallaxLayerModuleBase : MonoBehaviour {
        public abstract List<ParallaxLayer> Layers { get; protected set; }

        protected virtual void Awake () {
            if (Layers == null || Layers.Count == 0) {
                Debug.LogError("A ParallaxLayer is required to use this module");
                return;
            }

            foreach (var layer in Layers) {
                layer.TriggerSetup += Setup;
                layer.TriggerUpdate += UpdateModule;
            }
        }

        public void Setup (ParallaxLayer layer) {
            OnSetup(layer);
        }

        /// <summary>
        /// Triggered once when a layer is initially setup
        /// </summary>
        /// <param name="layer"></param>
        protected virtual void OnSetup (ParallaxLayer layer) {
        }

        public void UpdateModule (ParallaxLayer layer) {
            OnUpdateModule(layer);
        }

        /// <summary>
        /// Triggered each frame after a layer has been moved with the camera
        /// </summary>
        /// <param name="layer"></param>
        protected virtual void OnUpdateModule (ParallaxLayer layer) {
        }
    }
}