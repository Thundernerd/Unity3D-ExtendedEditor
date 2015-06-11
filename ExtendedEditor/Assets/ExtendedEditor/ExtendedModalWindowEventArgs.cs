#if UNITY_EDITOR
using System;

namespace TNRD.Editor {
	public class ExtendedModalWindowEventArgs : EventArgs {

		public ExtendedModalWindow Window { get; private set; }
		public EExtendedModalWindowResult Result { get; private set; }

		public ExtendedModalWindowEventArgs() : base() { }

		public ExtendedModalWindowEventArgs( ExtendedModalWindow window, EExtendedModalWindowResult result ) : this() {
			Window = window;
			Result = result;
		}
	}
}
#endif