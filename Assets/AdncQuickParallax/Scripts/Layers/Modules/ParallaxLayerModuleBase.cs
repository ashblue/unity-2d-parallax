using UnityEngine;

namespace Adnc.QuickParallax.Modules {
    public abstract class ParallaxLayerModuleBase : MonoBehaviour {
        [Tooltip("Leave blank to auto-retrieve the corresponding layer")]
        [SerializeField]
        private ParallaxLayer _layer;

        private void Awake () {
            if (_layer == null) {
                _layer = GetComponent<ParallaxLayer>();
            }

            if (_layer == null) {
                Debug.LogError("A ParallaxLayer is required to use this module");
                return;
            }

            _layer.modules.Add(this);
        }

        public void Setup () {
            OnSetup();
        }

        protected virtual void OnSetup () {
        }

        public void UpdateModule () {
            OnUpdateModule();
        }

        protected virtual void OnUpdateModule () {
        }
    }
}