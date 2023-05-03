using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkBuilder : MonoBehaviour
{
    public GameObject ObstacleSample;

    public int CountMin, CountMax;
    public float SizeMin, SizeMax;

    Vector3 scale_buffer;
    Vector3 pos_buffer;
    Color color_buffer;

    public GameObject GenerateChunk(GameObject chunkSample, Transform parent, Vector3 position)
    {
        GameObject newChunk = Instantiate(chunkSample, position, Quaternion.identity, parent);
        newChunk.SetActive(true);
        GenerateObstacles(newChunk.transform);
        return newChunk;
    }

    void GenerateObstacles(Transform parentChunk)
    {
        for (int i = 0; i < Random.Range(CountMin, CountMax); i++)
            GenerateObstacle(parentChunk);
    }

    void GenerateObstacle(Transform parentChunk)
    {
        Collider boundRegion = parentChunk.gameObject.GetComponent<Collider>();

        scale_buffer.x = Random.Range(SizeMin, SizeMax);
        scale_buffer.y = Random.Range(SizeMin, SizeMax);
        scale_buffer.z = Random.Range(SizeMin, SizeMax);

        // Scale Spaghetts    >:-|
        scale_buffer.x /= parentChunk.localScale.x;
        scale_buffer.y /= parentChunk.localScale.y;
        scale_buffer.z /= parentChunk.localScale.z;

        pos_buffer.x = Random.Range(boundRegion.bounds.min.x, boundRegion.bounds.max.x);
        pos_buffer.y = Random.Range(boundRegion.bounds.min.y, boundRegion.bounds.max.y);
        pos_buffer.z = Random.Range(boundRegion.bounds.min.z, boundRegion.bounds.max.z);

        color_buffer.r = Random.value;
        color_buffer.g = Random.value;
        color_buffer.b = Random.value;

        GameObject newObstacle = Instantiate(ObstacleSample, pos_buffer, Quaternion.identity, parentChunk);
        newObstacle.transform.localScale = scale_buffer;
        Renderer newObstacleRenderer = newObstacle.GetComponent<Renderer>();
        newObstacleRenderer.material.color = color_buffer;

        newObstacle.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
