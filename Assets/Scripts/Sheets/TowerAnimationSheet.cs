using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "towerAnimationSheet", menuName = "Sheets/TowerAnimationSheet", order = 1)]
public class TowerAnimationSheet : ScriptableObject{
    public Animator[] archerAnimators;
    public Animator[] barrackAnimators;
    public Animator[] magicAnimators;
    public Animator[] bombAnimators;
}