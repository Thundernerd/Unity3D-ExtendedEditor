#if UNITY_EDITOR
using System;
using TNRD.Editor.Core;

namespace TNRD.Editor.Blocks {

	[DocsDescription("")]
	public class AreaBlock : IDisposable {

		[DocsDescription("Creates a new instance of AreaBlock")]
		[DocsParameter("options", "The options to apply to the area")]
		public AreaBlock( params ExtendedGUIOption[] options ) {
			ExtendedGUI.BeginArea( options );
		}

		[DocsIgnore]
		public void Dispose() {
			ExtendedGUI.EndArea();
		}
	}
}
#endif