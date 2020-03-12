using UnityEditor;
using UnityEngine;

namespace SMI.Utils.Editor
{
	public class DeletePlayerPrefs
	{
		[ MenuItem( "SMI/Utils/Delete PleyerPref Data" ) ]
		static void deletePlayerPrefs()	{ PlayerPrefs.DeleteAll(); }
	}
}
