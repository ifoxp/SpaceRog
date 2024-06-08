
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;


public class Spawner : NetworkBehaviour
{
    [SerializeField] private float TimeToSpawn;
    [SerializeField] private int Rannd, Hard = 0;
    [SerializeField] private GameObject[] PipePrefabs = new GameObject[3];
    [SerializeField] private Transform WhereSpawn;
    [SerializeField] private bool superHard = false;
    [SerializeField] private float Timer = 4, HardTime;
    [SerializeField] private float WhyTimeMinus = 0;
    [SerializeField] private NetworkBuffSpawn spawnBuff;
    [SerializeField] private NetworkObstaclesSpawb spawnObstacles;
    [SerializeField] private bool whoSpawn = true;
    private float hardT, spawnT, TimerHard;

    private void Awake()
    {
        if (!superHard)
        {
            if (PlayerPrefs.GetInt("Difficult") == 0)
                WhyTimeMinus = 0.015f;
            else if (PlayerPrefs.GetInt("Difficult") == 2)
                WhyTimeMinus = 0.018f;
        }

        hardT = HardTime;
        spawnT = Timer;
        TimerHard = TimeToSpawn;
    }

    private void FixedUpdate()
    {
        if (IsHost || SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (hardT < 0 && Hard < PipePrefabs.Length && !superHard)
            {
                Hard = Hard + 3;
                if ((Hard / 3) + 1 > PlayerPrefs.GetInt("maxWave" + $"{PlayerPrefs.GetInt("Difficult")}"))
                {
                    PlayerPrefs.SetInt("maxWave" + $"{PlayerPrefs.GetInt("Difficult")}", (Hard / 3) + 1);
                    PlayerPrefs.SetInt("DayWave" + $"{PlayerPrefs.GetInt("Difficult")}", (Hard / 3) + 1);
                }
                hardT = HardTime;
                TimerHard = TimeToSpawn;

                string sms = "WAVE " + ((Hard / 3) + 1);

                if ((PlayerPrefs.GetString("SelectedLocale") == "uk"))
                    sms = "’¬»Àﬂ " + ((Hard / 3) + 1);
                HintSystem.Instance.ShowHint(sms);
            }

            if (Hard == PipePrefabs.Length)
            {
                superHard = true;
                if (Hard / 3 > PlayerPrefs.GetInt("maxWave" + $"{PlayerPrefs.GetInt("Difficult")}"))
                {
                    PlayerPrefs.SetInt("maxWave" + $"{PlayerPrefs.GetInt("Difficult")}", 5);
                    PlayerPrefs.SetInt("DayWave" + $"{PlayerPrefs.GetInt("Difficult")}", 5);
                }
                string sms = "WAVE 5";
                if ((PlayerPrefs.GetString("SelectedLocale") == "uk"))
                    sms = "’¬»Àﬂ 5";
                HintSystem.Instance.ShowHint(sms);
                Hard = 0;
                TimeToSpawn /= 2;
                TimerHard = TimeToSpawn;
                Rannd = PipePrefabs.Length;
            }

            if (spawnT <= 0)
            {
                spawnT = TimerHard;
                if (!superHard)
                {
                    TimerHard -= WhyTimeMinus;
                    Debug.Log(TimerHard);
                }

                if (SceneManager.GetActiveScene().buildIndex == 8)
                {
                    // Spawn on the server and synchronize to all clients
                    
                    int sp = Random.Range(Hard, Rannd + Hard);
                    if (whoSpawn)
                        spawnBuff.Gun(sp);
                    else
                    {
                        spawnObstacles.Gun(sp, WhereSpawn);
                    }



                }
                else
                {
                    var prefabToInstantiate = PipePrefabs[Random.Range(Hard, Rannd + Hard)];
                    var instance = Instantiate(prefabToInstantiate, WhereSpawn);
                   
                }
            }
            else
            {
                spawnT -= Time.deltaTime;
            }
            hardT -= Time.deltaTime;
        }
        
    }
   


}
