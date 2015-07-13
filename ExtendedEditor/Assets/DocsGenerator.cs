using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using TNRD.Editor.Core;
using UnityEditor;
using UnityEngine;
using System.Linq;

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
sealed class DocsIgnoreAttribute : Attribute {
	public DocsIgnoreAttribute() { }
}

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
sealed class DocsDescriptionAttribute : Attribute {
	readonly string description;

	public DocsDescriptionAttribute( string description ) {
		this.description = description;
	}

	public string Description {
		get { return description; }
	}
}

[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
sealed class DocsParameterAttribute : Attribute {
	readonly string parameter;
	readonly string description;

	public DocsParameterAttribute( string parameter, string description ) {
		this.parameter = parameter;
		this.description = description;
	}

	public string Parameter {
		get { return parameter; }
	}

	public string Description {
		get { return description; }
	}
}

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
sealed class DocsReturnValueAttribute : Attribute {
	readonly string description;

	public DocsReturnValueAttribute( string description ) {
		this.description = description;
	}

	public string Description {
		get { return description; }
	}
}

public class DocsGenerator : MonoBehaviour {

	private class FieldInfoComparer : IComparer<FieldInfo> {
		public int Compare( FieldInfo f1, FieldInfo f2 ) {
			return f1.Name.CompareTo( f2.Name );
		}
	}

	private class PropertyInfoComparer : IComparer<PropertyInfo> {
		public int Compare( PropertyInfo p1, PropertyInfo p2 ) {
			return p1.Name.CompareTo( p2.Name );
		}
	}

	private class MethodInfoComparer : IComparer<MethodInfo> {
		public int Compare( MethodInfo m1, MethodInfo m2 ) {
			return m1.Name.CompareTo( m2.Name );
		}
	}

	#region Table Of Contents
	private class TocItem {
		public string link = "null";
		public string title = "";
		public List<TocItem> children = null;

		public override string ToString() {
			return title;
		}
	}

	[MenuItem("TNRD/Create TableOfContents")]
	private static void CreateTOC() {
		var items = new Dictionary<string, List<string>>();
		var tocs = new Dictionary<string, TocItem>();

		var ext = typeof(ExtendedEditor);
		var types = ext.Assembly.GetTypes();
		foreach ( var item in types ) {
			if ( item.Namespace == null ) continue;
			if ( !item.IsPublic ) continue;
			if ( item.Namespace.StartsWith( "TNRD" ) ) {
				if ( item.GetCustomAttributes( typeof(DocsIgnoreAttribute), false ).Length > 0 ) continue;
				if ( item.Namespace != null ) {
					if ( item.ReflectedType == null ) {
						var key = item.Namespace.Replace( "`1", "" );
						if ( !items.ContainsKey( key ) ) {
							items.Add( key, new List<string>() );
						}
						items[key].Add( item.Name.Replace( "`1", "" ) );
					} else {
						var key = string.Format( "{0}.{1}",
							item.Namespace, item.ReflectedType.Name ).Replace( "`1", "" );
						if ( !items.ContainsKey( key ) ) {
							items.Add( key, new List<string>() );
						}
						items[key].Add( item.Name.Replace( "`1", "" ) );
					}
				}
			}
		}

		foreach ( var item in items ) {
			var namespaces = item.Key.Split( '.' );
			var ns = "";
			foreach ( var @namespace in namespaces ) {
				if ( string.IsNullOrEmpty( ns ) ) {
					ns = @namespace;
					if ( !tocs.ContainsKey( ns ) ) {
						tocs.Add( ns, new TocItem() );
					}
				} else {
					if ( !tocs.ContainsKey( ns + "." + @namespace ) ) {
						if ( tocs[ns].children == null ) {
							tocs[ns].children = new List<TocItem>();
						}
						var toc = new TocItem();
						toc.title = @namespace;

						tocs[ns].children.Add( toc );
						tocs[ns + "." + @namespace] = toc;

					}

					ns += "." + @namespace;
				}
			}
		}

		foreach ( var item in items ) {
			foreach ( var value in item.Value ) {
				var key = item.Key;
				var key2 = key + "." + value;
				if ( tocs.ContainsKey( key2 ) ) {
					tocs[key2].link = key2;
				} else {
					if ( tocs[key].children == null ) {
						tocs[key].children = new List<TocItem>();
					}

					tocs[item.Key].children.Add( new TocItem() {
						link = key2,
						title = value
					} );
				}
			}
		}

		var json = TNRD.Json.JsonConvert.SerializeObject( tocs["TNRD"] );
		File.WriteAllText( "toc.jsu", string.Format( "var toc = {0}", json ) );
	}
	#endregion

	#region Item Page
	[MenuItem("TNRD/Create ObjectDocs")]
	private static void CreateObjectDocs() {
		var t = typeof(ExtendedEditor);
		var types = t.Assembly.GetTypes();
		foreach ( var item in types ) {
			if ( item.Namespace == null ) continue;
			if ( !item.IsPublic ) continue;
			if ( item.Namespace.StartsWith( "TNRD" ) ) {
				if ( item.GetCustomAttributes( typeof(DocsIgnoreAttribute), false ).Length > 0 ) continue;

				var builder = new StringBuilder();

				builder.Append( DocumentStart() );

				builder.Append( ItemStart( item ) );
				builder.Append( GenerateItemFields( item ) );
				builder.Append( GenerateItemProperties( item ) );
				builder.Append( GenerateItemConstructors( item ) );
				builder.Append( GenerateItemPublicFunctions( item ) );
				builder.Append( GenerateItemStaticFunctions( item ) );

				builder.Append( DocumentEnd() );

				File.WriteAllText( string.Format( "{0}.{2}{1}.html",
					item.Namespace,
					item.Name,
					item.ReflectedType == null ? "" : item.ReflectedType.Name + "." ).Replace( "`1", "" ), builder.ToString().Replace( "`1", "" ) );
			}
		}
	}

	private static string ItemStart( Type item ) {
		var attr = item.GetCustomAttributes( typeof(DocsDescriptionAttribute), false );


		return string.Format( @"<div class=""section"">
	<div class=""mb20 clear"" id="""">
		<h1 class=""heading inherit"">{0}</h1>
		<div class=""clear""></div>
		<p class=""cl mb0 left mr10"">class in {1}</p>
        <div class=""clear""></div>
	</div>
	<div class=""subsection"">
		<h2>Description</h2>
		<p>{2}</p>
	</div>", item.Name, item.Namespace, GetDescription( item ) );
	}

	private static string GenerateItemFields( Type item ) {
		var fields = item.GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly ).ToList();
		fields.Sort( new FieldInfoComparer() );

		for ( int i = fields.Count - 1; i >= 0; i-- ) {
			if ( i < fields.Count - 1 ) {
				if ( fields[i].Name == fields[i + 1].Name ) {
					fields.RemoveAt( i + 1 );
				}
			}

			if ( fields[i].GetCustomAttributes( typeof(DocsIgnoreAttribute), false ).Length > 0 ) {
				fields.RemoveAt( i );
			}
		}
		var builder = new StringBuilder();

		builder.Append( SubsectionStart( "Fields" ) );
		foreach ( var field in fields ) {
			if ( field.IsPrivate ) continue;
			//builder.Append( SubsectionSegment( string.Format( "{2}.{0}-{1}.html", item.Name, variable.Name, item.Namespace ), variable.Name ) );
			builder.Append( SubsectionSegment( "#", field.Name, GetDescription( field ) ) );
		}

		if ( fields.Count == 0 ) {
			builder.Append( SubsectionSegment( "#", "No fields visible", "-" ) );
		}

		builder.Append( SubsectionEnd() );

		return builder.ToString();
	}

	private static string GenerateItemProperties( Type item ) {
		var properties = item.GetProperties( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly ).ToList();
		properties.Sort( new PropertyInfoComparer() );
		for ( int i = properties.Count - 1; i >= 0; i-- ) {
			if ( properties[i].GetCustomAttributes( typeof(DocsIgnoreAttribute), false ).Length > 0 ) {
				properties.RemoveAt( i );
			}
		}

		var builder = new StringBuilder();

		builder.Append( SubsectionStart( "Properties" ) );
		foreach ( var property in properties ) {
			if ( !property.CanRead && !property.CanWrite ) continue;

			//builder.Append( SubsectionSegment( string.Format( "{2}.{0}-{1}.html", item.Name, property.Name, item.Namespace ), property.Name ) );
			builder.Append( SubsectionSegment( "#", property.Name, GetDescription( property ) ) );
		}

		if ( properties.Count == 0 ) {
			builder.Append( SubsectionSegment( "#", "No properties visible", "-" ) );
		}

		builder.Append( SubsectionEnd() );

		return builder.ToString();
	}

	private static string GenerateItemConstructors( Type item ) {
		var constructors = item.GetConstructors( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly ).ToList();
		for ( int i = constructors.Count - 1; i >= 0; i-- ) {
			if ( constructors[i].GetCustomAttributes( typeof(DocsIgnoreAttribute), false ).Length > 0 ) {
				constructors.RemoveAt( i );
			}
		}

		var builder = new StringBuilder();

		builder.Append( SubsectionStart( "Constructors" ) );
		for ( int i = 0; i < constructors.Count; i++ ) {
			if ( constructors[i].IsPrivate ) continue;
			builder.Append( SubsectionSegment(
				string.Format( "{2}.{3}{0}-{0}.html",
				item.Name,
				constructors[i].Name,
				item.Namespace,
				item.ReflectedType == null ? "" : item.ReflectedType.Name + "." ), item.Name, GetDescription( constructors[i] ) ) );
			// breaking after the first, we only want one
			break;
		}

		if ( constructors.Count == 0 ) {
			builder.Append( SubsectionSegment( "#", "No constructors visible", "-" ) );
		}

		builder.Append( SubsectionEnd() );

		return builder.ToString();
	}

	private static string GenerateItemPublicFunctions( Type item ) {
		var methods = item.GetMethods( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly ).ToList();
		methods.Sort( new MethodInfoComparer() );

		for ( int i = methods.Count - 1; i >= 0; i-- ) {
			if ( i < methods.Count - 1 ) {
				if ( methods[i].Name == methods[i + 1].Name ) {
					methods.RemoveAt( i + 1 );
				}
			}

			if ( methods[i].GetCustomAttributes( typeof(DocsIgnoreAttribute), false ).Length > 0 ) {
				methods.RemoveAt( i );
			}
		}

		var builder = new StringBuilder();

		builder.Append( SubsectionStart( "Public Methods" ) );
		foreach ( var method in methods ) {
			if ( method.IsPrivate ) continue;
			if ( method.Name.StartsWith( "get_" ) || method.Name.StartsWith( "set_" ) ) continue;

			builder.Append( SubsectionSegment(
				string.Format( "{2}.{3}{0}-{1}.html",
				item.Name,
				method.Name,
				item.Namespace,
				item.ReflectedType == null ? "" : item.ReflectedType.Name + "." ), method.Name, GetDescription( method ) ) );
		}

		if ( methods.Count == 0 ) {
			builder.Append( SubsectionSegment( "#", "No methods visible", "-" ) );
		}

		builder.Append( SubsectionEnd() );

		return builder.ToString();
	}

	private static string GenerateItemStaticFunctions( Type item ) {
		var methods = item.GetMethods( BindingFlags.Static | BindingFlags.Public ).ToList();
		methods.Sort( new MethodInfoComparer() );

		for ( int i = methods.Count - 1; i >= 0; i-- ) {
			if ( i < methods.Count - 1 ) {
				if ( methods[i].Name == methods[i + 1].Name ) {
					methods.RemoveAt( i + 1 );
				}
			}

			if ( methods[i].GetCustomAttributes( typeof(DocsIgnoreAttribute), false ).Length > 0 ) {
				methods.RemoveAt( i );
			}
		}
		var builder = new StringBuilder();

		builder.Append( SubsectionStart( "Static Methods" ) );
		foreach ( var method in methods ) {
			builder.Append( SubsectionSegment(
				string.Format( "{2}.{3}{0}-{1}.html",
				item.Name,
				method.Name,
				item.Namespace,
				item.ReflectedType == null ? "" : item.ReflectedType.Name + "." ), method.Name, GetDescription( method ) ) );
		}

		if ( methods.Count == 0 ) {
			builder.Append( SubsectionSegment( "#", "No static methods visible", "-" ) );
		}

		builder.Append( SubsectionEnd() );

		return builder.ToString();
	}

	private static string SubsectionStart( string name ) {
		return string.Format( @"<div clas=""subsection"">
	<h2>{0}</h2>
	<table class=""list"">
		<tbody>", name );
	}

	private static string SubsectionSegment( string link, string name, string description ) {
		return string.Format( @"<tr>
	<td class=""lbl"">
		<a href=""{0}"">{1}</a>
	</td>
	<td class=""desc"">{2}</td>
</tr>", link, name, string.IsNullOrEmpty( description ) ? "..." : description );
	}

	private static string SubsectionEnd() {
		return @"		</tbody>
	</table>
</div>";
	}
	#endregion

	#region Member Pages
	[MenuItem("TNRD/Create MemberDocs")]
	private static void CreateMemberDocs() {
		var t = typeof(ExtendedEditor);
		var types = t.Assembly.GetTypes();
		foreach ( var item in types ) {
			if ( item.Namespace == null ) continue;
			if ( item.GetCustomAttributes( typeof(DocsIgnoreAttribute), false ).Length > 0 ) continue;

			if ( item.Namespace.StartsWith( "TNRD" ) ) {

				// Not doing pages for fields or properties
				//GenerateMemberFields( item );
				//GenerateMemberProperties( item );
				GenerateMemberMethods( item );
				GenerateMemberConstructors( item );
			}
		}
	}

	private static void GenerateMemberFields( Type parent ) {
		var fields = parent.GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly );

		foreach ( var field in fields ) {
			if ( field.IsPrivate ) continue;
			if ( field.GetCustomAttributes( typeof(DocsIgnoreAttribute), false ).Length > 0 ) continue;

			var builder = new StringBuilder();

			builder.Append( DocumentStart() );
			builder.AppendFormat(
@"<div class=""section"">
<div class=""mb20 clear"" id="""">
	<h1 class=""heading inherit"">
		<a href=""{0}{2}.html"">{2}</a>.{1}</h1>
	<div class=""clear""></div>
    <div class=""clear""></div>
    <div class=""clear""></div>
</div>
<div class=""subsection"">
	<div class=""signature"">", parent.Namespace == null ? "" : parent.Namespace + ".", field.Name, parent.Name );

			builder.AppendFormat(
@"<div class=""signature-CS sig-block"" style=""display: block;"">
	{0} {1} <span class=""sig-kw"">{2}</span>;
</div>", field.IsFamily ? "protected" : "public", GetType( field.FieldType ), field.Name );

			builder.Append(
@"	</div>
</div>" );

			builder.Append( @"<div class=""subsection"">
	<h2>Description</h2>
	<p>...</p>
</div>" );

			builder.Append( DocumentEnd() );

			File.WriteAllText( string.Format( "{2}.{0}-{1}.html", parent.Name, field.Name, parent.Namespace ).Replace( "`1", "" ), builder.ToString() );
		}
	}

	private static void GenerateMemberProperties( Type parent ) {
		var properties = parent.GetProperties( BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly );

		foreach ( var property in properties ) {
			if ( !property.CanWrite && !property.CanRead ) continue;

			var getm = property.GetSetMethod();
			var setm = property.GetGetMethod();

			if ( !( getm == null || getm.IsFamily || getm.IsPublic ) && ( setm == null || !setm.IsFamily || setm.IsPublic ) ) continue;
			if ( property.GetCustomAttributes( typeof(DocsIgnoreAttribute), false ).Length > 0 ) continue;

			var builder = new StringBuilder();

			builder.Append( DocumentStart() );
			builder.AppendFormat(
@"<div class=""section"">
<div class=""mb20 clear"" id="""">
	<h1 class=""heading inherit"">
		<a href=""{0}{2}.html"">{2}</a>.{1}</h1>
	<div class=""clear""></div>
    <div class=""clear""></div>
    <div class=""clear""></div>
</div>
<div class=""subsection"">
	<div class=""signature"">", parent.Namespace == null ? "" : parent.Namespace + ".", property.Name, parent.Name );

			var accessors = "{ ";
			if ( property.CanRead && getm != null ) {

				if ( getm.IsFamily ) {
					accessors += " protected";
				} else if ( getm.IsPrivate ) {
					accessors += " private";
				}

				accessors += "get;";
			}
			if ( property.CanWrite && setm != null ) {
				if ( setm.IsFamily ) {
					accessors += " protected ";
				} else if ( setm.IsPrivate ) {
					accessors += " private ";
				}

				accessors += "set;";
			}
			accessors += " }";

			builder.AppendFormat(
@"<div class=""signature-CS sig-block"" style=""display: block;"">
				public {0} <span class=""sig-kw"">{1}</span> {2}
			</div>", GetType( property.PropertyType ), property.Name, accessors );

			builder.Append(
@"	</div>
</div>" );

			builder.Append( @"<div class=""subsection"">
				<h2>Description</h2>
				<p>...</p>
			</div>" );

			builder.Append( DocumentEnd() );

			File.WriteAllText( string.Format( "{2}.{0}-{1}.html", parent.Name, property.Name, parent.Namespace ).Replace( "`1", "" ), builder.ToString() );
		}
	}

	private static void GenerateMemberMethods( Type parent ) {
		var typeMethods = parent.GetMethods( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly );
		var methodInfos = new Dictionary<string, List<MethodInfo>>();

		foreach ( var item in typeMethods ) {
			if ( item.IsPrivate ) continue;
			if ( item.Name.StartsWith( "get_" ) || item.Name.StartsWith( "set_" ) ) continue;
			if ( item.GetCustomAttributes( typeof(DocsIgnoreAttribute), false ).Length > 0 ) continue;

			if ( methodInfos.ContainsKey( item.Name ) ) {
				methodInfos[item.Name].Add( item );
			} else {
				methodInfos.Add( item.Name, new List<MethodInfo>() );
				methodInfos[item.Name].Add( item );
			}
		}

		foreach ( var item in methodInfos ) {
			var value = item.Value[0];
			var path = string.Format( "{0}-{1}.html",
				parent.ToString(), value.Name )
				.Replace( "+", "." )
				.Replace( "[T]", "" )
				.Replace( "`1", "" );

			if ( path.Contains( "+" ) ) {
				Debug.Log( value.Name + "contains+" );
			}

			var builder = new StringBuilder();

			builder.Append( DocumentStart() );

			builder.AppendFormat(
@"<div class=""section"">
	<div class=""mb20 clear"" id="""">
		<h1 class=""heading inherit"">
			<a href=""{0}"">{1}</a>.{2}</h1>
		<div class=""clear""></div>
		<div class=""clear""></div>
		<div class=""clear""></div>
	</div>
	<div class=""subsection"">
		<div class=""signature"">", path.Replace( "-", "" ).Replace( value.Name, "" ), parent.Name, value.Name );

			var parameterNames = new List<string>();

			foreach ( var method in item.Value ) {
				var parameterInfos = method.GetParameters();
				string parameters = "";
				foreach ( var parameter in parameterInfos ) {
					parameters += string.Format( " {0} <span class=\"sig-kw\">{1}</span>,", GetType( parameter.ParameterType ), parameter.Name );

					if ( !parameterNames.Contains( parameter.Name ) ) {
						parameterNames.Add( parameter.Name );
					}
				}
				parameters = parameters.TrimEnd( ',' );
				parameters += " ";
				parameters = parameters.Trim();

				builder.AppendFormat(
@"<div class=""signature-CS sig-block"">
	{0}{4} {1} <span class=""sig-kw"">{2}</span>({3});
</div>", method.IsFamily ? "protected" : "public", GetType( method.ReturnType ), method.Name, parameters, method.IsStatic ? " static" : "" );
			}

			builder.Append(
@"	</div>
</div>" );

			builder.Append( @"<div class=""subsection"">
	<h2>Parameters</h2>
	<table class=""list"">
		<tbody>" );
			foreach ( var parameter in parameterNames ) {
				builder.AppendFormat( @"<tr>
	<td class=""name lbl"">{0}</td>
	<td class=""desc"">...</td>", parameter );
			}

			builder.Append( @" </tbody>
	</table>
</div>" );

			builder.AppendFormat( @"<div class=""subsection"">
	<h2>Description</h2>
	<p>{0}</p>
</div>", GetDescription( value ) );

			builder.Append( DocumentEnd() );

			File.WriteAllText( path, builder.ToString() );
		}
	}

	private static void GenerateMemberConstructors( Type parent ) {
		var typeMethods = parent.GetConstructors( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly );
		var methodInfos = new Dictionary<string, List<ConstructorInfo>>();

		foreach ( var item in typeMethods ) {
			if ( item.IsPrivate ) continue;
			if ( item.Name.StartsWith( "get_" ) || item.Name.StartsWith( "set_" ) ) continue;

			if ( methodInfos.ContainsKey( item.Name ) ) {
				methodInfos[item.Name].Add( item );
			} else {
				methodInfos.Add( item.Name, new List<ConstructorInfo>() );
				methodInfos[item.Name].Add( item );
			}
		}

		foreach ( var item in methodInfos ) {
			var name = parent.Name.Replace( "[T]", "" ).Replace( "`1", "" );
			var value = item.Value[0];
			var path = string.Format( "{0}-{1}.html",
				parent.ToString(), parent.Name )
				.Replace( "+", "." )
				.Replace( "[T]", "" )
				.Replace( "`1", "" );

			var builder = new StringBuilder();

			builder.Append( DocumentStart() );

			builder.AppendFormat(
@"<div class=""section"">
	<div class=""mb20 clear"" id="""">
		<h1 class=""heading inherit"">
			<a href=""{0}"">{1}</a>.{2}</h1>
		<div class=""clear""></div>
		<div class=""clear""></div>
		<div class=""clear""></div>
	</div>
	<div class=""subsection"">
		<div class=""signature"">", path.Remove( path.LastIndexOf( name ), name.Length ).Replace( "-", "" ), parent.Name, parent.Name );

			var parameterNames = new List<string>();

			foreach ( var method in item.Value ) {
				var parameterInfos = method.GetParameters();
				string parameters = "";
				foreach ( var parameter in parameterInfos ) {
					parameters += string.Format( " {0} <span class=\"sig-kw\">{1}</span>,", GetType( parameter.ParameterType ), parameter.Name );

					if ( !parameterNames.Contains( parameter.Name ) ) {
						parameterNames.Add( parameter.Name );
					}
				}
				parameters = parameters.TrimEnd( ',' );
				parameters += " ";
				parameters = parameters.Trim();

				builder.AppendFormat(
@"<div class=""signature-CS sig-block"">
	{0} <span class=""sig-kw"">{1}</span>({2});
</div>", method.IsFamily ? "protected" : "public", parent.Name, parameters );
			}

			builder.Append(
@"	</div>
</div>" );

			builder.Append( @"<div class=""subsection"">
	<h2>Parameters</h2>
	<table class=""list"">
		<tbody>" );
			foreach ( var parameter in parameterNames ) {
				builder.AppendFormat( @"<tr>
	<td class=""name lbl"">{0}</td>
	<td class=""desc"">...</td>", parameter );
			}

			builder.Append( @" </tbody>
	</table>
</div>" );

			builder.Append( @"<div class=""subsection"">
	<h2>Description</h2>
	<p>...</p>
</div>" );

			builder.Append( DocumentEnd() );

			File.WriteAllText( path, builder.ToString() );
		}
	}

	//	private static string GenerateMember( Type parent, FieldInfo item ) {
	//		return string.Format(
	//@" < div class=""section"">
	//	<div class=""mb20 clear"" id="""">
	//		<h1 class=""heading inherit"">
	//			<a href=""{0}.html"">{0}</a>.{1}</h1>
	//		<div class=""clear""></div>
	//        <div class=""clear""></div>
	//        <div class=""clear""></div>
	//	</div>
	//	<div class=""subsection"">
	//		<div class=""signature"">
	//			<div class=""signature-CS sig-block"" style=""display: block;"">
	//			{2} {3} <span class=""sig-kw"">{1}</span>;
	//			</div>
	//		</div>
	//	</div>", parent.Name, item.Name, item.IsFamily ? "protected" : "public", item.FieldType.Name );
	//	}

	//	private static string GenerateMember( Type parent, PropertyInfo item ) {
	//		return string.Format(
	//@"<div class=""section"">
	//	<div class=""mb20 clear"" id="""">
	//		<h1 class=""heading inherit"">
	//			<a href=""{0}.html"">{0}</a>.{1}</h1>
	//		<div class=""clear""></div>
	//        <div class=""clear""></div>
	//        <div class=""clear""></div>
	//	</div>
	//	<div class=""subsection"">
	//		<div class=""signature"">
	//			<div class=""signature-CS sig-block"" style=""display: block;"">
	//			{2} {3} <span class=""sig-kw"">{1}</span> { {4} {5} };
	//			</div>
	//		</div>
	//	</div>", parent.Name, item.Name, "public/protected", item.PropertyType.Name,
	//	"get;", "set;" );
	//	}

	//	private static string GenerateMember( Type parent, ConstructorInfo item ) {
	//		return string.Format(
	//@"<div class=""section"">
	//	<div class=""mb20 clear"" id="""">
	//		<h1 class=""heading inherit"">
	//			<a href=""{0}.html"">{0}</a>.{1}</h1>
	//		<div class=""clear""></div>
	//        <div class=""clear""></div>
	//        <div class=""clear""></div>
	//	</div>
	//	<div class=""subsection"">
	//		<div class=""signature"">
	//			<div class=""signature-CS sig-block"" style=""display: block;"">
	//			{2} {3} <span class=""sig-kw"">{1}</span>({4});
	//			</div>
	//		</div>
	//	</div>", parent.Name, item.Name, item.IsFamily ? "protected" : "public", "parameters" );
	//	}

	//	private static string GenerateMember( Type parent, MethodInfo item ) {
	//		return string.Format(
	//@"<div class=""section"">
	//	<div class=""mb20 clear"" id="""">
	//		<h1 class=""heading inherit"">
	//			<a href=""{0}.html"">{0}</a>.{1}</h1>
	//		<div class=""clear""></div>
	//        <div class=""clear""></div>
	//        <div class=""clear""></div>
	//	</div>
	//	<div class=""subsection"">
	//		<div class=""signature"">
	//			<div class=""signature-CS sig-block"" style=""display: block;"">
	//			{2} {3} <span class=""sig-kw"">{1}</span>({4});
	//			</div>
	//		</div>
	//	</div>", parent.Name, item.Name, item.IsFamily ? "protected" : "public", "parameters" );
	//	}

	//private static string GenerateParameters() {
	//	return "";
	//}
	#endregion

	private static string GetType( Type item ) {
		Type[] args;
		switch ( item.Name ) {
			case "Void":
				return "void";
			case "Int32":
				return "int";
			case "Boolean":
				return "bool";
			case "Single":
				return "float";
			case "String":
				return "string";
			case "Dictionary`2":
				args = item.GetGenericArguments();
				return string.Format( "Dictionary&lt;{0}, {1}&gt;", GetType( args[0] ), GetType( args[1] ) );
			case "List`1":
				args = item.GetGenericArguments();
				return string.Format( "List&lt;{0}&gt;", GetType( args[0] ) );
			default:
				return item.Name;
		}
	}

	private static string DocumentStart() {
		return @"<!DOCTYPE html>
<html lang=""en"" class=""no-js"">
    <head>
        <title>Extended Editor Documentation</title>
        <meta name=""msapplication-TileColor"" content=""#222c37"" />
        <script type=""text/javascript"" src=""js/jquery.jsu""></script>
        <script type=""text/javascript"" src=""js/toc.jsu""></script>
        <script type=""text/javascript"" src=""js/core.jsu""></script>
        <link href=""http://fonts.googleapis.com/css?family=Open+Sans:400,700,400italic"" rel=""stylesheet"" type=""text/css"" />
        <link rel=""stylesheet"" type=""text/css"" href=""css/core.cssu"" />
    </head>
    <body>
        <div class=""header-wrapper"">
            <div id=""header"" class=""header"">
                <div class=""content"">
                    <div class=""more"">
                    </div>
                </div>
            </div>
            <div class=""toolbar"">
                <div class=""content clear""></div>
            </div>
        </div>
        <div id=""master-wrapper"" class=""master-wrapper clear"">
            <div id=""sidebar"" class=""sidebar"">
                <div class=""sidebar-wrap"">
                    <div class=""content"">
                        <div class=""sidebar-menu"">
                            <div class=""toc"">
                                <h2>Scripting API</h2>
                            </div>
                        </div>
                        <p>
                            <br/>
                        </p>
                    </div>
                </div>
            </div>
            <div id=""content-wrap"" class=""content-wrap"">
                <div class=""content-block"">
					<div class=""content"">";
	}

	private static string DocumentEnd() {
		return @"                </div>
                <div class=""footer-wrapper"">
                    <div class=""footer clear""></div>
                </div>
            </div>
			</div>
        </div>
    </div>
</div>
</body>
</html>";
	}

	private static string GetDescription( MemberInfo item ) {
		var attr = item.GetCustomAttributes( typeof(DocsDescriptionAttribute), false );
		if ( attr.Length > 0 ) {
			return ( attr[0] as DocsDescriptionAttribute ).Description;
		} else {
			return "";
		}
	}

	private static string GetDescription( ParameterInfo item ) {
		var attr = item.GetCustomAttributes( typeof(DocsDescriptionAttribute), false );
		if ( attr.Length > 0 ) {
			return ( attr[0] as DocsDescriptionAttribute ).Description;
		} else {
			return "";
		}
	}
}
