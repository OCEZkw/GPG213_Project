using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialWaveManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialWave
    {
        public int numberOfEnemies;
        public bool isBossWave;
        public GameObject bossPrefab;
    }

    public List<TutorialWave> tutorialWaves;
    public Transform[] spawnPoints;
    public GameObject enemyPrefab;

    private int currentWaveIndex = 0;
    public int enemiesRemainingAlive;

    private int currentEnemyCode = 0;
    private List<GameObject> enemyInstances = new List<GameObject>();

    private List<GameObject> currentWaveEnemies = new List<GameObject>();
    private bool[] spawnPointsOccupied;

    [SerializeField] private TutorialDeckManager tutorialDeckManager;
    [SerializeField] private TutorialManager tutorialManager;

    [SerializeField] private AudioClip footstepsAudio;
    [SerializeField] private AudioClip battleMusic;
    [SerializeField] private AudioClip victoryMusic;
    private AudioSource victoryMusicAudioSource;
    private AudioSource footstepsAudioSource;
    private AudioSource battleMusicAudioSource;

    [SerializeField] private GameObject winPanel;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private float footstepsPauseDuration = 0.5f; // Duration between footstep sounds
    [SerializeField] private float totalFootstepsDuration = 3f;

    [SerializeField] private CameraWalkEffect cameraWalkEffect;

    void Start()
    {
        spawnPointsOccupied = new bool[spawnPoints.Length];
        footstepsAudioSource = gameObject.AddComponent<AudioSource>();
        battleMusicAudioSource = gameObject.AddComponent<AudioSource>();
        victoryMusicAudioSource = gameObject.AddComponent<AudioSource>();
        victoryMusicAudioSource.clip = victoryMusic;
        victoryMusicAudioSource.loop = true;
        victoryMusicAudioSource.playOnAwake = false;
        StartCoroutine(TutorialStartSequence());
    }

    IEnumerator TutorialStartSequence()
    {
        // Play footsteps and wait for them to finish
        yield return StartCoroutine(PlayIntermittentFootsteps());

        yield return new WaitForSeconds(1f);
        InitializeTutorialWaves();
        StartNextWave();

        // Start the tutorial
        tutorialManager.StartTutorial();
        tutorialDeckManager.InitializeDeck();
    }

    IEnumerator PlayIntermittentFootsteps()
    {
        float elapsedTime = 0f;
        int stepCount = 0;

        while (elapsedTime < totalFootstepsDuration)
        {
            footstepsAudioSource.PlayOneShot(footstepsAudio);
            cameraWalkEffect.PlayStepEffect();
            stepCount++;

            yield return new WaitForSeconds(footstepsAudio.length);

            elapsedTime += footstepsAudio.length;

            if (elapsedTime + footstepsPauseDuration < totalFootstepsDuration)
            {
                yield return new WaitForSeconds(footstepsPauseDuration);
                elapsedTime += footstepsPauseDuration;
            }
        }

        Debug.Log($"Played {stepCount} footsteps");
    }

    void InitializeTutorialWaves()
    {
        // Set up tutorial waves here
        // For example:
        tutorialWaves = new List<TutorialWave>
        {
            new TutorialWave { numberOfEnemies = 1, isBossWave = false },
            new TutorialWave { numberOfEnemies = 2, isBossWave = false },
            // Add more waves as needed
        };
    }

    public void StartNextWave()
    {
        if (currentWaveIndex < tutorialWaves.Count)
        {
            currentWaveEnemies.Clear();
            TutorialWave currentWave = tutorialWaves[currentWaveIndex];

            Debug.Log($"Starting Wave {currentWaveIndex + 1}");
            Debug.Log($"Number of enemies to spawn: {currentWave.numberOfEnemies}");

            enemiesRemainingAlive = currentWave.numberOfEnemies;
            SpawnAllEnemiesForWave(currentWave);

            currentWaveIndex++;


        }
        else
        {
            Debug.Log("All waves completed!");
            StartCoroutine(ShowWinPanel());
        }
    }

    IEnumerator ShowWinPanel()
    {
        // Hide all cards
        tutorialDeckManager.HideAllCards();

        // Fade out battle music
        StartCoroutine(FadeOutAudio(battleMusicAudioSource, 1f));

        // Start victory music
        StartCoroutine(FadeInAudio(victoryMusicAudioSource, 1f));

        // Short delay to let music transition start
        yield return new WaitForSeconds(0.5f);

        // Show win panel
        winPanel.SetActive(true);

        // Trigger win text animation
        WinPanelAnimator winPanelAnimator = winPanel.GetComponent<WinPanelAnimator>();
        if (winPanelAnimator != null)
        {
            winPanelAnimator.AnimateText();
        }

        yield return new WaitForSeconds(5f); // Wait for 5 seconds before returning to main menu

        // Fade out victory music
        StartCoroutine(FadeOutAudio(victoryMusicAudioSource, 1f));

        yield return new WaitForSeconds(1f); // Wait for fade out

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private IEnumerator FadeOutAudio(AudioSource audioSource, float fadeDuration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    private IEnumerator FadeInAudio(AudioSource audioSource, float fadeDuration)
    {
        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < 1)
        {
            audioSource.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }
    }

    void SpawnAllEnemiesForWave(TutorialWave wave)
    {
        for (int i = 0; i < wave.numberOfEnemies; i++)
        {
            int spawnPointIndex = i % spawnPoints.Length;
            SpawnEnemy(spawnPoints[spawnPointIndex]);
        }
    }

    void SpawnEnemy(Transform spawnPoint)
    {
        GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        currentWaveEnemies.Add(enemyInstance);

        int uniqueCode = GenerateUniqueEnemyCode();

        Enemy enemy = enemyInstance.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.enemyCode = uniqueCode;
        }

        enemyInstances.Add(enemyInstance);
    }

    int GenerateUniqueEnemyCode()
    {
        return ++currentEnemyCode;
    }

    public void OnEnemyDefeated()
    {
        enemiesRemainingAlive--;
        Debug.Log($"Enemy defeated in tutorial. Remaining alive: {enemiesRemainingAlive}");

        if (enemiesRemainingAlive <= 0)
        {
            Debug.Log("All enemies in the current tutorial wave defeated.");
            if (ShouldStartNextWave())
            {
                StartNextWave();
            }
            else if (currentWaveIndex >= tutorialWaves.Count)
            {
                StartCoroutine(ShowWinPanel());
            }
        }
    }

    public bool ShouldStartNextWave()
    {
        return enemiesRemainingAlive <= 0 && currentWaveIndex < tutorialWaves.Count;
    }

    public List<GameObject> GetEnemyInstances()
    {
        return enemyInstances;
    }

    public void RemoveEnemyInstance(GameObject enemyInstance)
    {
        enemyInstances.Remove(enemyInstance);
        if (enemyInstance != null)
        {
            Destroy(enemyInstance);
        }
    }

    // Additional methods as needed for tutorial-specific functionality
}