using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    [SerializeField]
    int width;

    [SerializeField]
    int height;

    long seed;

    [SerializeField]
    DungeonTile tilePrefab;

    [SerializeField]
    BoxCollider2D cameraExtents;

    [SerializeField]
    GameObject player;

    int playerX;
    int playerY;

    // upper limit of monsters
    const int MAX_MONSTERS = 20;
    int monstersRemaining = MAX_MONSTERS;

    //chance that a monster will spawn when a tile is revealed
    [SerializeField]
    float monsterSpawnChance = 0.2f;

    [SerializeField]
    int monsterMinHealth = 10;

    [SerializeField]
    int monsterMaxHealth = 30;

    Dictionary<Vector2Int, DungeonTile> tiles = new Dictionary<Vector2Int, DungeonTile>();

    [SerializeField]
    GameObject[] monsterPrefabs;

    [Serializable]
    public class RevealedMonster
    {
        public int health;
        public GameObject obj;
        public bool hasTreasure;
    }

    Dictionary<Vector2Int, RevealedMonster> revealedMonsters = new Dictionary<Vector2Int, RevealedMonster>();

    // upper limit of treasures
    const int MAX_TREASURES = 5;
    int treasuresRemaining = MAX_TREASURES;

    [Serializable]
    public class RevealedTreasure
    {
        public int contents; //what kind of treasure?
        public GameObject obj;
    }

    [SerializeField]
    float treasureSpawnChance = 0.05f;

    [SerializeField]
    float treasureSpawnOnMonsterChance = 0.5f;

    Dictionary<Vector2Int, RevealedTreasure> revealedTreasures = new Dictionary<Vector2Int, RevealedTreasure>();

    [SerializeField]
    GameObject treasurePrefab;

    [SerializeField]
    float trapSpawnChance = 0.1f;

    const int MAX_TRAPS = 20;
    int trapsRemaining = MAX_TRAPS;

    //the exit
    int exitX;
    int exitY;

    [SerializeField]
    GameObject exitPrefab;


    [SerializeField]
    int playerAttack = 5;

    [SerializeField]
    int monsterAttack = 5;

    [SerializeField]
    float critChance = 0.05f;

    [SerializeField]
    int playerHealth = 100;

    public int MaxPlayerHealth => playerHealth;

    public int PlayerHealth {get; private set;}

    [SerializeField]
    int playerPotions = 5;

    public int MaxPotions => playerPotions;

    public int Potions { get; private set; }


    [SerializeField]
    int potionHealing = 10;


    [SerializeField]
    DungeonUI ui;

    [SerializeField]
    int minTrapDamage = 2;

    [SerializeField]
    int maxTrapDamage = 10;

    public int EnemiesKilled { get; private set; }

    public int TreasuresCollected => revealedTreasures.Count;


    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x=0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                var newTile = Instantiate(tilePrefab, transform);
                newTile.gameObject.layer = LayerMask.NameToLayer("DungeonTiles");
                newTile.transform.localPosition = new Vector3(x, y);
                newTile.name = $"Tile {x} {y}";
                newTile.Init((x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0));
                int cx = x;
                int cy = y;
                newTile.OnTileSelected = () => OnTileSelected(cx,cy,newTile);

                tiles.Add(new Vector2Int(x, y), newTile);

            }
        }

        cameraExtents.size = new Vector2(width + 1, height + 1);
        cameraExtents.offset = new Vector2(width * 0.5f - 0.5f, height * 0.5f - 0.5f);

        //place the player spawn
        int spawnX = Random.Range(0, width);
        int spawnY = Random.Range(0, height);

        playerX = spawnX;
        playerY = spawnY;

        tiles[new Vector2Int(playerX, playerY)].Reveal();

        player.transform.localPosition = new Vector3(spawnX, spawnY, 0);

        PlayerHealth = MaxPlayerHealth;
        Potions = MaxPotions;

        //place the exit
        exitX = Random.Range(0, width);
        exitY = Random.Range(0, height);
        var exitRerolls = 100;
        //use Manhanttan distance to make sure exit is far away enough
        while ( exitRerolls > 0 && (Mathf.Abs(spawnX-exitX) + Mathf.Abs(spawnY - exitY) < 10))
        {
            Debug.Log("Rerolling exit");
            exitX = Random.Range(0, width);
            exitY = Random.Range(0, height);
            exitRerolls--;
        }

    }
    
    void OnTileSelected(int x, int y, DungeonTile tile)
    {
        if (((Mathf.Abs(playerX - x) <= 1) && (Mathf.Abs(playerY - y) == 0))
            || ((Mathf.Abs(playerX - x) == 0) && (Mathf.Abs(playerY - y) <= 1)))
        {
            Move(x, y);
        }
    }

    Tween tween;

    private void Update()
    {
        if (tween == null || !tween.active)
        {
            int destX = playerX;
            int destY = playerY;

            //check for intput
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                destY++;
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                destY--;
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                destX--;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                destX++;
            }

            if (destX != playerX || destY != playerY)
            {
                if ((destX >= 0 && destX < width) && (destY >= 0 && destY < height))
                {
                    if ((Mathf.Abs(playerX - destX) <= 1) && (Mathf.Abs(playerY - destY) <= 1))
                    {
                        Move(destX, destY);
                    }
                }
            }
        }
    }

    void Move(int x, int y)
    {
        var tilePos = new Vector2Int(x, y);

        var tile = tiles[tilePos];
        if (!tile.IsRevealed)
        {
            RevealTile(x, y);
        }

        if (revealedMonsters.TryGetValue(tilePos, out var monster))
        {
            if (monster.health > 0)
            {
                //attack!

                //roll to see who goes first
                if (Random.value < 0.5f)
                {
                    //player attacks
                    AttackMonster(tilePos, monster);
                    if (monster.health > 0)
                    {
                        AttackPlayer(tilePos, monster);
                        if (PlayerHealth <= 0)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    //monster attacks
                    AttackPlayer(tilePos, monster);
                    if (PlayerHealth > 0)
                    {
                        AttackMonster(tilePos, monster);
                    }
                    else
                    {
                        return;
                    }
                }

                ui.ShowDamageFx();

                return;
            }
        }

        playerX = x;
        playerY = y;
        tween = player.transform.DOLocalMove(new Vector3(x, y, 0), 0.33f);

        //if exit ask if they want to exit dungeon
        if (x == exitX && y == exitY)
        {
            //prompt exit
        }
    }

    void RevealTile(int x, int y)
    {
        var tilePos = new Vector2Int(x, y);

        var tile = tiles[tilePos];

        //is it the exit?
        if (x == exitX && y == exitY)
        {
            //reveal exit
            var exitObj = Instantiate(exitPrefab, transform);
            exitObj.transform.localPosition = new Vector3(x, y, 0);
            ui.ShowLog("You found the exit");
            return;
        }

        //is it a trap?

        if (trapsRemaining > 0 && Random.value < trapSpawnChance)
        {
            ui.ShowDamageFx();
            //it's a trap
            var trapDamage = Random.Range(minTrapDamage, maxTrapDamage);
            PlayerHealth -= trapDamage;
            ui.ShowLog($"You sprung a trap! You take {trapDamage} damage!!!");
            if (PlayerHealth <= 0)
            {
                ui.ShowLog($"You died!");
                ui.ShowGameOver();
            }

            trapsRemaining--;
            tile.Reveal(true);
            return;
        }

        if (monstersRemaining > 0 && Random.value < monsterSpawnChance)
        {
            //spawn a monster
            monstersRemaining--;

            ui.ShowLog($"You stumble upon a monster!!!");

            var monsterHasTreasure = false;
            if (treasuresRemaining > 0 && Random.value < treasureSpawnChance)
            {
                treasuresRemaining--;
                monsterHasTreasure = true;
            }

            var monsterObj = Instantiate(monsterPrefabs[Random.Range(0, monsterPrefabs.Length)], transform);
            monsterObj.transform.localPosition = new Vector3(x, y, 0);

            revealedMonsters.Add(tilePos, new RevealedMonster()
            {
                health = Random.Range(monsterMinHealth, monsterMaxHealth),
                obj = monsterObj,
                hasTreasure = monsterHasTreasure
            });
        }
        else if (treasuresRemaining > 0 && Random.value < treasureSpawnChance)
        {
            var treasureObj = Instantiate(treasurePrefab, transform);
            treasureObj.transform.localPosition = new Vector3(x, y, 0);

            ui.ShowLog("You found a treasure!");

            treasuresRemaining--;
            revealedTreasures.Add(tilePos, new RevealedTreasure()
            {
                contents = 0,
                obj = treasureObj
            });
        }

        tile.Reveal();

    }

    void AttackMonster(Vector2Int tilePos, RevealedMonster monster)
    {
        bool crit = Random.value < critChance;
        var damage = Random.Range(1, monsterAttack);
        if (crit)
        {
            damage = playerAttack + damage;
            ui.ShowLog($"You CRIT the monster for {damage} damage!!!");
        }
        else if (damage > 0)
        {
            ui.ShowLog($"You hit the monster for {damage} damage!");
        }
        else
        {
            ui.ShowLog("You missed!The monster laughs at you.");
        }
        monster.health -= damage;
        if ( monster.health > 20 )
        {
            ui.ShowLog($"The monster looks angry!");
        }
        else if (monster.health > 10)
        {
            ui.ShowLog($"The monster is bleeding!");
        }
        else
        {
            ui.ShowLog($"The monster is struggling to stay up!");
        }

        if (monster.health <= 0)
        {
            Destroy(monster.obj);

            if (monster.hasTreasure)
            {
                var treasureObj = Instantiate(treasurePrefab, transform);
                treasureObj.transform.localPosition = new Vector3(tilePos.x, tilePos.y, 0);
                revealedTreasures.Add(tilePos, new RevealedTreasure()
                {
                    contents = 0,
                    obj = treasureObj
                });

                ui.ShowLog("The monster died and dropped a treasure!");
            }
            else
            {
                ui.ShowLog("The monster died!");
            }
            
        }
    }

    void AttackPlayer(Vector2Int tilePos, RevealedMonster monster)
    {
        bool crit = Random.value < critChance;
        var damage = Random.Range(1, monsterAttack);
        if ( crit )
        {
            damage = monsterAttack + damage;
            ui.ShowLog($"The monster CRITS you for {damage} damage!!!");
        }
        else if (damage > 0)
        {
            ui.ShowLog($"The monster hits you for {damage} damage!");
        }
        else
        {
            ui.ShowLog("The monster missed!");
        }
        PlayerHealth -= damage;
        if (PlayerHealth <= 0 )
        {
            ui.ShowLog("You died!");
            //GameOver
            ui.ShowGameOver();
        }
    }

    public void UsePotion()
    {
        if (Potions > 0)
        {
            Potions--;
            PlayerHealth = Math.Min(PlayerHealth + potionHealing, MaxPlayerHealth);
            ui.ShowLog("You used a potions and healed a bit");
        }
    }

    public void Quit()
    {
        SceneManager.LoadScene("Main");
    }

}
