using UnityEngine;

[ AddComponentMenu( "Custom/Custom Component" ) ]
public class CustomComponent : MonoBehaviour
{
	public CustomAsset customAsset;
	
	public void Start()
	{
		Debug.Log( customAsset.text, gameObject );
	}
}