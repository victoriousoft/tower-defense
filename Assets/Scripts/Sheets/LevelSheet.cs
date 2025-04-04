using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelSheet : ScriptableObject
{
	[System.Serializable]
	public class Level
	{
		public string SceneName;
		public string LevelName;
		public int initialGold;
		public int initialLives;

		public EvolutionLock[] evolutionLocks;
	}

	[System.Serializable]
	public class EvolutionLock
	{
		public TowerTypes towerType;

		[Range(0, 1)]
		public int evolutionIndex;
	}

	public string mainMenuSceneName;
	public Level[] levels;
}
