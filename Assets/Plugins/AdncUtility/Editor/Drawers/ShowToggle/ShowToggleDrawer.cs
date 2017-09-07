using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Adnc.Utility.Drawers {
    [CustomPropertyDrawer(typeof(ShowToggleAttribute))]
    public class ShowToggleDrawer : PropertyDrawer {
        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            var toggle = (ShowToggleAttribute)attribute;
            var isVisible = GetIsVisible(toggle, property);

            if (isVisible) {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            if (toggle.invalidDisplay == ShowToggleDisplay.Hide) return;

            if (toggle.invalidDisplay == ShowToggleDisplay.Disable) {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = true;
            }
        }

        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
            var toggle = (ShowToggleAttribute)attribute;
            var isVisible = GetIsVisible(toggle, property);

            if (isVisible || toggle.invalidDisplay == ShowToggleDisplay.Disable) {
                return base.GetPropertyHeight(property, label);
            }

            return 0;
        }

        bool GetIsVisible (ShowToggleAttribute toggle, SerializedProperty property) {
            var propertyPath = property.propertyPath;
            var conditionPath = propertyPath.Replace(property.name, toggle.fieldName);
            var condition = property.serializedObject.FindProperty(conditionPath);

            if (condition != null) {
                return condition.boolValue == toggle.requiredValue;
            }

            Debug.LogWarningFormat("[ShowToggle] could not find attribute {0}", toggle.fieldName);

            return true;
        }
    }
}
