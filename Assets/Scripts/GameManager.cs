using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public LoseWindow loseWindow;

    public LoseWindow pause;
    private void Start()
    {
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Graphics"));
        instance = this;
        StandartSettings();

        System.DateTime currentTime = System.DateTime.UtcNow;
        for (int i = 0; i < 3; i++)
        {
            if (currentTime.Day != PlayerPrefs.GetInt("DayNow" + $"{i}"))
            {
                PlayerPrefs.SetInt("EnemysDay", 0);
                PlayerPrefs.SetInt("MoneyDay", 0);
                PlayerPrefs.SetInt("ScoreDay", 0);
                PlayerPrefs.SetInt("maxWave", 0);


                PlayerPrefs.SetInt("DayNow" + $"{i}", currentTime.Day);
                PlayerPrefs.SetInt("DayQuest" + $"{i}", UnityEngine.Random.Range(1, 5));

            }
        }

    }

    private void OnApplicationQuit()
    {
        // Çáåð³ãàºìî íàëàøòóâàííÿ ãðàâöÿ ïðè âèõîä³ ç ãðè
        PlayerPrefs.Save();
    }
    public void LoseSurvival()
    {
        loseWindow.gameObject.SetActive(true);
        PlayerPrefs.SetInt("AdsInt", PlayerPrefs.GetInt("AdsInt")+1);
        Debug.Log(PlayerPrefs.GetInt("AdsInt"));
        loseWindow.PlaeyrLose();
        Time.timeScale = 0;

        if (PlayerPrefs.GetInt("AdsInt") >= UnityEngine.Random.Range(5, 10))
        {
            PlayerPrefs.SetInt("AdsInt", 0);
            InterstitialAd.S.ShowAd();
        }
        pause.gameObject.SetActive(false);
    }
    public void Pause()
    {
        if (!loseWindow.gameObject.activeSelf)
        {
            pause.gameObject.SetActive(true);
            if(SceneManager.GetActiveScene().buildIndex != 8)
            Time.timeScale = 0;
        }
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        var netman = FindAnyObjectByType<NetworkManager>();
        if (netman.IsClient || netman.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
        }


        // loseWindow.gameObject.SetActive(true);
        loseWindow.PlaeyrLose();

        Time.timeScale = 1;
    }
    public void Continue()
    {
        Time.timeScale = 1;
        pause.gameObject.SetActive(false);
    }
    public void LoadScene(int SceneNumber)
    {
        
        if (SceneNumber == 1)
        {
            loseWindow.PlaeyrLose();
        }
        var netman = FindAnyObjectByType<NetworkManager>();
        if (netman.IsClient || netman.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
        }
        if (SceneNumber == 2) { }
        SceneManager.LoadScene(SceneNumber);
        // loseWindow.gameObject.SetActive(true);

        Time.timeScale = 1;
    }
    public void ScoreReset()
    {
        PlayerPrefs.SetInt("BestScore", 0);

        PlayerPrefs.SetInt("Money", 0);
    }



    private void StandartSettings()
    {

        if (PlayerPrefs.GetInt("DefaultSetting") == default)
        {
            PlayerPrefs.SetString("SelectedLocale", "en");
            //Debug.Log("DefaultSettings:"+ PlayerPrefs.GetInt("DefaultSettings"));
            float[] defaultSpeedHorizontal = { 8f, 10f, 12f, 14f };
            float[] defaultSpeedVertical = { 6f, 8f, 9f, 11f };
            float[] defaultRotationSpeed = { 5f, 6f, 7f, 9f };
            float[] defaultMaxHealth = { 1f, 2f, 3f, 4f };

            for (int i = 0; i < 4; i++)
            {
                PlayerPrefs.SetFloat($"{i}SpeedHorizontal", defaultSpeedHorizontal[i]);
                PlayerPrefs.SetFloat($"{i}SpeedVertical", defaultSpeedVertical[i]);
                PlayerPrefs.SetFloat($"{i}rotationSpeed", defaultRotationSpeed[i]);
                PlayerPrefs.SetFloat($"{i}maxHealth", defaultMaxHealth[i]);
            }

            PlayerPrefs.SetFloat("SpeedHorizontal", PlayerPrefs.GetFloat($"0SpeedHorizontal"));
            PlayerPrefs.SetFloat("SpeedVertical", PlayerPrefs.GetFloat($"0SpeedVertical"));
            PlayerPrefs.SetFloat("rotationSpeed", PlayerPrefs.GetFloat($"0rotationSpeed"));
            PlayerPrefs.SetFloat("maxHealth", PlayerPrefs.GetFloat($"0maxHealth"));

            PlayerPrefs.SetInt("BestScore", 0);


            //////
            PlayerPrefs.SetInt("DefaultSetting", 3);
            //////

            PlayerPrefs.SetInt("ship0Access", 0);
            PlayerPrefs.SetInt("ship1Access", 0);
            PlayerPrefs.SetInt("ship21Access", 0);
            PlayerPrefs.SetInt("ship3Access", 0);
            PlayerPrefs.SetInt("SelectPlayer", 0);

            for (int i = 0; i < 4; i++)
            {
                PlayerPrefs.SetFloat("SpeedHorizontal" + "Update" + $"{i}", 1);
                PlayerPrefs.SetFloat("SpeedVertical" + "Update" + $"{i}", 1);
                PlayerPrefs.SetFloat("rotationSpeed" + "Update" + $"{i}", 1);
                PlayerPrefs.SetFloat("maxHealth" + "Update" + $"{i}", 1);
            }

            PlayerPrefs.SetInt("power" + "BuffBuy", 0);
            PlayerPrefs.SetInt("brush" + "BuffBuy", 0);
            PlayerPrefs.SetInt("sniper" + "BuffBuy", 0);


            ////////////////////////////////////////
            PlayerPrefs.SetInt("Enemys", 0);

            PlayerPrefs.SetInt("EnemysDay", 0);
            PlayerPrefs.SetInt("MoneyDay", 0);
            PlayerPrefs.SetInt("maxWave", 0);
            for (int i = 0; i < 3; i++)
            {

                PlayerPrefs.SetInt("DayNow" + $"{i}", 0);
                PlayerPrefs.SetInt("DayQuest" + $"{i}", 0);


            }
            for (int i = 0; i < 31; i++)
            {

                PlayerPrefs.SetInt("AllQuest" + $"{i}", 0);
                
            }
            for (int i = 0; i < 30; i++)
            {
                PlayerPrefs.SetInt("QuestAll" + $"{i}", 0);
            }
            

            PlayerPrefs.SetInt("Graphics", 2);
            PlayerPrefs.SetInt("SettingAutoRotate", 0);

            PlayerPrefs.SetFloat("ZeroZonePlayer", 0.1f);
            PlayerPrefs.SetFloat("ZeroZoneGun", 0.1f);

            
            PlayerPrefs.SetFloat("volume", 0.5f);
            PlayerPrefs.SetFloat("sounds", 0.5f);
            PlayerPrefs.SetFloat("volumeSl", 0.5f);
            PlayerPrefs.SetFloat("soundsSl", 0.5f);
            //ÍÅ ÇÀÁÓÄÜ ÏÎÒ²Ì ÏÎÑÒÀÂÈÒÈ 0
            PlayerPrefs.SetInt("Money", 100);
            PlayerPrefs.SetInt("MaxMoney", 100);


            PlayerPrefs.SetInt("AdsInt",-10);
        }
    }



    public void ResetShipBuy()
    {
        PlayerPrefs.SetInt("ship0Access", 1);
        PlayerPrefs.SetInt("ship1Access", 0);
        PlayerPrefs.SetInt("ship2Access", 0);
        PlayerPrefs.SetInt("ship3Access", 0);
        PlayerPrefs.SetInt("SelectPlayer", 0);

    }
    public void MamyMoney()
    {
        PlayerPrefs.SetInt("Money", 5000);
        PlayerPrefs.SetInt("EnemysDay", 0);
        for (int i = 0; i < 3; i++)
        {

            PlayerPrefs.SetInt("DayNow" + $"{i}", 0);
            PlayerPrefs.SetInt("DayQuest" + $"{i}", 0);


        }
    }
}
