using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
	public void Click()
	{
		Debug.Log("Menu button clicked on " + gameObject.name);
		PlayerStatsManager.ReturnToMenu();
	}
}
