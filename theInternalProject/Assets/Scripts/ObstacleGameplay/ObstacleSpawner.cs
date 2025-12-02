using UnityEngine;
using System.Collections.Generic;


public class ObstacleSpawner : MonoBehaviour
{

    [Header("General Settings")]
    public GameObject infectionPrefab;
    public float spawnPos = 14f;             // pos where formations appear
    public float minSpawnDelay = 2f;
    public float maxSpawnDelay = 3f;
    public float obstacleMoveSpeed = 5f;

    [Header("Tunnel Settings")]
    public GameObject tunnelPrefab;
    public float tunnelY = 0f;               // vertical position of the tunnel
    public float tunnelStopX = -2f;          // where it stops on screen
    public float tunnelSpawnDelayAfterEnd = 1.5f; // delay after last obstacle

    [Header("Spawn Timer")]
    public float spawnDuration = 40f;        // how long obstacles should spawn
    private float elapsedTime = 0f;
    public bool stopWhenTimeIsUp = true;

    [Header("Speed Increase")]
    public float speedIncreaseDelay = 10f;   // time before speed up starts
    public float speedIncreaseRate = 0.5f;   // speed added every second (after delay)
    public float maxSpeed = 12f;


    //  ------ INFECTION FORMATIONS ------
    [Header("Pipe Settings")]
    public float pipeGapHeight = 2f;      // size of hole player can pass through
    public float pipeYMin = -1f;          // min center of the gap
    public float pipeYMax = 3f;           // max center of the gap
    public int pipeExtraSegments = 6;     // how tall the pipe can extend above/below

    [Header("Multi-Gap Tube Settings")]
    public int tubeSegments = 7;          // how many infection sprites stacked from bottom to top
    public float tubeBottomY = -5f;        // Y of the bottom segment
    public int minGapCount = 2;            // minimum number of gaps (holes)
    public int maxGapCount = 3;            // maximum number of gaps
    public int gapSizeInSegments = 1;      // how tall each gap is (in sprite slots)

    [Header("ZigZag Settings")]
    public int zigZagCount = 9;
    public float zigZagVerticalStep = 1.5f;

    float _timeToNextSpawn;

    void Start()
    {
        ScheduleNextSpawn();
    }

    void Update()
    {
        // timer
        elapsedTime += Time.deltaTime;

        // stop spawning when time is up
        if (stopWhenTimeIsUp && elapsedTime >= spawnDuration)
            return;

        // speed increase
        if (elapsedTime >= speedIncreaseDelay)
        {
            obstacleMoveSpeed += speedIncreaseRate * Time.deltaTime;
            obstacleMoveSpeed = Mathf.Min(obstacleMoveSpeed, maxSpeed);
        }

        // spawn logic
        _timeToNextSpawn -= Time.deltaTime;
        if (_timeToNextSpawn <= 0f)
        {
            SpawnRandomFormation();
            ScheduleNextSpawn();
        }
    }

    void ScheduleNextSpawn()
    {
        _timeToNextSpawn = Random.Range(minSpawnDelay, maxSpawnDelay);
    }

    void SpawnRandomFormation()
    {
        int type = Random.Range(0, 3); // 0,1,2

        switch (type)
        {
            case 0:
                SpawnPipeFormation();
                break;
            case 1:
                SpawnMultiGapWall();
                break;
            case 2:
                SpawnZigZagFormation();
                break;
        }
    }

    // --------  Helper: create parent + add Scroller  --------
    GameObject CreateGroup(string name)
    {
        GameObject group = new GameObject(name);
        group.transform.position = new Vector3(spawnPos, 0f, 0f);

        Scroller scroller = group.AddComponent<Scroller>();
        scroller.moveSpeed = obstacleMoveSpeed;

        return group;
    }

    void SpawnPipeFormation()
    {
        if (infectionPrefab == null) return;

        GameObject group = CreateGroup("PipeFormation");

        float spriteHeight = infectionPrefab.GetComponent<SpriteRenderer>().bounds.size.y;

        // where the gap is centered
        float gapCenterY = Random.Range(pipeYMin, pipeYMax);
        float halfGap = pipeGapHeight / 2f;

        // TOP pipe (above gap)
        float topStartY = gapCenterY + halfGap + spriteHeight * 0.5f;
        for (int i = 0; i < pipeExtraSegments; i++)
        {
            float y = topStartY + i * spriteHeight;
            CreateInfection(new Vector3(spawnPos, y, 0f), group.transform);
        }

        // BOTTOM pipe (below gap)
        float bottomStartY = gapCenterY - halfGap - spriteHeight * 0.5f;
        for (int i = 0; i < pipeExtraSegments; i++)
        {
            float y = bottomStartY - i * spriteHeight;
            CreateInfection(new Vector3(spawnPos, y, 0f), group.transform);
        }
    }


    void SpawnMultiGapWall()
    {
        if (infectionPrefab == null) return;

        GameObject group = CreateGroup("MultiGapTube");

        // use the sprite's height to stack them cleanly
        float spriteHeight = infectionPrefab.GetComponent<SpriteRenderer>().bounds.size.y;

        int segments = Mathf.Max(1, tubeSegments);
        bool[] isGap = new bool[segments];

        // how many gaps (holes) this tube will have
        int gapsToPlace = Mathf.Clamp(Random.Range(minGapCount, maxGapCount + 1), 1, segments);

        int safety = 0;
        int maxAttempts = 50;

        while (gapsToPlace > 0 && safety < maxAttempts)
        {
            safety++;

            // choose a random segment index where the gap starts
            int startIndex = Random.Range(0, segments - gapSizeInSegments);

            // check if this range already overlaps another gap
            bool overlap = false;
            for (int i = startIndex; i < startIndex + gapSizeInSegments; i++)
            {
                if (isGap[i])
                {
                    overlap = true;
                    break;
                }
            }

            if (overlap) continue;

            // mark this range as a gap
            for (int i = startIndex; i < startIndex + gapSizeInSegments; i++)
            {
                isGap[i] = true;
            }

            gapsToPlace--;
        }

        // now build the tube: fill all non-gap segments with infections
        for (int i = 0; i < segments; i++)
        {
            if (isGap[i]) continue; // leave the gap empty

            float y = tubeBottomY + i * spriteHeight;
            Vector3 pos = new Vector3(spawnPos, y, 0f);

            CreateInfection(pos, group.transform);
        }
    }



    void SpawnZigZagFormation()
    {
        if (infectionPrefab == null) return;

        GameObject group = CreateGroup("ZigZagFormation");

        float direction = Random.value > 0.5f ? 1f : -1f; // up or down
        float startY = Random.Range(-2f, 2f);

        for (int i = 0; i < zigZagCount; i++)
        {
            float xOffset = -i * 0.6f; // slightly spread backwards
            float y = startY + direction * i * zigZagVerticalStep;

            Vector3 pos = new Vector3(spawnPos + xOffset, y, 0f);
            CreateInfection(pos, group.transform);
        }
    }

    void CreateInfection(Vector3 position, Transform parent)
    {
        Instantiate(infectionPrefab, position, Quaternion.identity, parent);
    }
}
