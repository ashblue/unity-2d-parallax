using Adnc.Utility.Editors;
using UnityEditor;
using UnityEngine;

namespace Adnc.QuickParallax.Editors.CustomEditors {
    public class ParallaxSettingsListVector2Variable : SortableListBase {
        public ParallaxSettingsListVector2Variable (Editor editor, string property, string title) : base(editor, property, title) {
            _list.elementHeightCallback = index => {
                return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
            };

            _list.drawElementCallback = (rect, index, isActive, isFocused) => {
                var element = _list.serializedProperty.GetArrayElementAtIndex(index);
                var rowA = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                var rowB = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, rect.width, EditorGUIUtility.singleLineHeight);
                var obj = new SerializedObject(element.objectReferenceValue);
                var nameProp = obj.FindProperty("_displayName");
                var valueProp = obj.FindProperty("_value");

                if (nameProp == null) {
                    Debug.LogError("No name found");
                    return;
                }

                if (valueProp == null) {
                    Debug.LogError("No value found");
                }

                EditorGUI.PropertyField(rowA, nameProp, GUIContent.none);
                EditorGUI.PropertyField(rowB, valueProp, GUIContent.none);
                obj.ApplyModifiedProperties();
            };

            _list.onAddCallback = list => {
                var v2 = ScriptableObject.CreateInstance<Vector2Variable>();
                v2.name = "Vector2Variable";
                v2.hideFlags = HideFlags.HideInHierarchy;

                AssetDatabase.AddObjectToAsset(v2, editor.target);

                var settings = (ParallaxSettings)editor.target;
                settings.Vector2Variables.Add(v2);

                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            };

            _list.onRemoveCallback = list => {
                var v2 = (Vector2Variable)_list.serializedProperty.GetArrayElementAtIndex(_list.index).objectReferenceValue;
                var settings = (ParallaxSettings)editor.target;

                settings.Vector2Variables.Remove(v2);
                Object.DestroyImmediate(v2, true);

                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            };
        }
    }
}