using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Class <c>UnitsManager</c> controls unit and enemy spawning. Plays a role at setting units for the menu
/// </summary>
public class UnitsManager : MonoBehaviour
{
    public static UnitsManager Instance;
    private List<ScriptableUnit> _units;
    public BaseHero SelectedHero;
    void Awake()
    {
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }

    /**
     * @desc Spawns a hero(s) at a tile. (Currently random, but will later be changed)
     * @param N/A
     */
    public void SpawnHeroes(){
        var heroCount = 1;

        for(int i = 0 ; i < heroCount ; i++){
            var randomPrefab = GetRandomUnit<BaseHero>(Faction.Hero);
            var spawnedHero = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();

            randomSpawnTile.SetUnit(spawnedHero);
        }

        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
    }

    /**
     * @desc Spawns an enemy(s) at a tile. (Currently random, but will later be changed)
     * @param N/A
     */
    public void SpawnEnemies(){
        var enemyCount = 1;

        for(int i = 0 ; i < enemyCount ; i++){
            var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
            var spawnedEnemy = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

            randomSpawnTile.SetUnit(spawnedEnemy);
        }

        GameManager.Instance.ChangeState(GameState.HeroesTurn);
    }

    /**
     * @desc Gets a random unit from the list of units.
     * @param N/A
     */
    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit{
        return (T)_units.Where(u=>u.Faction == faction).OrderBy(o=>Random.value).First().UnitPrefab;
    }

    /**
     * @desc Sets the selected hero for the menu.
     * @param {BaseHero} hero - The hero to be selected.
     */
    public void SetSelectedHero(BaseHero hero){
        SelectedHero = hero;
        MenuManager.Instance.ShowSelectedHero(hero);
    }


}
