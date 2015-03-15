#if UNITY_EDITOR
using UnityEngine;

namespace TNRD {

	// Taken from https://github.com/thecodejunkie/unity.resources (Keyboard)
	public class KeyInfo {
		/// <summary>
		/// Initializes a new instance of the <see cref="Event"/> class,
		/// from the provided event <paramref name="evt"/>.
		/// </summary>
		/// <param name="evt">An <see cref="Event"/> instance.</param>
		public KeyInfo( Event evt ) {
			this.IsAlt = evt.alt;
			this.IsCapsLock = evt.capsLock;
			this.IsControl = evt.control;
			this.IsFunctionKey = evt.functionKey;
			this.IsNumeric = evt.numeric;
			this.IsShift = evt.shift;
			this.Modifiers = evt.modifiers;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the alt key is pressed.
		/// </summary>
		/// <value><c>true</c> if the alt key is pressed; otherwise, <c>false</c>.</value>
		public bool IsAlt { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the caps lock key is pressed.
		/// </summary>
		/// <value><c>true</c> if the caps lock key is pressed; otherwise, <c>false</c>.</value>
		public bool IsCapsLock { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the control key is pressed.
		/// </summary>
		/// <value><c>true</c> if the control key is pressed; otherwise, <c>false</c>.</value>
		public bool IsControl { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether a function key is pressed.
		/// </summary>
		/// <value><c>true</c> if a function key is pressed; otherwise, <c>false</c>.</value>
		public bool IsFunctionKey { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether a numeric key is pressed.
		/// </summary>
		/// <value><c>true</c> if a numeric key is pressed; otherwise, <c>false</c>.</value>
		public bool IsNumeric { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the control key is pressed.
		/// </summary>
		/// <value><c>true</c> if the control key is pressed; otherwise, <c>false</c>.</value>
		public bool IsShift { get; set; }

		/// <summary>
		/// Gets or sets the modifier keys that are pressed.
		/// </summary>
		/// <value>A combination of <see cref="EventModifiers"/> values.</value>
		public EventModifiers Modifiers { get; set; }
	}
}
#endif