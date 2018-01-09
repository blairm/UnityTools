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

using UnityEditor;
using UnityEngine;

using SMI.Utils.Editor;

public class CustomEditorWindow : EditorWindow
{
	[ MenuItem( "Assets/Create/Custom Asset" ) ]
	static public void CreateAsset()
	{
		CustomAssetUtility.CreateAsset< CustomAsset >();
	}
	
	[ MenuItem( "Tools/Custom Window" ) ]
	static public void ShowWindow()
	{
		EditorWindow.GetWindow<CustomEditorWindow>();
	}

	public void OnGUI()
	{
		if( Selection.activeObject != null )
		{
			if( Selection.activeObject is CustomAsset )
			{
				activeObject	= Selection.activeObject as CustomAsset;
				content			= activeObject.text;
			}
		}
		
		
		GUILayout.Label( "Custom Asset", EditorStyles.boldLabel );
		
		
		EditorGUILayout.BeginHorizontal();
		
		EditorGUILayout.PrefixLabel( "Name" );
		
		if( activeObject != null )
			GUILayout.Label( activeObject.name, EditorStyles.label );
		else
			GUILayout.Label( "none", EditorStyles.label );
		
		EditorGUILayout.EndHorizontal();
		
		
		content = EditorGUILayout.TextField( "Content", content, EditorStyles.textField );
		
		if( activeObject != null && content != activeObject.text )
		{
			activeObject.text = content;
			EditorUtility.SetDirty(activeObject);
		}
		
		
		if( GUILayout.Button( "Create", EditorStyles.miniButton ) )
			CreateAsset();
	}
	
	public void OnInspectorUpdate()
	{
	    Repaint();
	}
	
	
	private CustomAsset activeObject;
	
	private string	content = "";
}