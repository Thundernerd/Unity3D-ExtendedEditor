#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace TNRD.Editor.Core {
	public class ExtendedGUIOption {

		internal enum EType {
			Width,
			Height,
			HorizontalPosition,
			VerticalPosition,
			Position,
			WindowSize
		}

		internal EType Type;
		internal object Value;

		public static ExtendedGUIOption Width( float value ) {
			return new ExtendedGUIOption() { Type = EType.Width, Value = value };
		}
		public static ExtendedGUIOption Height( float value ) {
			return new ExtendedGUIOption() { Type = EType.Height, Value = value };
		}
		public static ExtendedGUIOption HorizontalPosition( float value ) {
			return new ExtendedGUIOption() { Type = EType.HorizontalPosition, Value = value };
		}
		public static ExtendedGUIOption VerticalPosition( float value ) {
			return new ExtendedGUIOption() { Type = EType.VerticalPosition, Value = value };
		}
		public static ExtendedGUIOption Position( Vector2 value ) {
			return new ExtendedGUIOption() { Type = EType.Position, Value = value };
		}
		public static ExtendedGUIOption Position( float x, float y ) {
			return new ExtendedGUIOption() { Type = EType.Position, Value = new Vector2( x, y ) };
		}
	}
}
#endif