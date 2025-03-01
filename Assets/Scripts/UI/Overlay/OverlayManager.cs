using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Overlay : MonoBehaviour
{
	public TextMeshProUGUI goldText;
	public TextMeshProUGUI healthText;

	void Update()
	{
		goldText.text = "Gold: " + PlayerStatsManager.gold;
		healthText.text = "Health: " + PlayerStatsManager.lives;
	}
}
