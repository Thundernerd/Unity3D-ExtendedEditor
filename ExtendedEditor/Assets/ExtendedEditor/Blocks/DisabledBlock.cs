#if UNITY_EDITOR
using System;
using UnityEditor;

namespace TNRD {
	public class DisabledBlock : IDisposable {

		public DisabledBlock( bool disabled ) {
			EditorGUI.BeginDisabledGroup( disabled );			
		}

		public void Dispose() {
			EditorGUI.EndDisabledGroup();
		}
	}
}
#endif