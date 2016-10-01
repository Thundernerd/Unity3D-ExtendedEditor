#if UNITY_EDITOR
﻿using System;
using System.Linq;
using System.Reflection;

namespace TNRD.Editor.Serialization {

    public class SerializationHelper {

        public static PropertyInfo GetProperty( Type type, string name, BindingFlags flags ) {
            if ( type == null ) return null;

            var prop = type.GetProperty( name, flags );
            if ( prop != null ) return prop;

            return GetProperty( type.BaseType, name, flags );
        }

        public static FieldInfo GetField( Type type, string name, BindingFlags flags ) {
            if ( type == null ) return null;

            var field = type.GetField( name, flags );
            if ( field != null ) return field;

            return GetField( type.BaseType, name, flags );
        }

        public static MemberInfo GetMember( Type type, string name, BindingFlags flags ) {
            if ( type == null ) return null;

            var member = type.GetMember( name, flags );
            if ( member != null && member.Length >= 1 ) return member[0];

            return GetMember( type.BaseType, name, flags );
        }

        public static FieldInfo[] GetFields( Type type, BindingFlags flags ) {
            if ( type == null ) return new FieldInfo[0];

            return type.GetFields( flags )
                .Concat( GetFields( type.BaseType, flags ) )
                .GroupBy( f => f.ToString() )
                .Select( g => g.First() )
                .ToArray();
        }

        public static PropertyInfo[] GetProperties( Type type, BindingFlags flags ) {
            if ( type == null ) return new PropertyInfo[0];

            return type.GetProperties( flags )
                .Concat( GetProperties( type.BaseType, flags ) )
                .GroupBy( p => p.ToString() )
                .Select( g => g.First() )
                .ToArray();
        }
    }
}
#endif