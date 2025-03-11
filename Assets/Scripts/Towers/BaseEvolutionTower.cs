using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEvolutionTower : BaseTower
{
	[HideInInspector]
	public int[] skillLevels = new int[2] { 0, 0 };

	[HideInInspector]
	public int evolutionIndex = -1;

	void Start()
	{
		Debug.Log("BaseEvolutionTower Start");
	}
}
