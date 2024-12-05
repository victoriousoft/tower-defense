using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TowerHolder : MonoBehaviour
{
    public GameObject UIMenu;
    private bool isMenuActive = false;
    private bool menuLocked = false;

    private GameObject towerInstance;
    private PlayerStatsManager playerStats;

    public GameObject barracksPrefab;
    public GameObject archerPrefab;
    public GameObject magicPrefab;
    public GameObject bombPrefab;
    private Dictionary<TowerTypes, GameObject> towerPrefabs;
    private BaseTower baseTowerScript = null;
    [HideInInspector] public Animator UIAnimator;
    private TowerButton[] towerButtons;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI infoText;

    // provizorní
    private Animator towerHolderAnimator;

    void Awake()
    {
        towerButtons = GetComponentsInChildren<TowerButton>();

        towerHolderAnimator = GetComponent<Animator>();
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();

        towerPrefabs = new Dictionary<TowerTypes, GameObject>
        {
            { TowerTypes.Barracks, barracksPrefab },
            { TowerTypes.Archer, archerPrefab },
            { TowerTypes.Magic, magicPrefab },
            { TowerTypes.Bomb, bombPrefab }
        };

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
            infoPanel.SetActive(false);
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

    public IEnumerator BuildTower(TowerTypes towerType)
    {
        if (playerStats.SubtractGold(towerPrefabs[towerType].GetComponent<BaseTower>().towerData.levels[0].price) && towerInstance == null)
        {
            menuLocked = true;
            towerHolderAnimator.Play("towerHolder_build");
            yield return new WaitForSecondsRealtime(1.5f);
            menuLocked = false;
            towerInstance = Instantiate(towerPrefabs[towerType], transform.position, Quaternion.identity, transform);
            baseTowerScript = towerInstance.GetComponent<BaseTower>();
            baseTowerScript.towerType = towerType;
        }
        else
        {
            Debug.Log("nedeostatek peněz");
        }
        yield return null;
    }

    public void SellTower()
    {
        if (towerInstance != null)
        {
            Destroy(towerInstance);
            BaseTower baseTower = towerInstance.GetComponent<BaseTower>();
            playerStats.AddGold(baseTower.towerData.levels[baseTower.level].price / 2);
            towerInstance = null;
            towerHolderAnimator.Play("towerHolder_idle");
        }
    }

    public void UpgradeTower()
    {
        //změna vzhledu towerky
        baseTowerScript.UpgradeTower();
    }

    public void ChangeTargeting()
    {
        // TODO: implement actual retargeting
        baseTowerScript.ChangeTargeting(TowerHelpers.TowerTargetTypes.CLOSEST_TO_FINISH);
    }

    private void OnMouseDown()
    {
        if (menuLocked) return;
        isMenuActive = !isMenuActive;
        if (!isMenuActive)
        {
            DisableMenu();
        }
        UIAnimator.SetTrigger("enable");
        if (isMenuActive) StartCoroutine(EnableButtons());
        if (towerInstance == null) towerHolderAnimator.Play("towerHolder_pop");
    }

    private void DisableMenu()
    {
        isMenuActive = false;
        foreach (TowerButton button in towerButtons)
        {
            if (!button.isActiveAndEnabled) return;
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
        if (towerType == TowerTypes.Retarget) return;

        infoPanel.SetActive(true);

        if (towerType == TowerTypes.Upgrade)
        {
            infoText.text = "level " + (baseTowerScript.level + 1) + "\n" +
                            "dmg- " + baseTowerScript.towerData.levels[baseTowerScript.level].damage + "(+" + (
                            baseTowerScript.towerData.levels[baseTowerScript.level].damage - baseTowerScript.towerData.levels[baseTowerScript.level - 1].damage
                            ) + ")" + "\n" +
                        "cost- " + baseTowerScript.towerData.levels[baseTowerScript.level].damage;
        }
        else if (towerType == TowerTypes.Destroy)
        {
            infoText.text = "Cashback- " + baseTowerScript.towerData.levels[baseTowerScript.level].price / 2;
        }
        else
        {
            infoText.text = baseTowerScript.towerData.name
             + "\n" +
                        "dmg- " + baseTowerScript.towerData.levels[0].damage + "\n" +
                        "cost- " + baseTowerScript.towerData.levels[0].price;
        }

        Vector2 mousePosition = Input.mousePosition;
        infoPanel.transform.position = mousePosition;
    }
}