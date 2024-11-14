using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TowerSheet
{
    public static Dictionary<string, TowerData> towerDictionary = new Dictionary<string, TowerData>
    {
        { "BARRACKS", new TowerData {
            towerName = "Barracks",
            basePrice = 100,
            upgradePrices = new int[] { 200, 300, 400 },
            damageValues = new int[] { 1000, 10000, 50000 },
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
        }},{ "ARCHER", new TowerData {
            towerName = "Archers Hideout",
            basePrice = 200,
            upgradePrices = new int[] { 200, 300, 400 },
            damageValues = new int[] { 200, 300, 400 },
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
            basePrice = 300,
            upgradePrices = new int[] { 200, 300, 400 },
            damageValues = new int[] { 200, 300, 400 },
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
            basePrice = 400,
            upgradePrices = new int[] { 200, 300, 400 },
            damageValues = new int[] { 200, 300, 400 },
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
    public int[] upgradePrices;
    public int[] damageValues;
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
