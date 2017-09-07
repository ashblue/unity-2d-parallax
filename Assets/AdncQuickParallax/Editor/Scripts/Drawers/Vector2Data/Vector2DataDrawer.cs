using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Adnc.QuickParallax.Editors.Drawers {
	[CustomPropertyDrawer(typeof(Vector2Data))]
	public class Vector2DataDrawer : PropertyDrawer {
		private const string PROP_VAR_NAME = "_variable";
		private const string PROP_VECTOR2_NAME = "_value";
		private const string DEFAULT_NAME = "Custom";

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
			var propVar = property.FindPropertyRelative(PROP_VAR_NAME);
			var propVarHeight = EditorStyles.popup.CalcHeight(new GUIContent("Test"), EditorGUIUtility.currentViewWidth);

			if (propVar.objectReferenceValue == null) {
				return propVarHeight + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			}

			return propVarHeight;
		}

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			// Draw label
			var rectLabel = EditorGUI.IndentedRect(position);
			rectLabel.width = EditorGUIUtility.labelWidth;

			GUI.Label(rectLabel, label);

			// Draw Menu
			var rectMenu = EditorGUI.IndentedRect(position);
			rectMenu.height = EditorGUIUtility.singleLineHeight;
			rectMenu.width -= rectLabel.width;
			rectMenu.x += rectLabel.width;

			var propVar = property.FindPropertyRelative(PROP_VAR_NAME);
			var currentVar = (Vector2Variable)propVar.objectReferenceValue;
			if (GUI.Button(rectMenu, GetVariableName(currentVar), EditorStyles.popup)) {
				var menu = GetCategoryMenu(currentVar, c => {
					propVar.objectReferenceValue = c;
					propVar.serializedObject.ApplyModifiedProperties();
				});

				menu.ShowAsContext();
			}

			if (propVar.objectReferenceValue != null) return;

			var propVector2 = property.FindPropertyRelative(PROP_VECTOR2_NAME);
			var rectVector2 = EditorGUI.IndentedRect(position);
			rectVector2.x += rectLabel.width;
			rectVector2.width -= rectLabel.width;
			rectVector2.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			EditorGUI.PropertyField(rectVector2, propVector2, GUIContent.none);
		}

		static GenericMenu GetCategoryMenu (Vector2Variable current, System.Action<Vector2Variable> callback) {
			var menu = new GenericMenu();

			menu.AddItem(
				new GUIContent(DEFAULT_NAME),
				current == null,
				() => callback(null));

			ParallaxSettings.Current.Vector2Variables.ForEach(c => {
				menu.AddItem(
					new GUIContent(GetVariableName(c)),
					current == c,
					() => callback(c));
			});

			menu.AddSeparator("");

			menu.AddItem(
				new GUIContent("Create Vector2"),
				false,
				() => Selection.activeObject = ParallaxSettings.Current);

			return menu;
		}

		static string GetVariableName (Vector2Variable variable) {
			if (variable == null) {
				return DEFAULT_NAME;
			}

			return variable.DisplayName;
		}
	}
}
