#if UNITY_EDITOR

namespace TNRD.Editor.Core {

	[DocsDescription("Base for shared objects that can be added to editors")]
	public class ExtendedSharedObject {

		[DocsDescription("Creates an instance of ExtendedSharedObject")]
		public ExtendedSharedObject() { }

		[DocsDescription("Called 100 times per second")]
		[DocsParameter("windowHasFocus", "Does this window have focus")]
		public virtual void Update( bool windowHasFocus ) { }
	}
}
#endif