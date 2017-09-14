namespace Adnc.QuickParallax {
    public enum ResetTriggerAction {
        OnTriggerEnter,
        OnTriggerExit
    }

    internal static class ResetTriggerActionClass {
        public static bool IsTriggerEnter (this ResetTriggerAction action) {
            return action == ResetTriggerAction.OnTriggerEnter;
        }

        public static bool IsTriggerExit (this ResetTriggerAction action) {
            return action == ResetTriggerAction.OnTriggerExit;
        }
    }
}