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