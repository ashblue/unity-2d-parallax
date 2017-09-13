namespace Adnc.QuickParallax.Modules {
    public enum BoundaryOverride {
        None,
        Contain,
        RecycleOnOppositeSide
    }

    internal static class BoundaryOverrideMethods {
        public static bool IsOverride (this BoundaryOverride self) {
            return self != BoundaryOverride.None;
        }
    }
}