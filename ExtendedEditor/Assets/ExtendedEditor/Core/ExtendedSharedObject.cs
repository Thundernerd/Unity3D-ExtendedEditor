#if UNITY_EDITOR

namespace TNRD.Editor.Core {

	/// <summary>
	/// Base for shared objects that can be added to editors
	/// </summary>
	public class ExtendedSharedObject {

		/// <summary>
		/// Creates an instance of ExtendedSharedObject
		/// </summary>
		public ExtendedSharedObject() { }

		/// <summary>
		/// Called 100 times per second
		/// </summary>
		/// <param name="windowHasFocus">Does this window have focus</param>
		public virtual void Update( bool windowHasFocus ) { }
	}
}
#endif