using UnityEngine;
using System.Collections;

namespace Adnc.Parallax {
	public class ScreenRect {
		public Rect rect = new Rect();

		public void UpdateBoundary (Camera cam) {
			float vertExtent = cam.orthographicSize;    
			float horzExtent = vertExtent * Screen.width / Screen.height;

			rect.width = horzExtent * 2f;
			rect.height = vertExtent * 2f;
			rect.center = cam.transform.position;
		}

		static public void DrawBoundary (Rect rect, Color color) {
			Vector2 topLeft = new Vector2(rect.xMin, rect.yMax);
			Vector2 bottomRight = new Vector2(rect.xMax, rect.yMin);

			Debug.DrawLine(rect.min, topLeft, color); // Top
			Debug.DrawLine(rect.max, topLeft, color); // Left
			Debug.DrawLine(rect.min, bottomRight, color); // Bottom
			Debug.DrawLine(rect.max, bottomRight, color); // Right
		}
	}
}

