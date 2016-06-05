using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class Logger {

    public static void LogMethod() {
        var stack = new StackTrace();
        if ( stack.FrameCount >= 1 ) {
            var frame = stack.GetFrame( 1 );
            var method = frame.GetMethod();
            UnityEngine.Debug.LogFormat( "[{0}.{1}]", method.DeclaringType.Name, method.Name );
        }
    }
}
