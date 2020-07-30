using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class LevelAnalytics : MonoBehaviour
{
    public enum LevelPlayState { InProgress, Over, Quit}

    private Scene thisScene;
    private LevelPlayState state = LevelPlayState.InProgress;

    private float secondsElapsed = 0;
    private int waveCount = 0;

    private int meleeSpawnCount = 0;
    private int tankSpawnCount = 0;

    private int meleeKillCount = 0;
    private int tankKillCount = 0;

    void Awake()
    {
        thisScene = SceneManager.GetActiveScene();
        AnalyticsEvent.LevelStart(thisScene.name, thisScene.buildIndex);
    }

    public void SetLevelPlayerState(LevelPlayState newState)
    {
        this.state = newState;
    }

    public void IncreaseWaveCount()
    {
        waveCount++;
    }

    public void IncreaseMeleeSpawnCount()
    {
        meleeSpawnCount++;
    }

    public void IncreaseTankSpawnCount()
    {
        tankSpawnCount++;
    }

    public void IncreaseMeleeKillCount()
    {
        meleeKillCount++;
    }

    public void IncreaseTankKillCount()
    {
        tankKillCount++;
    }
    
    void Update()
    {
        secondsElapsed += Time.deltaTime;   
    }

    void OnDestroy()
    {
        Dictionary<string, object> customParams = new Dictionary<string, object>();
        customParams.Add("seconds_played", secondsElapsed);
        customParams.Add("wave_reached", waveCount);
        customParams.Add("melee_spawned", meleeSpawnCount);
        customParams.Add("tank_spawned", tankSpawnCount);
        customParams.Add("melee_killed", meleeKillCount);
        customParams.Add("tank_killed", tankKillCount);

        switch(this.state)
        {
            case LevelPlayState.Over:
                AnalyticsEvent.LevelComplete(thisScene.name, thisScene.buildIndex, customParams);
                break;

            case LevelPlayState.InProgress:
            case LevelPlayState.Quit:
            default:
                AnalyticsEvent.LevelQuit(thisScene.name, thisScene.buildIndex, customParams);
                break;
        }
    }
}
