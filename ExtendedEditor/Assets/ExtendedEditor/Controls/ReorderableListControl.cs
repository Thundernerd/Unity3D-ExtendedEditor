#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TNRD.Editor.Core;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TNRD.Editor.Controls {
	public class ReorderableListControl<T> : ExtendedControl {

		public class ReorderableListEventArgs : EventArgs {
			public readonly ReorderableList List;
			public readonly T Item;

			public ReorderableListEventArgs() : base() { }
			public ReorderableListEventArgs( ReorderableList list, T item ) : this() {
				List = list;
				Item = item;
			}
		}

		private enum EListType {
			Int,
			Float,
			Double,
			String,
			Enum
		}

		private List<T> collection;
		private string label;
		private EListType listType;
		private ReorderableList rList;

		/// <summary>
		/// Enables custom drawing for the header
		/// </summary>
		public Action<Rect> DrawHeader;
		/// <summary>
		/// Enables custom drawing for every element. Parameters are: rect, index, isActive, isFocused
		/// </summary>
		public Action<Rect, int, bool, bool> DrawElement;

		public event EventHandler<ReorderableListEventArgs> OnAddedItem;
		public event EventHandler<ReorderableListEventArgs> OnRemovedItem;

		public ReorderableListControl( string label = "" )
			: this( label, new List<T>() ) { }
		public ReorderableListControl( List<T> collection )
			: this( "", collection ) { }
		public ReorderableListControl( string label, List<T> collection ) {
			this.label = label;
			this.collection = collection;
		}

		public T this[int index] {
			get { return (T)rList.list[index]; }
			set { rList.list[index] = value; }
		}

		public float ElementHeight {
			get {
				return rList.elementHeight;
			}
			set {
				rList.elementHeight = value;
			}
		}

		public override void OnInitialize() {
			base.OnInitialize();

			var t = typeof(T);
			if ( t == typeof(int) ) {
				listType = EListType.Int;
			} else if ( t == typeof(float) ) {
				listType = EListType.Float;
			} else if ( t == typeof(double) ) {
				listType = EListType.Double;
			} else if ( t == typeof(string) ) {
				listType = EListType.String;
			} else if ( t.IsEnum ) {
				listType = EListType.Enum;
			}

			rList = new ReorderableList( new List<T>( collection ), typeof(T) );
			rList.onAddCallback = new ReorderableList.AddCallbackDelegate( AddInternal );
			rList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate( RemoveInternal );
			rList.drawElementCallback = new ReorderableList.ElementCallbackDelegate( DrawElementInternal );
			rList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate( DrawHeaderInternal );
		}

		public override void OnGUI() {
			base.OnGUI();

			Size.y = rList.headerHeight;
			Size.y += rList.elementHeight * rList.list.Count + 7; // The + 7 comes from Unity, no idea why
			Size.y += rList.footerHeight;

			if ( Input.ButtonReleased( EMouseButton.Left ) ) {
				if ( !Rectangle.Contains( Input.MousePosition ) ) {
					rList.index = -1;
				}
			}

			if ( Input.KeyReleased( KeyCode.Escape ) ) {
				rList.index = -1;
				Event.current.Use();
			}

			rList.DoList( Rectangle );
		}

		public void Add( T value ) {
			rList.list.Add( value );
		}

		public void Remove( T value ) {
			rList.list.Remove( value );
		}

		public void RemoveAt( int index ) {
			rList.list.RemoveAt( index );
		}

		public void Clear() {
			rList.list.Clear();
		}

		public List<T> Items {
			get {
				return (List<T>)rList.list;
			}
		}

		protected void AddInternal( ReorderableList list ) {
			try {
				var item = Activator.CreateInstance<T>();
				list.list.Add( item );

				if ( OnAddedItem != null ) {
					OnAddedItem.Invoke( this, new ReorderableListEventArgs( list, item ) );
				}
			} catch ( MissingMethodException) {
				list.list.Add( default(T) );
			}
		}

		private void RemoveInternal( ReorderableList list ) {
			var item = (T)list.list[list.index];

			list.list.RemoveAt( list.index );
			list.index = Mathf.Min( list.index, list.count - 1 );

			GUIUtility.hotControl = GUIUtility.keyboardControl = 0;

			if ( OnRemovedItem != null ) {
				OnRemovedItem.Invoke( this, new ReorderableListEventArgs( list, item ) );
			}
		}

		private void DrawHeaderInternal( Rect rect ) {
			if ( DrawHeader != null ) {
				DrawHeader.Invoke( rect );
			} else {
				EditorGUI.LabelField( rect, label );
			}
		}

		private void DrawElementInternal( Rect rect, int index, bool isActive, bool isFocused ) {
			if ( DrawElement != null ) {
				DrawElement.Invoke( rect, index, isActive, isFocused );
			} else {
				var id = GetControlID( FocusType.Keyboard );
				var name = string.Format( "_tnrdListElement.{0}.{1}", id, index );
				GUI.SetNextControlName( name );

				rect.yMin += 1.5f;
				rect.yMax -= 3.5f;
				switch ( listType ) {
					case EListType.Int:
						DrawInt( rect, index, isActive, isFocused );
						break;
					case EListType.Float:
						DrawFloat( rect, index, isActive, isFocused );
						break;
					case EListType.Double:
						DrawDouble( rect, index, isActive, isFocused );
						break;
					case EListType.String:
						DrawString( rect, index, isActive, isFocused );
						break;
					case EListType.Enum:
						DrawEnum( rect, index, isActive, isFocused );
						break;
				}
				if ( rList.index == index ) {
					EditorGUI.FocusTextInControl( name );
				}
			}
		}

		private void DrawInt( Rect rect, int index, bool isActive, bool isFocused ) {
			var value = (int)rList.list[index];
			if ( rList.index == index ) {
				rList.list[index] = EditorGUI.IntField( rect, value );
			} else {
				EditorGUI.LabelField( rect, value.ToString() );
			}
		}
		private void DrawFloat( Rect rect, int index, bool isActive, bool isFocused ) {
			var value = (float)rList.list[index];
			if ( rList.index == index ) {
				rList.list[index] = EditorGUI.FloatField( rect, value );
			} else {
				EditorGUI.LabelField( rect, value.ToString() );
			}
		}
		private void DrawDouble( Rect rect, int index, bool isActive, bool isFocused ) {
			var value = (double)rList.list[index];
			if ( rList.index == index ) {
				rList.list[index] = EditorGUI.DoubleField( rect, value );
			} else {
				EditorGUI.LabelField( rect, value.ToString() );
			}
		}
		private void DrawString( Rect rect, int index, bool isActive, bool isFocused ) {
			var value = (string)rList.list[index];
			if ( rList.index == index ) {
				rList.list[index] = EditorGUI.TextField( rect, value );
			} else {
				EditorGUI.LabelField( rect, value );
			}
		}
		private void DrawEnum( Rect rect, int index, bool isActive, bool isFocused ) {
			rList.list[index] = EditorGUI.EnumPopup( rect, (Enum)rList.list[index] );
		}
	}
}
#endif