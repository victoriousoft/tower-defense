using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TowerSheet
{
    public static Dictionary<TowerTypes, TowerData> towerDictionary = new Dictionary<TowerTypes, TowerData>
    {
        { TowerTypes.Barracks, new TowerData {
            towerName = "Barracks",
            prices = new int[] {200, 200, 300},
            refundValues = new int[] {101, 102, 103},
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
        }},{ TowerTypes.Archer, new TowerData {
            towerName = "Archers Hideout",
            prices = new int[] { 200, 200, 300},
            refundValues = new int[] {101, 102, 103},
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
        }},{ TowerTypes.Magic, new TowerData {
            towerName = "Tech Center",
            prices = new int[] {200, 200, 300},
            refundValues = new int[] {101, 102, 103},
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
        }},{ TowerTypes.Bomb, new TowerData {
            towerName = "U.C.M",
            prices = new int[] {400, 200, 300},
            refundValues = new int[] {101, 102, 103},
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
    public int[] prices;
    public int[] refundValues;
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
