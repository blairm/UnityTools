using UnityEngine;

using SMI.PostProcessEffects.Runtime;

public class HeatwaveTest : MonoBehaviour
{
    public GameObject	heatwavePrefab;

	public int			heatwaveCount	= 8;

	public void Awake()
	{
		heatwaveCount	= Mathf.NextPowerOfTwo( heatwaveCount );
		heatwaveList	= new HeatwaveImageEffect[ heatwaveCount ];

		for( int i = 0; i < heatwaveCount; ++i )
		{
			GameObject go		= ( GameObject ) Instantiate( heatwavePrefab );
			heatwaveList[ i ]	= go.GetComponent< HeatwaveImageEffect >();
			go.SetActive( false );
		}
	}

    public void Update()
    {
        if( Input.GetMouseButtonDown( 0 ) )
		{
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			RaycastHit hit;
			if( Physics.Raycast( ray, out hit, 1000 ) )
			{
				heatwaveList[ heatwaveIndex ].transform.position = hit.point;
				heatwaveList[ heatwaveIndex ].explode();
				heatwaveIndex = ++heatwaveIndex & ( heatwaveCount - 1 );
			}
		}
    }

	private HeatwaveImageEffect[]	heatwaveList;
	private int						heatwaveIndex	= 0;
}