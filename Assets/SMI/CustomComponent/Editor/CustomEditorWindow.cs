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