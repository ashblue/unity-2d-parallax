using UnityEngine;
using UnityEditor;
using System.Collections;

// @url http://www.ryan-meier.com/blog/?p=72
namespace Adnc.Parallax {
	[CustomPropertyDrawer(typeof(TagAttribute))]
	public class TagDrawer : PropertyDrawer {
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
		}
	}
}

