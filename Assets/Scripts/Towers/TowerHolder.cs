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

    void Awake()
    {
        UIMenu.SetActive(false);
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStatsManager>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void BuildTower(GameObject tower)
    {
        if (playerStats.SubtractGold(100) && towerInstance == null)
        {
            towerInstance = Instantiate(tower, transform.position, Quaternion.identity, transform);
            sprite.enabled = false;
        }
        else if (!playerStats.SubtractGold(100))
        {
            Debug.Log("nedeostatek peněz");
        }

        UIMenu.SetActive(false);
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

        UIMenu.SetActive(false);
    }

    void UpgradeTower()
    {

    }

    private void OnMouseDown()
    {
        Debug.Log("clicked");

        UIMenu.SetActive(!UIMenu.activeSelf);

        /*
        if (towerInstance == null)
        {
            BuildTower(TowerTypes.bombPrefab);
        }
        else SellTower();
        */
    }
}
