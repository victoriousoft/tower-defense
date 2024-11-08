using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PriceSheet
{
    public static Dictionary<string, TowerData> towerPriceDictionary = new Dictionary<string, TowerData>
    {
        { "BARRACKS", new TowerData {
            basePrice = 100,
            upgradePrices = new List<int> { 200, 300, 400 },
            evolutions = new Evolution[]
            {
                new Evolution {
                    specializationName = "barracks evo 1",
                    skills = new Skill[]
                    {
                        new Skill { skillName = "skill 1", upgradeCosts = new int[] { 100, 200, 300 } },
                        new Skill { skillName = "skill 2", upgradeCosts = new int[] { 100, 200, 300 } }
                    }
                },
                new Evolution {
                    specializationName = " evo 2",
                    skills = new Skill[]
                    {
                        new Skill { skillName = "skill 1", upgradeCosts = new int[] { 100, 200, 300 } },
                        new Skill { skillName = "skill 2", upgradeCosts = new int[] { 100, 200, 300 } }
                    }
                }
            }
        }},{ "ARCHER", new TowerData {
            basePrice = 100,
            upgradePrices = new List<int> { 200, 300, 400 },
            evolutions = new Evolution[]
            {
                new Evolution {
                    specializationName = " evo 1",
                    skills = new Skill[]
                    {
                        new Skill { skillName = "skill 1", upgradeCosts = new int[] { 100, 200, 300 } },
                        new Skill { skillName = "skill 2", upgradeCosts = new int[] { 100, 200, 300 } }
                    }
                },
                new Evolution {
                    specializationName = " evo 2",
                    skills = new Skill[]
                    {
                        new Skill { skillName = "skill 1", upgradeCosts = new int[] { 100, 200, 300 } },
                        new Skill { skillName = "skill 2", upgradeCosts = new int[] { 100, 200, 300 } }
                    }
                }
            }
        }},{ "MAGIC", new TowerData {
            basePrice = 100,
            upgradePrices = new List<int> { 200, 300, 400 },
            evolutions = new Evolution[]
            {
                new Evolution {
                    specializationName = " evo 1",
                    skills = new Skill[]
                    {
                        new Skill { skillName = "skill 1", upgradeCosts = new int[] { 100, 200, 300 } },
                        new Skill { skillName = "skill 2", upgradeCosts = new int[] { 100, 200, 300 } }
                    }
                },
                new Evolution {
                    specializationName = " evo 2",
                    skills = new Skill[]
                    {
                        new Skill { skillName = "skill 1", upgradeCosts = new int[] { 100, 200, 300 } },
                        new Skill { skillName = "skill 2", upgradeCosts = new int[] { 100, 200, 300 } }
                    }
                }
            }
        }},{ "BOMB", new TowerData {
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
                    specializationName = "Gripen",
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
