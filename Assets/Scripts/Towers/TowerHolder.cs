using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class TowerHolder : MonoBehaviour
{
    public GameObject UIMenu;
    private bool isMenuActive = false;

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

    // Reference to the UI Panel and Text
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI infoText; // Use TextMeshProUGUI

    void Awake()
    {
        towerButtons = GetComponentsInChildren<TowerButton>();

        UIAnimator = GetComponent<Animator>();
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
        sprite = GetComponent<SpriteRenderer>();

        towerPrefabs = new Dictionary<TowerTypes, GameObject>
        {
            { TowerTypes.Barracks, barracksPrefab },
            { TowerTypes.Archer, archerPrefab },
            { TowerTypes.Magic, magicPrefab },
            { TowerTypes.Bomb, bombPrefab }
        };

        // Initially hide the info panel
        infoPanel.SetActive(false);
    }

    void Update()
    {
        if (isMenuActive && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collider = GetComponent<Collider2D>();

            if (collider != null && !collider.OverlapPoint(mousePosition))
            {
                DisableMenu();
            }
        }

        TowerTypes? towerType = GetTowerTypeUnderCursor();
        if (towerType.HasValue)
        {
            PrintTowerInfo(towerType.Value);
        }
        else
        {
            infoPanel.SetActive(false); // Hide the info panel if not over a button
        }
    }

    private TowerTypes? GetTowerTypeUnderCursor()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            TowerButton towerButton = hit.collider.GetComponent<TowerButton>();
            if (towerButton != null)
            {
                return towerButton.towerType;
            }
        }

        return null;
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

    public void ChangeTargeting()
    {
        baseTowerScript.ChangeTargeting();
    }

    private void OnMouseDown()
    {
        isMenuActive = !isMenuActive;
        if (!isMenuActive)
        {
            DisableMenu();
        }
        UIAnimator.SetTrigger("enable");
        if (isMenuActive) StartCoroutine(EnableButtons());
    }

    private void DisableMenu()
    {
        isMenuActive = false;
        foreach (TowerButton button in towerButtons)
        {
            button.gameObject.GetComponent<Animator>().Play("disableButton");
            UIAnimator.SetTrigger("enable");
        }
    }

    private IEnumerator EnableButtons()
    {
        yield return new WaitForSeconds(0.15f);
        foreach (TowerButton button in towerButtons)
        {
            if (towerInstance == null)
            {
                if (button.towerType == TowerTypes.Barracks || button.towerType == TowerTypes.Archer || button.towerType == TowerTypes.Magic || button.towerType == TowerTypes.Bomb)
                {
                    button.gameObject.GetComponent<Animator>().Play("enableButton");
                }
                else
                {
                    button.gameObject.GetComponent<Animator>().Play("disableButton");
                }
            }
            else
            {
                if (button.towerType == TowerTypes.Upgrade || button.towerType == TowerTypes.Destroy || button.towerType == TowerTypes.Retarget)
                {
                    button.gameObject.GetComponent<Animator>().Play("enableButton");
                }
                else
                {
                    button.gameObject.GetComponent<Animator>().Play("disableButton");
                }
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void PrintTowerInfo(TowerTypes towerType)
    {
        if (towerType == TowerTypes.Destroy || towerType == TowerTypes.Upgrade || towerType == TowerTypes.Retarget) return;
        infoPanel.SetActive(true);
        infoText.text = TowerSheet.towerDictionary[towerType].towerName + "\n" +
                        "dmg- " + TowerSheet.towerDictionary[towerType].damageValues[0] + "\n" +
                        "cost- " + TowerSheet.towerDictionary[towerType].basePrice;

        Vector2 mousePosition = Input.mousePosition;
        infoPanel.transform.position = mousePosition;
    }
}