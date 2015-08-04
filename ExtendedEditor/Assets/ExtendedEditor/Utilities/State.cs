namespace TNRD {

    /// <summary>
    /// An object to easily save and query the current and previous value
    /// </summary>
    public class State<T> {

        /// <summary>
        /// The current value
        /// </summary>
        public T Current;

        /// <summary>
        /// The previous value
        /// </summary>
        public T Previous;

        /// <summary>
        /// Updates the values
        /// </summary>
        /// <param name="state">The new value for the current state</param>
        public void Update( T state ) {
            Previous = Current;
            Current = state;
        }

        /// <summary>
        /// Updates the values
        /// </summary>
        public void Update() {
            Previous = Current;
        }

        public static bool operator true( State<T> v ) {
            return v.Current.Equals( true );
        }

        public static bool operator false( State<T> v ) {
            return v.Current.Equals( false );
        }

        public static bool operator !( State<T> v ) {
            return v.Current.Equals( false );
        }

        /// <summary>
        /// Is the current state equal to true and the previous state equal to false
        /// </summary>
        public bool IsPressed() {
            return Current.Equals( true ) && Previous.Equals( false );
        }

        /// <summary>
        /// Is the current state equal to false and the previous state equal to true
        /// </summary>
        public bool IsReleased() {
            return Current.Equals( false ) && Previous.Equals( true );
        }

        /// <summary>
        /// Is the current state equal to true
        /// </summary>
        public bool IsDown() {
            return Current.Equals( true );
        }

        /// <summary>
        /// Is the current state equal to false
        /// </summary>
        public bool IsUp() {
            return Current.Equals( false );
        }
    }
}