#if UNITY_EDITOR
using System;
using TNRD.Editor.Core;

namespace TNRD.Editor.Blocks {

	/// <summary>
	/// 
	/// </summary>
	public class AreaBlock : IDisposable {

		/// <summary>
		/// Creates a new instance of AreaBlock
		/// </summary>
		/// <param name="options">The options to apply to the area</param>
		public AreaBlock( params ExtendedGUIOption[] options ) {
			ExtendedGUI.BeginArea( options );
		}

		public void Dispose() {
			ExtendedGUI.EndArea();
		}
	}
}
#endif