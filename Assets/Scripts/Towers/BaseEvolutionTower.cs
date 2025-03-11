using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEvolutionTower : BaseTower
{
	protected int[] skillLevels = new int[2] { 0, 0 };

	void Start()
	{
		Debug.Log("BaseEvolutionTower Start");
	}
}
