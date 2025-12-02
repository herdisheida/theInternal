using UnityEngine;
using System.Collections.Generic;


public class ObstacleSpawner : MonoBehaviour
{
    [Header("General Settings")]
    public GameObject infectionPrefab;
    public float spawnPos = 12f;          // pos where formations appear
    public float minSpawnDelay = 1.5f;
    public float maxSpawnDelay = 3f;
    public float obstacleMoveSpeed = 5f;

    [Header("Pipe Settings")]
    public float pipeGapHeight = 2f;      // size of hole player can pass through
    public float pipeYMin = -1f;          // min center of the gap
    public float pipeYMax = 3f;           // max center of the gap
    public int pipeExtraSegments = 6;     // how tall the pipe can extend above/below

    [Header("Random Scatter Settings")]
    public int scatterMinCount = 4;
    public int scatterMaxCount = 10;
    
    public float scatterAreaHeight = 7f;   // vertical size of scatter
    public float scatterYCenter = 0f;      // middle of scatter area

    public float scatterLength = 12f;      // how far to spread along x-axis
    public float scatterMinSpacing = 1.0f; // minimum distance between infections


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
                SpawnScatterFormation();
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

    void SpawnScatterFormation()
    {
        if (infectionPrefab == null) return;

        GameObject group = CreateGroup("ScatterFormation");

        int count = Random.Range(scatterMinCount, scatterMaxCount + 1);

        List<Vector2> usedPositions = new List<Vector2>();

        int safety = 0;
        int maxTries = 200;  // to avoid infinite loops

        while (usedPositions.Count < count && safety < maxTries)
        {
            safety++;

            // spread along X from spawnPos backwards (to the left)
            float x = spawnPos - Random.Range(0f, scatterLength);

            // random vertical position in the area
            float y = scatterYCenter + Random.Range(-scatterAreaHeight / 2f, scatterAreaHeight / 2f);

            Vector2 candidate = new Vector2(x, y);

            // check distance to all previously placed infections
            bool tooClose = false;
            foreach (Vector2 pos in usedPositions)
            {
                if (Vector2.Distance(pos, candidate) < scatterMinSpacing)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose) continue; // try another point

            // accept this position
            usedPositions.Add(candidate);
            CreateInfection(new Vector3(candidate.x, candidate.y, 0f), group.transform);
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
