/*
This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <http://unlicense.org>
//*/

using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;
using System.Xml;

using SMI.Utils.Runtime;

namespace SMI.Utils.Editor
{
	[ CustomEditor( typeof( LanguageAsset ) ) ]
	public class LanguageComponentEditor : UnityEditor.Editor
	{
		[ MenuItem( "Assets/Create/SMI/Utils/Language Asset" ) ]
		static public void CreateAsset()	{ CustomAssetUtility.CreateAsset< LanguageAsset >(); }

		public void OnEnable()
		{
			_target = target as LanguageAsset;
		}
	
		override public void OnInspectorGUI()
		{
			_target.XMLFile = ( TextAsset ) EditorGUILayout.ObjectField( "XML Language File", _target.XMLFile, typeof( TextAsset ), false );

			if( GUILayout.Button( "Import XML", EditorStyles.miniButton ) )
			{
				Undo.RecordObject( _target, "Import XML" );

				XmlDocument doc = new XmlDocument();
				doc.LoadXml( _target.XMLFile.text );

				for( int i = 0; i < doc.ChildNodes.Count; ++i )
				{
					if( doc.ChildNodes[ i ].Name == "textData" )
					{
						XmlNode xmlNode				= doc.ChildNodes[ i ];
						List< string > path			= new List< string >();
						List< string > keyList		= new List< string >();
						List< string > valueList	= new List< string >();

						_target.parseXML( xmlNode, path, keyList, valueList );
						_target.setKeyValuePairs( keyList, valueList );
						Debug.Log( "Language XML imported" );
						break;
					}
				}
			}

			if( GUI.changed )
				EditorUtility.SetDirty( _target );
		}
	
	
		private LanguageAsset _target;
	}
}