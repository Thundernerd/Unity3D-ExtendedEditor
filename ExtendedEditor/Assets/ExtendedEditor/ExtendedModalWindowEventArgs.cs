#if UNITY_EDITOR
using System;

namespace TNRD.Editor.Core {

    /// <summary>
    /// The result arguments when a modal window gets closed
    /// </summary>
    public class ExtendedModalWindowEventArgs : EventArgs {

        /// <summary>
        /// The window that got closed
        /// </summary>
        public ExtendedModalWindow Window { get; private set; }

        /// <summary>
        /// The result of the window
        /// </summary>
        public EExtendedModalWindowResult Result { get; private set; }

        /// <summary>
        /// Creates a new instance of ExtendedModalWindowEventArgs
        /// </summary>
        public ExtendedModalWindowEventArgs() : base() { }

        /// <summary>
        /// Creates a new instance of ExtendedModalWindowEventArgs
        /// </summary>
        /// <param name="window">The modal window</param>
        /// <param name="result">The result of the modal window</param>
        public ExtendedModalWindowEventArgs( ExtendedModalWindow window, EExtendedModalWindowResult result ) : this() {
            Window = window;
            Result = result;
        }
    }
}
#endif