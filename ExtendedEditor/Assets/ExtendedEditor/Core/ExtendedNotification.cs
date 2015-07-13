#if UNITY_EDITOR
using UnityEngine;

namespace TNRD.Editor.Core {

	[DocsDescription("A notification popup that can be shown in a window at the bottom right corner")]
	public class ExtendedNotification {

		[DocsDescription("The text to display on the notification")]
		public GUIContent Text;

		[DocsDescription("The color to draw the text with")]
		public Color @Color;

		[DocsDescription("The duration of the notification")]
		public float Duration;

		[DocsDescription("The size of the notification (read-only)")]
		public readonly Vector2 Size;
		
		[DocsDescription("Creates an instance of ExtendedNotification")]
		[DocsParameter("text", "The text to display on the notification")]
		[DocsParameter("color", "The color of the notification")]
		[DocsParameter("duration", "The duration of the notification")]
		[DocsParameter("style", "The style of the notification")]
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