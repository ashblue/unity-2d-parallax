using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adnc.QuickParallax {
    public class Vector2Variable : ScriptableObject {
        public const string DEFAULT_NAME = "Untitled";

        [Tooltip("The value of this Vector2 variable")]
        [SerializeField]
        private Vector2 _value;

        [Tooltip("Name used to identify this variable")]
        [SerializeField]
        private string _displayName = "Untitled";

        public Vector2 Value {
            get { return _value; }
        }

        public string DisplayName {
            get {
                if (string.IsNullOrEmpty(_displayName)) {
                    return DEFAULT_NAME;
                }

                return _displayName;
            }
        }
    }
}
