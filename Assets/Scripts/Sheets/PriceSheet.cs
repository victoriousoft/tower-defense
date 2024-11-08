using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PriceSheet
{
    public static Dictionary<string, TowerData> towerPriceDictionary = new Dictionary<string, TowerData>
    {
        { "BARRACKS", new TowerData {
            towerName = "Barracks",
            basePrice = 100,
            upgradePrices = new List<int> { 200, 300, 400 },
            evolutions = new Evolution[]
            {
                new Evolution {
                    specializationName = "Spartan Hoplites",
                    skills = new Skill[]
                    {
                        new Skill { skillName = "skill 1", upgradeCosts = new int[] { 100, 200, 300 } },
                        new Skill { skillName = "skill 2", upgradeCosts = new int[] { 100, 200, 300 } }
                    }
                },
                new Evolution {
                    specializationName = "Musketeer Quarters",
                    skills = new Skill[]
                    {
                        new Skill { skillName = "skill 1", upgradeCosts = new int[] { 100, 200, 300 } },
                        new Skill { skillName = "skill 2", upgradeCosts = new int[] { 100, 200, 300 } }
                    }
                }
            }
        }},{ "ARCHERS", new TowerData {
            towerName = "Archers Hideout",
            basePrice = 100,
            upgradePrices = new List<int> { 200, 300, 400 },
            evolutions = new Evolution[]
            {
                new Evolution {
                    specializationName = "MG 42",
                    skills = new Skill[]
                    {
                        new Skill { skillName = "skill 1", upgradeCosts = new int[] { 100, 200, 300 } },
                        new Skill { skillName = "skill 2", upgradeCosts = new int[] { 100, 200, 300 } }
                    }
                },
                new Evolution {
                    specializationName = "Sharpshooters Parapet",
                    skills = new Skill[]
                    {
                        new Skill { skillName = "skill 1", upgradeCosts = new int[] { 100, 200, 300 } },
                        new Skill { skillName = "skill 2", upgradeCosts = new int[] { 100, 200, 300 } }
                    }
                }
            }
        }},{ "MAGIC", new TowerData {
            towerName = "Tech Center",
            basePrice = 100,
            upgradePrices = new List<int> { 200, 300, 400 },
            evolutions = new Evolution[]
            {
                new Evolution {
                    specializationName = "Anonymous",
                    skills = new Skill[]
                    {
                        new Skill { skillName = "Stack Overflow", upgradeCosts = new int[] { 100, 200, 300 } },
                        new Skill { skillName = "skill 2", upgradeCosts = new int[] { 100, 200, 300 } }
                    }
                },
                new Evolution {
                    specializationName = "IBM",
                    skills = new Skill[]
                    {
                        new Skill { skillName = "1. 1. 1970", upgradeCosts = new int[] { 100, 200, 300 } },
                        new Skill { skillName = "skill 2", upgradeCosts = new int[] { 100, 200, 300 } }
                    }
                }
            }
        }},{ "BOMB", new TowerData {
            towerName = "Bomb Tower",
            basePrice = 100,
            upgradePrices = new List<int> { 200, 300, 400 },
            evolutions = new Evolution[]
            {
                new Evolution {
                    specializationName = "Suicide Bombers",
                    skills = new Skill[]
                    {
                        new Skill { skillName = "skill 1", upgradeCosts = new int[] { 100, 200, 300 } },
                        new Skill { skillName = "skill 2", upgradeCosts = new int[] { 100, 200, 300 } }
                    }
                },
                new Evolution {
                    specializationName = "Saab JAS-39 Gripen",
                    skills = new Skill[]
                    {
                        new Skill { skillName = "skill 1", upgradeCosts = new int[] { 100, 200, 300 } },
                        new Skill { skillName = "skill 2", upgradeCosts = new int[] { 100, 200, 300 } }
                    }
                }
            }
        }},
    };
}

[System.Serializable]
public class TowerData
{
    public string towerName;
    public int basePrice;
    public List<int> upgradePrices;
    public Evolution[] evolutions;
}
[System.Serializable]
public class Skill
{
    public string skillName;
    public int[] upgradeCosts;
}

[System.Serializable]
public class Evolution
{
    public string specializationName;
    public Skill[] skills;
}
