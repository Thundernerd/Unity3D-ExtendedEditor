#if UNITY_EDITOR
using System;
using TNRD.Editor.Core;

namespace TNRD.Editor.Blocks {
	public class AreaBlock : IDisposable {

		public AreaBlock( params ExtendedGUIOption[] options ) {
			ExtendedGUI.BeginArea( options );
		}

		public void Dispose() {
			ExtendedGUI.EndArea();
		}
	}
}
#endif