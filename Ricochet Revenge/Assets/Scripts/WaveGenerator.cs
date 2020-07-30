using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class WaveGenerator : MonoBehaviour
{
    public GameObject player;

    public GameObject meleeEnemy;
    public GameObject tankEnemy;
    public ArrayList enemyList;
    public int enemyCount;

    public Tilemap tilemap;

    [Range(0.1f, 3f)]
    public float waveDelay = 2f;
    [Range(0.1f, 2f)]
    public float spawnDelay = 0.1f;
    [Range(1f, 5f)]
    public float spawnRadiusMin = 1f;
    [Range(5f, 10f)]
    public float spawnRadiusMax = 5f;

    public Camera cam;
    public Vector2 arenaSize;

    public int waveNumber = 1;
    public int waveDanger = 1;
    [Range(0.1f, 2f)]
    public float dangerCurve = 1f;
    private int maxEnemyDanger;

    public ShieldSpawner shieldSpawner;

    public GameObject WavePanel;

    private VariableTrackerScript varTrack;

    public LevelAnalytics analytics;

    void Start()
    {
        maxEnemyDanger = TankEnemyBehaviour.DANGER; //has to be manually changed if stronger enemy
        enemyList = new ArrayList();
        NextWave();
        varTrack = GameObject.FindWithTag("VarTrack").GetComponent<VariableTrackerScript>();
    }

    public void NextWave()
    {
        StartCoroutine(SpawnWave());
    }

    /*
     * each wave can have a difficulty number
     * and waves can spawn that number of enemies to meet the difficulty number
     * Make spawning part of the corutine with micro-pauses in spawing
     */
    private IEnumerator SpawnWave()
    {
        // sets up wave
        WaveSetup();

        WavePanel.GetComponentInChildren<Text>().text = "Wave: " + waveNumber;
        WavePanel.SetActive(true);
        Invoke("WavePanelHide", 1);

        yield return new WaitForSeconds(waveDelay + 1);

        // spawn enemies with delays
        foreach(GameObject enemy in enemyList)
        {
            Vector2 spawnPos = spawnPoint();

            float camSize = cam.GetComponent<Camera>().orthographicSize;
            float aspect = cam.GetComponent<Camera>().aspect;
            spawnPos.x = Mathf.Clamp(spawnPos.x, -arenaSize.x + camSize * aspect, arenaSize.x - camSize * aspect);
            spawnPos.y = Mathf.Clamp(spawnPos.y, -arenaSize.y + camSize, arenaSize.y - camSize);

            Instantiate(enemy, spawnPos, Quaternion.identity);
            enemyCount++;

            yield return new WaitForSeconds(spawnDelay);
        }

        // clears old wave and progresses
        NewWave();
    }

    private void WavePanelHide()
    {
        WavePanel.SetActive(false);
    }

    private Vector2 spawnPoint()
    {
        var pos = player.GetComponent<Rigidbody2D>().position + UnityEngine.Random.insideUnitCircle * UnityEngine.Random.Range(spawnRadiusMin, spawnRadiusMax);

        // verify
        var hit = Physics2D.CircleCast(pos, 1, Vector2.zero, 1, 9);

        if (hit.collider)
        {
            if (hit.collider.GetComponent<Tilemap>())
            {
                if (tilemap.GetColliderType(tilemap.WorldToCell(hit.transform.position)) != Tile.ColliderType.None)
                {
                    spawnPoint();
                }
            }
        }

        return pos;
    }

    // generates list for next wave
    private void WaveSetup()
    {
        shieldSpawner.SpawnShield();

        //print("Wave Danger " + waveDanger);
        for (int currentDanger = waveDanger; currentDanger > 0; currentDanger--)
        {
            //print("Current danger " + currentDanger);
            int num = UnityEngine.Random.Range(1, maxEnemyDanger + 1);
            //print("Random num " + num);

            if (num <= currentDanger)
            {
                if (num >= TankEnemyBehaviour.DANGER) // spawn tank
                {
                    enemyList.Add(tankEnemy);
                    currentDanger -= TankEnemyBehaviour.DANGER - 1;
                }
                else // spawn melee
                {
                    enemyList.Add(meleeEnemy);
                    //currentDanger -= MeleeEnemyScript.DANGER;
                }
            }
            else // reset because didn't spawn
            {
                currentDanger++;
            }
        }
        //print("Enemy count " + enemyList.Count);
    }

    // sets up for new wave
    private void NewWave()
    {
        enemyList.Clear();

        //WavePanel.GetComponentInChildren<Text>().text = "Wave: " + waveNumber;

        varTrack.wave = waveNumber;

        analytics.IncreaseWaveCount();
        waveNumber++;

        waveDanger = (int)(waveNumber / dangerCurve);
    }
}
