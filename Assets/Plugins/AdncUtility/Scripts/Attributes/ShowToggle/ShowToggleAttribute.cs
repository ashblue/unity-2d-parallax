using UnityEngine;

namespace Adnc.Utility {
    public class ShowToggleAttribute : PropertyAttribute {
        /// <summary>
        /// Name of the field required to show this attribute. Accepts dot notation.
        /// Example `myObj.myProp`. Must point to a field that is a boolean.
        /// </summary>
        public string fieldName;

        /// <summary>
        /// Value required by the fieldName's value when turned into a bool
        /// </summary>
        public bool requiredValue;

        /// <summary>
        /// How to handle displaying an invalid value
        /// </summary>
        public ShowToggleDisplay invalidDisplay;

        public ShowToggleAttribute (string fieldName, bool requiredValue = true, ShowToggleDisplay invalidDisplay = ShowToggleDisplay.Hide) {
            this.fieldName = fieldName;
            this.requiredValue = requiredValue;
            this.invalidDisplay = invalidDisplay;
        }
    }
}