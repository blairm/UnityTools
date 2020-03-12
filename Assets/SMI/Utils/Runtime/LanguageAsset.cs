using UnityEngine;

using System.Collections.Generic;

#if UNITY_EDITOR
	using System.Xml;
#endif

namespace SMI.Utils.Runtime
{
	public class LanguageAsset : ScriptableObject
	{
		public const string DEFAULT_TEXT = "NULL";

		public string[] keyArray;
		public string[] valueArray;

#if UNITY_EDITOR
		//keep this so we don't have to set the xml file each
		//time we select this asset but only in the editor
		public TextAsset XMLFile;
#endif

		public string getText( string key )
		{
			string result = DEFAULT_TEXT;

			for( int i = 0; i < keyArray.Length; ++i )
			{
				if( key == keyArray[ i ] )
				{
					result = valueArray[ i ];
					break;
				}
			}

			if( result == DEFAULT_TEXT )
				Debug.LogError( "unable to retrieve text with key " + key, this );

			return result;
		}

		public void setKeyValuePairs( List< string > keyList, List< string > valueList )
		{
			keyArray	= keyList.ToArray();
			valueArray	= valueList.ToArray();
		}

#if UNITY_EDITOR
		public void parseXML( XmlNode xmlNode, List< string > path, List< string > keyList, List< string > valueList )
		{
			XmlNode childNode;

			for( int i = 0; i < xmlNode.ChildNodes.Count; ++i )
			{
				childNode = xmlNode.ChildNodes[ i ];

				switch( childNode.Name )
				{
					case "section":
					{
						for( int j = 0 ; j < childNode.Attributes.Count; ++j )
						{
							if( childNode.Attributes[ j ].Name == "name" )
							{
								path.Add( childNode.Attributes[ j ].Value + "/" );
								break;
							}
						}
					
						int count = path.Count;

						if( count > 1 )
							path[ count - 1 ] = path[ count - 2 ] + path[ count - 1 ];

						parseXML( childNode, path, keyList, valueList );
						path.RemoveAt( count - 1 );

						break;
					}
					case "text":
					{
						int count		= path.Count;
						string key		= path[ count - 1 ];
						string value	= "";

						for( int j = 0; j < childNode.Attributes.Count; ++j )
						{
							switch( childNode.Attributes[ j ].Name )
							{
								case "name":	key		+= childNode.Attributes[ j ].Value;	break;
								case "value":	value	= childNode.Attributes[ j ].Value;	break;
							}
						}

						keyList.Add( key );
						valueList.Add( value );
						break;
					}
					default:
					{
						Debug.LogError( "unable to handle " + childNode.Name + " node type" );
						break;
					}
				}
			}
		}
#endif
	}
}