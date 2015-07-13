namespace TNRD {

	[DocsDescription("An object to easily save and query the current and previous value")]
	public class State<T> {

		[DocsDescription("The current value")]
		public T Current;

		[DocsDescription("The previous value")]
		public T Previous;

		[DocsDescription("Updates the values")]
		[DocsParameter("state", "The new value for the current state")]
		public void Update( T state ) {
			Previous = Current;
			Current = state;
		}

		[DocsDescription("Updates the values")]
		public void Update() {
			Previous = Current;
		}

		[DocsIgnore]
		public static bool operator true( State<T> v ) {
			return v.Current.Equals( true );
		}

		[DocsIgnore]
		public static bool operator false( State<T> v ) {
			return v.Current.Equals( false );
		}

		[DocsIgnore]
		public static bool operator !( State<T> v ) {
			return v.Current.Equals( false );
		}

		[DocsDescription("Is the current state equal to true and the previous state equal to false")]
		public bool IsPressed() {
			return Current.Equals( true ) && Previous.Equals( false );
		}

		[DocsDescription("Is the current state equal to false and the previous state equal to true")]
		public bool IsReleased() {
			return Current.Equals( false ) && Previous.Equals( true );
		}

		[DocsDescription("Is the current state equal to true")]
		public bool IsDown() {
			return Current.Equals( true );
		}

		[DocsDescription("Is the current state equal to false")]
		public bool IsUp() {
			return Current.Equals( false );
		}
	}
}