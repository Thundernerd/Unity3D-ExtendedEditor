#if UNITY_EDITOR
using UnityEngine;

namespace TNRD.Editor.Core {

	/// <summary>
	/// A notification popup that can be shown in a window at the bottom right corner
	/// </summary>
	public class ExtendedNotification {

		/// <summary>
		/// The text to display on the notification
		/// </summary>
		public GUIContent Text;

		/// <summary>
		/// The color to draw the text with
		/// </summary>
		public Color @Color;

		/// <summary>
		/// The duration of the notification
		/// </summary>
		public float Duration;

		/// <summary>
		/// The size of the notification (read-only)
		/// </summary>
		public readonly Vector2 Size;


		/// <summary>
		/// Creates an instance of ExtendedNotification
		/// </summary>
		/// <param name="text">The text to display on the notification</param>
		/// <param name="color">The color of the notification</param>
		/// <param name="duration">The duration of the notification</param>
		/// <param name="style">The style of the notification</param>
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