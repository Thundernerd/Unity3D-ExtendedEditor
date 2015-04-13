#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace TNRD {
	public abstract class SelectableControl : ExtendedControl {

		public bool IsSelected;

		public virtual void OnSelect() { IsSelected = true; }
		public virtual void OnDeselect() { IsSelected = false; }

		public abstract Vector2 Center();
		public abstract bool Contains( Vector2 value );
	}
}
#endif