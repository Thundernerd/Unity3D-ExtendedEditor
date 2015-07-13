#if UNITY_EDITOR
using System;

namespace TNRD.Editor.Core {

	[DocsDescription("The result arguments when a modal window gets closed")]
	public class ExtendedModalWindowEventArgs : EventArgs {

		[DocsDescription("The window that got closed")]
		public ExtendedModalWindow Window { get; private set; }

		[DocsDescription("The result of the window")]
		public EExtendedModalWindowResult Result { get; private set; }

		[DocsDescription("Creates a new instance of ExtendedModalWindowEventArgs")]
		public ExtendedModalWindowEventArgs() : base() { }

		[DocsDescription("Creates a new instance of ExtendedModalWindowEventArgs")]
		[DocsParameter("window", "The modal window")]
		[DocsParameter("result", "The result of the modal window")]
		public ExtendedModalWindowEventArgs( ExtendedModalWindow window, EExtendedModalWindowResult result ) : this() {
			Window = window;
			Result = result;
		}
	}
}
#endif