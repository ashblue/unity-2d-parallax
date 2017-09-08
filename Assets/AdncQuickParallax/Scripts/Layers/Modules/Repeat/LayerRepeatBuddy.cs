using UnityEngine;

namespace Adnc.QuickParallax.Modules {
    public class LayerRepeatBuddy : MonoBehaviour {
        private SpriteRenderer _image;
        private LayerRepeat _ctrl;
        public Vector2Int Id { get; private set; }

        public bool IsVisible {
            get {
                return _image.bounds.Intersects(_ctrl.ViewBoundary.GetBounds());
            }
        }

        public void Setup (LayerRepeat ctrl, Vector2Int id) {
            _ctrl = ctrl;
            Id = id;

            _image = GetComponent<SpriteRenderer>();

            transform.SetParent(ctrl.transform);
            transform.localPosition = Vector3.zero;
            transform.position = ctrl.GetWorldPosition(Id, transform.position.z);
            gameObject.SetActive(true);
        }

        public void UpdateBuddy () {
            if (!IsVisible) {
                Debug.Log("Send to graveyard");
                return;
            }

            // @TODO Check if nearby slots are filled
            // If not fill them
        }

        public void Recycle () {
            gameObject.SetActive(false);
        }

        private void OnDrawGizmosSelected () {
            Gizmos.color = ParallaxSettings.Current.tileColor;
            Gizmos.DrawWireCube(_image.bounds.center, _image.bounds.size);
        }
    }
}