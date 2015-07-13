using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using TNRD.Editor.Core;
using UnityEditor;
using UnityEngine;

public class DocsGenerator : MonoBehaviour {


	//[MenuItem("TNRD/Create ObjectDocs")]
	//private static void CreateDocs() {
	//	var t = typeof(ExtendedEditor);
	//	var types = t.Assembly.GetTypes();
	//	foreach ( var item in types ) {
	//		if ( item.Namespace == null ) continue;
	//		if ( item.Namespace.StartsWith( "TNRD" ) ) {

	//			var builder = new StringBuilder();

	//			builder.Append( DocumentStart() );

	//			builder.Append( ItemStart( item ) );
	//			builder.Append( GenerateItemFields( item ) );
	//			builder.Append( GenerateItemProperties( item ) );
	//			builder.Append( GenerateItemConstructors( item ) );
	//			builder.Append( GenerateItemPublicFunctions( item ) );
	//			builder.Append( GenerateItemStaticFunctions( item ) );

	//			builder.Append( DocumentEnd() );


	//			//Debug.Log( builder.ToString() );
	//			File.WriteAllText( string.Format( "{0}.{1}.html", item.Namespace, item.Name ).Replace( "`1", "" ), builder.ToString() );
	//			//var s = "{";
	//			//s += string.Format( "\n\t \"link\":\"{1}\", \n\t \"title\":\"{0}\", \n\t \"children\":[] \n", item.Name, item.Namespace );
	//			//s += "},";
	//			//Debug.Log( s );
	//		}
	//	}
	//}

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
			if ( item.Namespace.StartsWith( "TNRD" ) ) {
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
		File.WriteAllText( "toc.jsu", json );
	}

	#region Item
	[MenuItem("TNRD/Create ObjectDocs")]
	private static void CreateObjectDocs() {
		var t = typeof(ExtendedEditor);
		var types = t.Assembly.GetTypes();
		foreach ( var item in types ) {
			if ( item.Namespace == null ) continue;
			if ( item.Namespace.StartsWith( "TNRD" ) ) {

				var builder = new StringBuilder();

				builder.Append( DocumentStart() );

				builder.Append( ItemStart( item ) );
				builder.Append( GenerateItemFields( item ) );
				builder.Append( GenerateItemProperties( item ) );
				builder.Append( GenerateItemConstructors( item ) );
				builder.Append( GenerateItemPublicFunctions( item ) );
				builder.Append( GenerateItemStaticFunctions( item ) );

				builder.Append( DocumentEnd() );

				File.WriteAllText( string.Format( "{0}.{1}.html", item.Namespace, item.Name ).Replace( "`1", "" ), builder.ToString() );
			}
		}
	}

	private static string ItemStart( Type item ) {
		return string.Format( @"<div class=""section"">
	<div class=""mb20 clear"" id="""">
		<h1 class=""heading inherit"">{0}</h1>
		<div class=""clear""></div>
		<p class=""cl mb0 left mr10"">class in {1} </p>
        <div class=""clear""></div>
	</div>
	<div class=""subsection"">
		<h2>Description</h2>
		<p></p>
	</div>", item.Name, item.Namespace );
	}

	private static string GenerateItemFields( Type item ) {
		var variables = item.GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly );
		if ( variables.Length > 0 ) {
			var builder = new StringBuilder();

			builder.Append( SubsectionStart( "Variables" ) );
			foreach ( var variable in variables ) {
				if ( variable.IsPrivate ) continue;
				builder.Append( SubsectionSegment( string.Format( "{0}-{1}.html", item.Name, variable.Name ), variable.Name ) );
			}
			builder.Append( SubsectionEnd() );

			return builder.ToString();
		}

		return "";
	}

	private static string GenerateItemProperties( Type item ) {
		var properties = item.GetProperties( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly );
		if ( properties.Length > 0 ) {
			var builder = new StringBuilder();

			builder.Append( SubsectionStart( "Properties" ) );
			foreach ( var property in properties ) {
				if ( !property.CanRead && !property.CanWrite ) continue;
				builder.Append( SubsectionSegment( "#", property.Name ) );
			}
			builder.Append( SubsectionEnd() );

			return builder.ToString();
		}

		return "";
	}

	private static string GenerateItemConstructors( Type item ) {
		var constructors = item.GetConstructors( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly );
		if ( constructors.Length > 0 ) {
			var builder = new StringBuilder();

			builder.Append( SubsectionStart( "Constructors" ) );
			for ( int i = 0; i < constructors.Length; i++ ) {
				if ( constructors[i].IsPrivate ) continue;
				builder.Append( SubsectionSegment( "#", item.Name ) );
			}
			builder.Append( SubsectionEnd() );

			return builder.ToString();
		}

		return "";
	}

	private static string GenerateItemPublicFunctions( Type item ) {
		var pubFunctions = item.GetMethods( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly );
		if ( pubFunctions.Length > 0 ) {
			var builder = new StringBuilder();

			builder.Append( SubsectionStart( "Public Functions" ) );
			foreach ( var function in pubFunctions ) {
				if ( function.IsPrivate ) continue;
				if ( function.Name.StartsWith( "get_" ) || function.Name.StartsWith( "set_" ) ) continue;
				builder.Append( SubsectionSegment( string.Format( "{0}-{1}.html", item.Name, function.Name ), function.Name ) );
			}
			builder.Append( SubsectionEnd() );

			return builder.ToString();
		}

		return "";
	}

	private static string GenerateItemStaticFunctions( Type item ) {
		var statFunctions = item.GetMethods( BindingFlags.Static | BindingFlags.Public );
		if ( statFunctions.Length > 0 ) {
			var builder = new StringBuilder();

			builder.Append( SubsectionStart( "Static Functions" ) );
			foreach ( var function in statFunctions ) {
				builder.Append( SubsectionSegment( "#", function.Name ) );
			}
			builder.Append( SubsectionEnd() );


			return builder.ToString();
		}

		return "";
	}

	private static string SubsectionStart( string name ) {
		return string.Format( @"<div clas=""subsection"">
	<h2>{0}</h2>
	<table class=""list"">
		<tbody>", name );
	}

	private static string SubsectionSegment( string link, string name ) {
		return string.Format( @"<tr>
	<td class=""lbl"">
		<a href=""{0}"">{1}</a>
	</td>
	<td class=""desc"">...</td>
</tr>", link, name );
	}

	private static string SubsectionEnd() {
		return @"		</tbody>
	</table>
</div>";
	}
	#endregion

	[MenuItem("TNRD/Create MemberDocs")]
	private static void CreateMemberDocs() {
		var t = typeof(ExtendedEditor);
		var types = t.Assembly.GetTypes();
		foreach ( var item in types ) {
			if ( item.Namespace == null ) continue;
			if ( item.Namespace.StartsWith( "TNRD" ) ) {

				//GenerateMemberFields( item );
				GenerateMemberMethods( item );
			}
		}
	}

	private static string GetType( string type ) {
		switch ( type ) {
			case "Void":
				return "void";
			case "Int32":
				return "int";
			case "Boolean":
				return "bool";
			default:
				return type;
		}
	}

	private static void GenerateMemberFields( Type parent ) {
		var fields = parent.GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly );

		foreach ( var field in fields ) {
			if ( field.IsPrivate ) continue;
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
@"<div class==""signature-CS sig-block"" style=""display: block;"">
	{0} {1} <span class=""sig-kw"">{2}</span>;
</div>", field.IsFamily ? "protected" : "public", GetType( field.FieldType.Name ), field.Name );

			builder.Append( @"<div class=""subsection"">
	<h2>Description</h2>
	<p>...</p>
</div>" );

			builder.Append( DocumentEnd() );

			File.WriteAllText( string.Format( "{0}-{1}.html", parent.Name, field.Name ).Replace( "`1", "" ), builder.ToString() );
		}

	}

	private static void GenerateMemberMethods( Type parent ) {
		var typeMethods = parent.GetMethods( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly );
		var methodInfos = new Dictionary<string, List<MethodInfo>>();

		foreach ( var item in typeMethods ) {
			if ( item.IsPrivate ) continue;
			if ( item.Name.StartsWith( "get_" ) || item.Name.StartsWith( "set_" ) ) continue;

			if ( methodInfos.ContainsKey( item.Name ) ) {
				methodInfos[item.Name].Add( item );
			} else {
				methodInfos.Add( item.Name, new List<MethodInfo>() );
				methodInfos[item.Name].Add( item );
			}
		}

		foreach ( var item in methodInfos ) {
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
	<div class=""signature"">", parent.Namespace == null ? "" : parent.Namespace + ".", item.Value[0].Name, parent.Name );

			var parameterNames = new List<string>();

			foreach ( var method in item.Value ) {
				var parameterInfos = method.GetParameters();
				string parameters = "";
				foreach ( var parameter in parameterInfos ) {
					parameters += string.Format( " {0} <span class=\"sig-kw\">{1}</span>,", GetType( parameter.ParameterType.Name ), parameter.Name );

					if ( !parameterNames.Contains( parameter.Name ) ) {
						parameterNames.Add( parameter.Name );
					}
				}
				parameters = parameters.TrimEnd( ',' );
				parameters += " ";

				builder.AppendFormat(
@"<div class==""signature-CS sig-block"" style=""display: block;"">
	{0} {1} <span class=""sig-kw"">{2}</span>({3});
</div>", method.IsFamily ? "protected" : "public", GetType( method.ReturnType.Name ), method.Name, parameters );
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

			File.WriteAllText( string.Format( "{0}-{1}.html", parent.Name, item.Value[0].Name ).Replace( "`1", "" ), builder.ToString() );
		}
	}

	private static string GenerateMember( Type parent, FieldInfo item ) {
		return string.Format(
@" < div class=""section"">
	<div class=""mb20 clear"" id="""">
		<h1 class=""heading inherit"">
			<a href=""{0}.html"">{0}</a>.{1}</h1>
		<div class=""clear""></div>
        <div class=""clear""></div>
        <div class=""clear""></div>
	</div>
	<div class=""subsection"">
		<div class=""signature"">
			<div class=""signature-CS sig-block"" style=""display: block;"">
			{2} {3} <span class=""sig-kw"">{1}</span>;
			</div>
		</div>
	</div>", parent.Name, item.Name, item.IsFamily ? "protected" : "public", item.FieldType.Name );
	}

	private static string GenerateMember( Type parent, PropertyInfo item ) {
		return string.Format(
@"<div class=""section"">
	<div class=""mb20 clear"" id="""">
		<h1 class=""heading inherit"">
			<a href=""{0}.html"">{0}</a>.{1}</h1>
		<div class=""clear""></div>
        <div class=""clear""></div>
        <div class=""clear""></div>
	</div>
	<div class=""subsection"">
		<div class=""signature"">
			<div class=""signature-CS sig-block"" style=""display: block;"">
			{2} {3} <span class=""sig-kw"">{1}</span> { {4} {5} };
			</div>
		</div>
	</div>", parent.Name, item.Name, "public/protected", item.PropertyType.Name,
	"get;", "set;" );
	}

	private static string GenerateMember( Type parent, ConstructorInfo item ) {
		return string.Format(
@"<div class=""section"">
	<div class=""mb20 clear"" id="""">
		<h1 class=""heading inherit"">
			<a href=""{0}.html"">{0}</a>.{1}</h1>
		<div class=""clear""></div>
        <div class=""clear""></div>
        <div class=""clear""></div>
	</div>
	<div class=""subsection"">
		<div class=""signature"">
			<div class=""signature-CS sig-block"" style=""display: block;"">
			{2} {3} <span class=""sig-kw"">{1}</span>({4});
			</div>
		</div>
	</div>", parent.Name, item.Name, item.IsFamily ? "protected" : "public", "parameters" );
	}

	private static string GenerateMember( Type parent, MethodInfo item ) {
		return string.Format(
@"<div class=""section"">
	<div class=""mb20 clear"" id="""">
		<h1 class=""heading inherit"">
			<a href=""{0}.html"">{0}</a>.{1}</h1>
		<div class=""clear""></div>
        <div class=""clear""></div>
        <div class=""clear""></div>
	</div>
	<div class=""subsection"">
		<div class=""signature"">
			<div class=""signature-CS sig-block"" style=""display: block;"">
			{2} {3} <span class=""sig-kw"">{1}</span>({4});
			</div>
		</div>
	</div>", parent.Name, item.Name, item.IsFamily ? "protected" : "public", "parameters" );
	}

	private static string GenerateParameters() {
		return "";
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
}
