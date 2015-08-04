#if UNITY_EDITOR
using UnityEngine;

namespace TNRD.Editor.Core {

    /// <summary>
    /// Options that can be applied to ExtendedGUI's Area
    /// </summary>
    public class ExtendedGUIOption {

        internal enum EType {
            Width,
            Height,
            HorizontalPosition,
            VerticalPosition,
            Position,
            Size,
            WindowSize
        }

        internal EType Type;

        internal object Value;

        internal ExtendedGUIOption() { }

        /// <summary>
        /// Option for a custom width
        /// </summary>
        /// <param name="value">The value for the width</param>
        public static ExtendedGUIOption Width( float value ) {
            return new ExtendedGUIOption() { Type = EType.Width, Value = value };
        }

        /// <summary>
        /// Option for a custom height
        /// </summary>
        /// <param name="value">The value for the height</param>
        public static ExtendedGUIOption Height( float value ) {
            return new ExtendedGUIOption() { Type = EType.Height, Value = value };
        }

        /// <summary>
        /// Option for a custom x position
        /// </summary>
        /// <param name="value">The value for the x position</param>
        public static ExtendedGUIOption HorizontalPosition( float value ) {
            return new ExtendedGUIOption() { Type = EType.HorizontalPosition, Value = value };
        }

        /// <summary>
        /// Option for a custom y position
        /// </summary>
        /// <param name="value">The value for the y position</param>
        public static ExtendedGUIOption VerticalPosition( float value ) {
            return new ExtendedGUIOption() { Type = EType.VerticalPosition, Value = value };
        }

        /// <summary>
        /// Option for a custom position
        /// </summary>
        /// <param name="value">The value for the position</param>
        public static ExtendedGUIOption Position( Vector2 value ) {
            return new ExtendedGUIOption() { Type = EType.Position, Value = value };
        }

        /// <summary>
        /// Option for a custom position
        /// </summary>
        /// <param name="x">The x value for the position</param>
        /// <param name="y">The y value for the position</param>
        public static ExtendedGUIOption Position( float x, float y ) {
            return new ExtendedGUIOption() { Type = EType.Position, Value = new Vector2( x, y ) };
        }

        /// <summary>
        /// Option for a custom size
        /// </summary>
        /// <param name="value">The value for the size</param>
        public static ExtendedGUIOption Size( Vector2 value ) {
            return new ExtendedGUIOption() { Type = EType.Size, Value = value };
        }

        /// <summary>
        /// Option for a custom size
        /// </summary>
        /// <param name="x">The x value for the size</param>
        /// <param name="y">The y value for the size</param>
        public static ExtendedGUIOption Size( float x, float y ) {
            return new ExtendedGUIOption() { Type = EType.Size, Value = new Vector2( x, y ) };
        }
    }
}
#endif