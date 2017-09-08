using UnityEngine;

namespace Adnc.QuickParallax.Modules {
    public class LayerRepeatBuddy : MonoBehaviour {
        private LayerRepeat _ctrl;
        public Vector2Int Id { get; private set; }

        public void Setup (LayerRepeat ctrl, Vector2Int id) {
            _ctrl = ctrl;
            Id = id;

            transform.SetParent(ctrl.transform);
            transform.localPosition = Vector3.zero;
            transform.position = ctrl.GetWorldPosition(Id, transform.position.z);
            gameObject.SetActive(true);
        }

        public void UpdateBuddy () {
            // Check if nearby slots are filled
        }

        public void Recycle () {
            gameObject.SetActive(false);
        }
    }
}