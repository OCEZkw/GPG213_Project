using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int numberOfEnemies;
        public bool isBossWave;
        public GameObject bossPrefab;
    }

    public List<Wave> waves;
    public Transform[] spawnPoints;
    public GameObject enemyPrefab;

    private int currentWaveIndex = 0;
    private int enemiesRemainingToSpawn;
    public int enemiesRemainingAlive;

    private int currentEnemyCode = 0;
    private List<GameObject> enemyInstances = new List<GameObject>();

    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private List<GameObject> currentWaveEnemies = new List<GameObject>();
    private bool[] spawnPointsOccupied;

    // New audio-related fields
    [SerializeField] private AudioClip footstepsAudio;
    [SerializeField] private AudioClip battleMusic;
    [SerializeField] private AudioClip victoryMusic;
    private AudioSource victoryMusicAudioSource;
    private AudioSource footstepsAudioSource;
    private AudioSource battleMusicAudioSource;

    // References to other managers
    [SerializeField] private DeckManager deckManager;

    [SerializeField] private float footstepsPauseDuration = 0.5f; // Duration between footstep sounds
    [SerializeField] private float totalFootstepsDuration = 3f;

    [SerializeField] private CameraWalkEffect cameraWalkEffect;

    [SerializeField] private GameObject winPanel;



    void Start()
    {
        spawnPointsOccupied = new bool[spawnPoints.Length];
        // Create two separate AudioSources
        footstepsAudioSource = gameObject.AddComponent<AudioSource>();
        battleMusicAudioSource = gameObject.AddComponent<AudioSource>();
        victoryMusicAudioSource = gameObject.AddComponent<AudioSource>();
        victoryMusicAudioSource.clip = victoryMusic;
        victoryMusicAudioSource.loop = true;
        victoryMusicAudioSource.playOnAwake = false;

        StartCoroutine(GameStartSequence());
    }

    IEnumerator GameStartSequence()
    {
        // Play footsteps and wait for them to finish
        yield return StartCoroutine(PlayIntermittentFootsteps());

        yield return new WaitForSeconds(1f);

        // Initialize the game
        InitializeWavesForLevel();
        // Now start the first wave
        StartNextWave();
        // Start battle music
        battleMusicAudioSource.clip = battleMusic;
        battleMusicAudioSource.loop = true;
        battleMusicAudioSource.Play();

        // Wait a moment before starting the first wave
        yield return new WaitForSeconds(1f);

        deckManager.InitializeDeck();
        // Draw initial hand
        deckManager.DrawHand();
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

    void InitializeWavesForLevel()
    {
        int selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 0);
        Debug.Log("Initializing waves for level: " + selectedLevel);
        // Here you would load or set up the waves data for the selected level
    }

    public void StartNextWave()
    {
        if (currentWaveIndex < waves.Count)
        {
            currentWaveEnemies.Clear();
            Wave currentWave = waves[currentWaveIndex];

            Debug.Log($"Starting Wave {currentWaveIndex + 1}");
            Debug.Log($"Number of enemies to spawn: {currentWave.numberOfEnemies}");

            if (currentWave.isBossWave)
            {
                Debug.Log($"Spawning boss for wave {currentWaveIndex + 1}");
                SpawnBoss(currentWave.bossPrefab);
                enemiesRemainingAlive = 1;
            }
            else
            {
                enemiesRemainingAlive = currentWave.numberOfEnemies;
                SpawnAllEnemiesForWave(currentWave);
            }

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
        deckManager.HideAllCards();

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

    void SpawnAllEnemiesForWave(Wave wave)
    {
        Debug.Log($"Spawning {wave.numberOfEnemies} enemies for wave {currentWaveIndex + 1}");

        for (int i = 0; i < wave.numberOfEnemies; i++)
        {
            int spawnPointIndex = i % spawnPoints.Length;
            SpawnEnemy(spawnPoints[spawnPointIndex]);
        }

        Debug.Log($"Spawned {currentWaveEnemies.Count} enemies. Remaining alive: {enemiesRemainingAlive}");
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

        Debug.Log($"Spawned enemy at {spawnPoint.name}. Total spawned: {currentWaveEnemies.Count}");
    }

    void SpawnBoss(GameObject bossPrefab)
    {
        if (spawnPoints.Length > 0)
        {
            Transform spawnPoint = spawnPoints[0];
            GameObject bossInstance = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
            currentWaveEnemies.Add(bossInstance);

            if (bossInstance.TryGetComponent(out WizardBossEnemy wizardBoss))
            {
                SetupWizardBoss(wizardBoss);
            }
            else if (bossInstance.TryGetComponent(out TripartiteBoss tripartiteBoss))
            {
                tripartiteBoss.InitializeBossParts();
            }

            Debug.Log("Boss spawned");
        }
    }

    void SetupWizardBoss(WizardBossEnemy boss)
    {
        BossPart[] bossParts = boss.GetComponentsInChildren<BossPart>();

        if (bossParts.Length < 3)
        {
            Debug.LogError("Not enough BossPart components found on the WizardBossEnemy prefab.");
            return;
        }

        int staffCode = GenerateUniqueEnemyCode();
        int headCode = GenerateUniqueEnemyCode();
        int leftHandCode = GenerateUniqueEnemyCode();

        boss.AssignUniqueCodes(bossParts[0], bossParts[1], bossParts[2], staffCode, headCode, leftHandCode);
    }

    int GenerateUniqueEnemyCode()
    {
        return ++currentEnemyCode;
    }

    public void OnEnemyDefeated()
    {
        enemiesRemainingAlive--;
        Debug.Log($"Enemy defeated. Remaining alive: {enemiesRemainingAlive}");

        if (enemiesRemainingAlive <= 0)
        {
            Debug.Log("All enemies in the current wave defeated.");
        }
    }

    public bool ShouldStartNextWave()
    {
        return enemiesRemainingAlive <= 0 && currentWaveIndex < waves.Count;
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

    IEnumerator ReturnToMainMenuWithDelay(float delay)
    {
        Debug.Log("All waves completed! Returning to main menu...");
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnFlowerDestroyed(int spawnPointIndex)
    {
        if (spawnPointIndex >= 0 && spawnPointIndex < spawnPointsOccupied.Length)
        {
            spawnPointsOccupied[spawnPointIndex] = false;
            Debug.Log($"WaveManager: Spawn point {spawnPointIndex} is now available.");
        }
        else
        {
            Debug.LogWarning($"WaveManager: Invalid spawn point index: {spawnPointIndex}");
        }
        OnEnemyDefeated();
    }

    public int GetAvailableSpawnPoint()
    {
        for (int i = 1; i <= 2; i++)  // Check only spawn points 1 and 2 for flowers
        {
            if (!spawnPointsOccupied[i])
            {
                return i;
            }
        }
        return -1;  // No available spawn point
    }

    public void SetSpawnPointOccupied(int index, bool occupied)
    {
        if (index >= 0 && index < spawnPointsOccupied.Length)
        {
            spawnPointsOccupied[index] = occupied;
        }
        else
        {
            Debug.LogWarning($"WaveManager: Attempted to set invalid spawn point index: {index}");
        }
    }
}