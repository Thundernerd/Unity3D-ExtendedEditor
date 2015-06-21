#if UNITY_EDITOR
using UnityEngine;

namespace TNRD.Editor.Core {
	public class ExtendedNotification {
		public GUIContent Text;
		public Color @Color;
		public float Duration;
		public readonly Vector2 Size;

		public ExtendedNotification( string text, Color color, float duration, GUIStyle style ) {
			Text = new GUIContent( text );
			Color = color;
			Duration = duration;

			style.CalcMinMaxWidth( Text, out Size.y, out Size.x );
			Size.y = style.CalcHeight( Text, Size.x );
		}
	}
}
#endif