using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHolder : MonoBehaviour
{
    public GameObject UIMenu;

    private GameObject towerInstance;
    private PlayerStatsManager playerStats;
    private SpriteRenderer sprite;

    public GameObject barracksPrefab;
    public GameObject archerPrefab;
    public GameObject magicPrefab;
    public GameObject bombPrefab;
    private Dictionary<TowerTypes, GameObject> towerPrefabs;
    private BaseTower baseTowerScript = null;
    [HideInInspector]public Animator UIAnimator;
    private TowerButton[] towerButtons;

    void Awake()
    {
        towerButtons = GetComponentsInChildren<TowerButton>();

        UIAnimator = GetComponent<Animator>();
        UIMenu.SetActive(false);
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
        sprite = GetComponent<SpriteRenderer>();

        towerPrefabs = new Dictionary<TowerTypes, GameObject>
        {
            { TowerTypes.Barracks, barracksPrefab },
            { TowerTypes.Archer, archerPrefab },
            { TowerTypes.Magic, magicPrefab },
            { TowerTypes.Bomb, bombPrefab }
        };
    }

    void Update(){
        if (UIMenu.activeSelf && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collider = GetComponent<Collider2D>();

            if (collider != null && !collider.OverlapPoint(mousePosition))
            {
                UIMenu.SetActive(false);
                foreach (TowerButton button in towerButtons)
                {
                    button.gameObject.GetComponent<Animator>().SetTrigger("disable");
                    UIAnimator.SetTrigger("enable");
                }
            }
        }
        
    }

    public void BuildTower(TowerTypes towerType)
    {
        if (playerStats.SubtractGold(TowerSheet.towerDictionary[towerType].basePrice) && towerInstance == null)
        {
            towerInstance = Instantiate(towerPrefabs[towerType], transform.position, Quaternion.identity, transform);
            baseTowerScript = towerInstance.GetComponent<BaseTower>();
            baseTowerScript.towerType = towerType;
            baseTowerScript.towerName = TowerSheet.towerDictionary[towerType].towerName;
            baseTowerScript.damage = TowerSheet.towerDictionary[towerType].damageValues[0];
            sprite.enabled = false;
        }
        else if (!playerStats.SubtractGold(100))
        {
            Debug.Log("nedeostatek peněz");
        }
    }

    public void SellTower()
    {
        if (towerInstance != null)
        {

            Destroy(towerInstance);
            towerInstance = null;
            playerStats.AddGold(100);
            sprite.enabled = true;
        }
    }

    public void UpgradeTower()
    {
        //změna vzhledu towerky
        baseTowerScript.UpgradeTower();
    }

    public void ChangeTargeting(){
        baseTowerScript.ChangeTargeting();
    }

    private void OnMouseDown()
    {
        UIMenu.SetActive(!UIMenu.activeSelf);
        if (!UIMenu.activeSelf)
        {
            foreach (TowerButton button in towerButtons)
            {
                button.gameObject.GetComponent<Animator>().SetTrigger("disable");
            }
        UIMenu.SetActive(false);
    }
        UIAnimator.SetTrigger("enable");
        if(UIMenu.activeSelf)StartCoroutine(EnableButtons());
    }
    private IEnumerator EnableButtons()
    {
        yield return new WaitForSeconds(0.15f);
        foreach (TowerButton button in towerButtons)
        {
            if(towerInstance == null){
                if(button.towerType == TowerTypes.Barracks || button.towerType == TowerTypes.Archer || button.towerType == TowerTypes.Magic || button.towerType == TowerTypes.Bomb){
                    button.gameObject.GetComponent<Animator>().SetTrigger("enable");
                }else{
                    button.gameObject.GetComponent<Animator>().SetTrigger("disable");   
                }
            }
            else{
                if(button.towerType == TowerTypes.Upgrade || button.towerType == TowerTypes.Destroy || button.towerType == TowerTypes.Retarget){
                    button.gameObject.GetComponent<Animator>().SetTrigger("enable");
                }else{
                    button.gameObject.GetComponent<Animator>().SetTrigger("disable");
                }
            }
            yield return new WaitForSeconds(0.05f);
        }
    }


}
