using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enemies; // 적 프리팹 배열

    [SerializeField]
    private GameObject[] bossPrefabs; // 보스 프리팹 배열

    [SerializeField]
    private GameObject[] hardModeEnemies; // Hard 모드 적 프리팹 배열

    [SerializeField]
    private GameObject[] hardModeBossPrefabs; // Hard 모드 보스 프리팹 배열

    public bool isHardMode = false; // Hard 모드 여부

    private float[] arrPosX = { -1.8f, -0.9f, 0f, 0.9f, 1.8f };

    [SerializeField]
    private float spawnInterval = 4.0f;
    private float minSpawnInterval = 1.5f;
    private float spawnIntervalDecreaseRate = 0.05f;

    private Coroutine enemyRoutine;

    public int CurrentLevel { get; private set; } = 0;

    public void StartEnemyRoutine()
    {
        if (enemyRoutine != null)
        {
            StopCoroutine(enemyRoutine);
        }

        // 게임 상태가 Playing이 아닌 경우 적 생성 루틴을 시작하지 않음
        if (GameManager.instance.gameState == GameManager.GameState.Playing)
        {
            enemyRoutine = StartCoroutine(EnemyRoutine());
        }
    }

    public void StopEnemyRoutine()
    {
        if (enemyRoutine != null)
        {
            StopCoroutine(enemyRoutine);
            enemyRoutine = null;
        }
    }

    public void ClearEnemies()
    {
        GameObject[] spawnedEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in spawnedEnemies)
        {
            Destroy(enemy);
        }

        GameObject[] spawnedBosses = GameObject.FindGameObjectsWithTag("Boss");
        foreach (GameObject boss in spawnedBosses)
        {
            Destroy(boss);
        }
    }

    public void ResetEnemySpawnerState()
    {
        CurrentLevel = 0; // 스테이지를 초기화
        spawnInterval = 4.0f; // 스폰 간격 초기화
        SetEnemyGroup(CurrentLevel); // 첫 번째 적 그룹 설정
    }

    IEnumerator EnemyRoutine()
    {
        yield return new WaitForSeconds(3f);

        while (true)
        {
            Debug.Log($"Starting Level {CurrentLevel}"); // 현재 레벨을 디버그 로그로 출력

            // 각 적을 1마리부터 5마리까지 점점 늘려가며 5번 반복해서 스폰
            for (int enemyIndex = CurrentLevel * 4; enemyIndex < (CurrentLevel + 1) * 4; enemyIndex++)
            {
                for (int i = 1; i <= 5; i++) // 한 번에 1마리에서 5마리까지 스폰
                {
                    SpawnEnemyWave(enemyIndex % enemies.Length, i);
                    yield return new WaitForSeconds(spawnInterval);
                    spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - spawnIntervalDecreaseRate);
                }
            }

            // 보스 스폰
            SpawnBoss();

            // 보스 처치 대기
            yield return new WaitUntil(() => GameManager.instance.IsBossDefeated());

            // 다음 레벨로 이동
            CurrentLevel++;
            GameManager.instance.SetBossDefeated(false);  // 보스 처치 상태 리셋

            yield return new WaitForSeconds(3f); // 다음 레벨 시작 전 잠시 대기
        }
    }

    void SpawnEnemyWave(int enemyIndex, int count)
    {
        if (GameManager.instance.gameState != GameManager.GameState.Playing) return; // 추가: 게임 상태 체크

        List<int> usedPositions = new List<int>(); // 이미 사용된 위치 인덱스

        for (int i = 0; i < count; i++)
        {
            int posIndex;
            do
            {
                posIndex = Random.Range(0, arrPosX.Length);
            } while (usedPositions.Contains(posIndex));

            usedPositions.Add(posIndex);
            float posX = arrPosX[posIndex];
            SpawnEnemy(posX, enemyIndex);
        }
    }

    void SpawnEnemy(float posX, int index)
    {
        if (GameManager.instance.gameState != GameManager.GameState.Playing) return;

        Vector3 spawnPos = new Vector3(posX, transform.position.y, transform.position.z);

        GameObject[] selectedEnemies = isHardMode ? hardModeEnemies : enemies;

        if (index >= selectedEnemies.Length)
        {
            index = selectedEnemies.Length - 1;
        }

        Instantiate(selectedEnemies[index], spawnPos, Quaternion.identity);
    }

    void SpawnBoss()
    {
        if (GameManager.instance.gameState != GameManager.GameState.Playing) return;

        GameObject[] selectedBosses = isHardMode ? hardModeBossPrefabs : bossPrefabs;

        if (CurrentLevel < selectedBosses.Length)
        {
            Vector3 spawnPos = new Vector3(0f, transform.position.y, transform.position.z);
            GameObject bossObject = Instantiate(selectedBosses[CurrentLevel], spawnPos, Quaternion.identity);
            Debug.Log($"Boss {CurrentLevel} spawned at {spawnPos}");

            Boss bossComponent = bossObject.GetComponent<Boss>();
            if (bossComponent != null)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    bossComponent.player = playerObject.transform;
                }

                bossComponent.bulletPrefab = GameManager.instance.bulletPrefab;
                Transform firePoint = bossObject.transform.Find("FirePoint");
                if (firePoint != null)
                {
                    bossComponent.firePoint = firePoint;
                }

                Debug.Log($"Current boss set to {bossComponent}");
            }
        }
    }

    public void SetEnemyGroup(int level)
    {
        CurrentLevel = level;
    }
}
