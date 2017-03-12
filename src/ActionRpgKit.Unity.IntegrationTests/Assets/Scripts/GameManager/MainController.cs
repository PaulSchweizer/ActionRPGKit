﻿using ActionRpgKit.Character;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Control the switching between scenes and the loading and saving of game states.</summary>
public class MainController : MonoBehaviour
{
    /// <summary>
    /// To keep the Controller a Singleton</summary>
    public static MainController Instance;

    /// <summary>Leverl
    /// The name of the starting scene for the Game.</summary>
    public string StartScene;
    public string MainMenuScene;
    public string GameOverScene;
    public GameObject PlayerPrefab;
    public GameObject GameMenuPrefab;
    public GameObject StoryViewerPrefab;
    public GameObject CameraRigPrefab;

    public AudioClip MainMenuMusic;
    public AudioClip IntroMusic;
    public AudioClip IntroText;
    public AudioClip GhostCinematicText;
    public AudioClip EndMusic;
    public AudioClip EndText;

    /// <summary>
    /// The list of actual Game Scenes as opposed to other scenes that do not contain GamePlay.</summary>
    public string[] GameScenes;

    // Story
    public UStoryline Storyline;

    // Fade out
    public Canvas FadingCanvas;
    public Image FadingScreen;
    public float FadingSpeed = 3;
    public Text LoadingText;

    // From the Tutorial
    public string CurrentSceneName;
    public string NextSceneName;
    private AsyncOperation resourceUnloadTask;
    private AsyncOperation sceneLoadTask;
    private enum SceneState { FadeOut, Reset, Preload, Load, Unload, Postload, Ready, Run, FadeIn, Count };
    private SceneState sceneState;
    private delegate void UpdateDelegate();
    private UpdateDelegate[] updateDelegates;
    private Player _playerDataToLoad;

    /// <summary>
    /// Handler operates after the scene is ready.</summary>
    public delegate void SceneReadyHandler();
    public event SceneReadyHandler OnSceneReady;

    /// <summary>
    /// Switch the scene to the given scene name.</summary>
    /// <param name="nextSceneName"></param>
    public static void SwitchScene(string nextSceneName)
    {
        if (Instance != null)
        {
            if (Instance.CurrentSceneName != nextSceneName)
            {
                Instance.NextSceneName = nextSceneName;
                Instance.LoadingText.text = string.Format("{0}", nextSceneName); 
            }
        }
    }

    /// <summary>
    /// Keep the Controller a Singleton.</summary>
    protected void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }

        updateDelegates = new UpdateDelegate[(int)SceneState.Count];
        updateDelegates[(int)SceneState.FadeOut] = UpdateSceneFadeOut;
        updateDelegates[(int)SceneState.Reset] = UpdateSceneReset;
        updateDelegates[(int)SceneState.Preload] = UpdateScenePreload;
        updateDelegates[(int)SceneState.Load] = UpdateSceneLoad;
        updateDelegates[(int)SceneState.Unload] = UpdateSceneUnload;
        updateDelegates[(int)SceneState.Postload] = UpdateScenePostload;
        updateDelegates[(int)SceneState.Ready] = UpdateSceneReady;
        updateDelegates[(int)SceneState.FadeIn] = UpdateSceneFadeIn;
        updateDelegates[(int)SceneState.Run] = UpdateSceneRun;

        sceneState = SceneState.Ready;
    }

    /// <summary>
    /// Delete the delegates and the mainController statics.</summary>
    protected void OnDestroy()
    {
        if (updateDelegates != null)
        {
            for (int i = 0; i < (int)SceneState.Count; i++)
            {
                updateDelegates[i] = null;
            }
            updateDelegates = null;
        }
    }

    #region SceneManagement

    /// <summary>
    /// Update the current Delegate.</summary>
    protected void Update()
    {
        if (updateDelegates[(int)sceneState] != null)
        {
            updateDelegates[(int)sceneState]();
        }
    }

    /// <summary>
    /// Fade out the level.</summary>
    private void UpdateSceneFadeOut()
    {
        if (FadingScreen.color.a < 1)
        {
            FadingCanvas.sortingOrder = 999;
            FadingCanvas.enabled = true;
            FadingScreen.color = new Color(0, 0, 0, FadingScreen.color.a + Time.deltaTime * FadingSpeed);
        }
        else
        {
            AudioControl.Instance.StopText();
            LoadingText.enabled = true;
            sceneState = SceneState.Reset;
        }
    }

    /// <summary>
    /// Attach the new scene controller to start cascade of loading.</summary>
    private void UpdateSceneReset()
    {
        UStoryline.Instance.PauseStory();

        ActionRpgKitController.Instance.enabled = false;

        // Disable the Player
        if (GamePlayer.Instance != null)
        {
            GamePlayer.Instance.NavMeshAgent.enabled = false;
            GamePlayer.Instance.enabled = false;
        }

        GC.Collect();
        sceneState = SceneState.Preload;
    }

    /// <summary>
    /// Handle anything that needs to happen before loading.</summary>
    private void UpdateScenePreload()
    {
        sceneLoadTask = SceneManager.LoadSceneAsync(NextSceneName);
        sceneState = SceneState.Load;
    }

    /// <summary>
    /// Show the loading screen until it's loaded.</summary>
    private void UpdateSceneLoad()
    {
        if (sceneLoadTask.isDone)
        {
            sceneState = SceneState.Unload;
        }
        else
        {
            // Update some scene loading progress bar
        }
    }

    /// <summary>
    /// Clean up unused resources by unloading them.</summary>
    private void UpdateSceneUnload()
    {
        if (resourceUnloadTask == null)
        {
            // Reset the ActionRpgKit Controller
            ActionRpgKitController.Instance.Reset();
            resourceUnloadTask = Resources.UnloadUnusedAssets();
        }
        else
        {
            if (resourceUnloadTask.isDone)
            {
                resourceUnloadTask = null;
                sceneState = SceneState.Postload;
            }
        }
    }

    /// <summary>
    /// Handle anything that needs to happen immediately after loading.</summary>
    private void UpdateScenePostload()
    {
        CurrentSceneName = NextSceneName;
        sceneState = SceneState.Ready;
    }

    /// <summary>
    /// Handle anything that needs to happen immediately before running.
    /// </summary>
    private void UpdateSceneReady()
    {
        ActionRpgKitController.Instance.enabled = false;

        // Init the Player and other game objects
        if (Array.Exists(GameScenes, element => element == NextSceneName))
        {
            ActionRpgKitController.Instance.enabled = true;

            // Instantiate the Player
            if (GamePlayer.Instance == null)
            {
                Instantiate(PlayerPrefab);
            }

            // Set the main Audio Source
            //var audioSource = GamePlayer.Instance.gameObject.GetComponentInChildren<AudioSource>();
            //AudioControl.Instance.mainAudioSource = audioSource;

            // If there is Player data to load, load it and reset it to null
            if (_playerDataToLoad != null)
            {
                var playerLive = (Player)GamePlayer.Instance.Character;
                playerLive.InitFromPlayer(_playerDataToLoad);
                GamePlayer.Instance.transform.position = 
                    new Vector3(_playerDataToLoad.Position.X, 0, _playerDataToLoad.Position.Y);
                _playerDataToLoad = null;
            }
            else
            {
                // Find a spawn point in the level and put the player there
                foreach(SavePoint savePoint in FindObjectsOfType<SavePoint>())
                {
                    if (savePoint.IsSpawnPoint)
                    {
                        GamePlayer.Instance.NavMeshAgent.enabled = false;
                        GamePlayer.Instance.transform.position = savePoint.transform.position;
                        GamePlayer.Instance.NavMeshAgent.enabled = true;
                        break;
                    }
                }
            }

            // Instantiate the PlayerMenu 
            if (GameMenu.Instance == null)
            {
                Instantiate(GameMenuPrefab);
                GameMenu.Instance.SwitchToGame();
            }

            // Instantiate the StoryViewer 
            if (StoryViewer.Instance == null)
            {
                Instantiate(StoryViewerPrefab);
                StoryViewer.Instance.Hide();
            }

            // Instantiate the CameraRig
            if (CameraRig.Instance == null)
            {
                // Get all cameras in the scene and disable them
                foreach(Camera camera in FindObjectsOfType<Camera>())
                {
                    camera.gameObject.SetActive(false);
                }

                // Get all audio listeners in the scene and disable them
                foreach (AudioListener audio in FindObjectsOfType<AudioListener>())
                {
                    audio.gameObject.SetActive(false);
                }

                Instantiate(CameraRigPrefab);
                CameraRig.Instance.Target = GamePlayer.Instance.transform;
                CameraRig.Instance.Update();
            }
            else
            {
                CameraRig.Instance.Target = GamePlayer.Instance.transform;
                CameraRig.Instance.Update();
            }

            foreach (Camera camera in FindObjectsOfType<Camera>())
            {
                if (camera != CameraRig.Instance.Camera)
                {
                    camera.gameObject.SetActive(false);
                }
                else
                {
                    camera.gameObject.SetActive(true);
                }
            }

            // Initialize the ActionRpgKit Controller
            ActionRpgKitController.Instance.Initialize();

            // Resume the Storyline
            Storyline.ResumeStory();

            // Disable the Player
            if (GamePlayer.Instance != null)
            {
                GamePlayer.Instance.NavMeshAgent.enabled = true;
                GamePlayer.Instance.enabled = true;
            }

        }
        else
        {
            Storyline.PauseStory();
        }

        if (CurrentSceneName == "Main Menu" || CurrentSceneName == "")
        {
            AudioControl.Instance.PlaySound(MainMenuMusic);
        }

        // The Game is now ready to run
        sceneState = SceneState.FadeIn;
    }

    /// <summary>
    /// Fade out the level.</summary>
    private void UpdateSceneFadeIn()
    {
        LoadingText.enabled = false;
        if (FadingScreen.color.a > 0)
        {
            FadingScreen.color = new Color(0, 0, 0, FadingScreen.color.a - Time.deltaTime * FadingSpeed);
        }
        else
        {
            FadingCanvas.sortingOrder = 0;
            FadingCanvas.enabled = false;
            sceneState = SceneState.Run;

            var handler = OnSceneReady;
            if (handler != null)
            {
                handler();
            }

            // Some hacky stuff
            if (CurrentSceneName == "The Swamp")
            {
                UStoryline.Instance.Quests[3].Start();
            }
            else if (CurrentSceneName == "The Lair of the Necromancer")
            {
                UStoryline.Instance.Quests[4].Start();
            }
            else if (CurrentSceneName == "Main Menu")
            {
                AudioControl.Instance.PlaySound(MainMenuMusic);
            }
            else if (CurrentSceneName == "Shadowhunter")
            {
                AudioControl.Instance.PlaySound(IntroMusic);
                AudioControl.Instance.PlayText(IntroText);
            }
            else if (CurrentSceneName == "Asking a Ghost")
            {
                AudioControl.Instance.PlayText(GhostCinematicText);
            }
            else if (CurrentSceneName == "The End")
            {
                GamePlayer.Instance.enabled = false;
                Destroy(GamePlayer.Instance.gameObject);
                Destroy(GameMenu.Instance.gameObject);
                AudioControl.Instance.PlaySound(EndMusic);
                AudioControl.Instance.PlayText(EndText);
            }
        }
    }

    /// <summary>
    /// Wait for scene change.</summary>
    private void UpdateSceneRun()
    {
        if (CurrentSceneName != NextSceneName)
        {
            sceneState = SceneState.FadeOut;
        }
    }

    #endregion

    #region Actions

    /// <summary>
    /// Start a new game by switching to the starting scene.</summary>
    public void StartNewGame()
    {
        SwitchScene(StartScene);
        LoadingText.text = "Starting New Game";
        OnSceneReady += StartStory;
    }

    public void StartStory()
    {
        if (CurrentSceneName == "Nightly Streets")
        {
            Storyline.StartStory();
            OnSceneReady -= StartStory;
        }
    }

    /// <summary>
    /// Save the GameState.
    /// Create a inbetween player object for serialization.</summary>
    public void SaveGameState()
    {
        // The Player
        var player = (Player)GamePlayer.Instance.Character;
        var playerForSerialization = new Player(player);
        SaveData("Player", playerForSerialization);

        // Save the Quest progress
        foreach(UQuest quest in Storyline.Quests)
        {
            SaveJsonData(quest.Name, quest);
        }

        SaveJsonData("Player", GamePlayer.Instance.playerData);

        // The current Scene
        SaveData("CurrentScene", CurrentSceneName);
    }

    /// <summary>
    /// Load the Game Progress from a saved location.</summary>
    public void LoadGameState()
    {
        // Load the Player data
        _playerDataToLoad = (Player)LoadData("Player");

        // Load the scene
        var scene = (string)LoadData("CurrentScene");

        // Load the story progress
        foreach (UQuest quest in Storyline.Quests)
        {
            string json = LoadJsonData(quest.Name);
            JsonUtility.FromJsonOverwrite(json, quest);
        }
        LoadingText.text = "Loading...";
        SwitchScene(scene);
    }

    /// <summary>
    /// Act when the Player has been defeated.</summary>
    public void GameOver()
    {
        InputController.Instance.enabled = false;
        ActionRpgKitController.Instance.enabled = false;
        StartCoroutine(FadeInGameOverMenu());
    }

    private IEnumerator FadeInGameOverMenu()
    {
        FadingSpeed = 0.75f;
        float endTime = Time.time + 1 / FadingSpeed + 1;
        LoadingText.text = "Game Over";
        while (Time.time < endTime)
        {
            UpdateSceneFadeOut();
            yield return null;
        }
        FadingSpeed = 3;

        GamePlayer.Instance.enabled = false;
        Destroy(GamePlayer.Instance.gameObject);
        Destroy(GameMenu.Instance.gameObject);
        // Destroy(AudioControl.Instance.gameObject);
        // Destroy(StoryViewer.Instance.gameObject);
        //Destroy(CameraRig.Instance.gameObject);

        SwitchScene(GameOverScene);
    }

    /// <summary>
    /// Save the given object under the given file name.</summary>
    public static void SaveData(string fileName, object data)
    {
        string dataFile = String.Format("{0}/{1}.dat", GameStatesDirectory, fileName);
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(dataFile,
                                       FileMode.OpenOrCreate,
                                       FileAccess.Write,
                                       FileShare.None);
        formatter.Serialize(stream, data);
        stream.Close();
        Debug.Log("Saved: " + dataFile);
    }

    public static void SaveJsonData(string fileName, object data)
    {
        string dataFile = String.Format("{0}/{1}.json", GameStatesDirectory, fileName);
        var json = JsonUtility.ToJson(data, true);
        System.IO.File.WriteAllText(@dataFile, json);
    }

    /// <summary>
    /// Create a location for the Save Game.</summary>
    public static string GameStatesDirectory
    {
        get
        {
            var directory = String.Format("{0}/GameStates", Application.persistentDataPath);
            Directory.CreateDirectory(directory);
            return directory;
        }
    }

    /// <summary>
    /// Load the given object stored at the given file name.</summary>
    public static object LoadData(string fileName)
    {
        string dataFile = String.Format("{0}/{1}.dat", GameStatesDirectory, fileName);
        if (File.Exists(dataFile))
        {
            IFormatter formatter = new BinaryFormatter();
            var stream = new FileStream(dataFile,
                                        FileMode.Open,
                                        FileAccess.Read,
                                        FileShare.Read);
            var data = (object)formatter.Deserialize(stream);
            stream.Close();
            return data;
        }
        return null;
    }

    public static string LoadJsonData(string fileName)
    {
        string dataFile = String.Format("{0}/{1}.json", GameStatesDirectory, fileName);
        if (File.Exists(dataFile))
        {
            return System.IO.File.ReadAllText(@dataFile);
        }
        return null;
    }

    #endregion

}
