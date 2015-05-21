using UnityEngine;
using System.Collections;
using System;

namespace TNRD {
	public class TestDraggableControl : SelectableControl {

		public override Vector2 Center() {
			return Rectangle.center;
		}

		public override bool Contains( Vector2 value ) {
			return Rectangle.Contains( value );
		}

		private Color color = Color.red;

		public override void OnSelect() {
			base.OnSelect();
			color = Color.green;
		}

		public override void OnDeselect() {
			base.OnDeselect();
			color = Color.red;
		}

		public override void OnGUI() {
			base.OnGUI();

			var c = GUI.color;
			GUI.color = color;
			GUI.Box( Rectangle, "" );
			GUI.Box( Rectangle, "" );
			GUI.color = c;
		}
	}
}