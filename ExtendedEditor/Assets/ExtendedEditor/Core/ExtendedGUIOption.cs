#if UNITY_EDITOR
using UnityEngine;

namespace TNRD.Editor.Core {

	[DocsDescription("Options that can be applied to ExtendedGUI's Area")]
	public class ExtendedGUIOption {

		[DocsIgnore]
		internal enum EType {
			Width,
			Height,
			HorizontalPosition,
			VerticalPosition,
			Position,
			Size,
			WindowSize
		}
		[DocsIgnore]
		internal EType Type;
		[DocsIgnore]
		internal object Value;

		[DocsIgnore]
		public ExtendedGUIOption() { }
		
		[DocsDescription("Option for a custom width")]
		[DocsParameter("value", "The value for the width")]
		public static ExtendedGUIOption Width( float value ) {
			return new ExtendedGUIOption() { Type = EType.Width, Value = value };
		}

		[DocsDescription("Option for a custom height")]
		[DocsParameter("value", "The value for the height")]
		public static ExtendedGUIOption Height( float value ) {
			return new ExtendedGUIOption() { Type = EType.Height, Value = value };
		}

		[DocsDescription("Option for a custom x position")]
		[DocsParameter("value", "The value for the x position")]
		public static ExtendedGUIOption HorizontalPosition( float value ) {
			return new ExtendedGUIOption() { Type = EType.HorizontalPosition, Value = value };
		}
		[DocsDescription("Option for a custom y position")]
		[DocsParameter("value", "The value for the y position")]
		public static ExtendedGUIOption VerticalPosition( float value ) {
			return new ExtendedGUIOption() { Type = EType.VerticalPosition, Value = value };
		}
		[DocsDescription("Option for a custom position")]
		[DocsParameter("value", "The value for the position")]
		public static ExtendedGUIOption Position( Vector2 value ) {
			return new ExtendedGUIOption() { Type = EType.Position, Value = value };
		}
		[DocsDescription("Option for a custom position")]
		[DocsParameter("x", "The x value for the position")]
		[DocsParameter("y", "The y value for the position")]
		public static ExtendedGUIOption Position( float x, float y ) {
			return new ExtendedGUIOption() { Type = EType.Position, Value = new Vector2( x, y ) };
		}
		[DocsDescription("Option for a custom size")]
		[DocsParameter("value", "The value for the size")]
		public static ExtendedGUIOption Size( Vector2 value ) {
			return new ExtendedGUIOption() { Type = EType.Size, Value = value };
		}
		[DocsDescription("Option for a custom size")]
		[DocsParameter("x", "The value for the size")]
		[DocsParameter("y", "The value for the size")]
		public static ExtendedGUIOption Size( float x, float y ) {
			return new ExtendedGUIOption() { Type = EType.Size, Value = new Vector2( x, y ) };
		}
	}
}
#endif