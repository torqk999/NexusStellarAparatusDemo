using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardinalDirection
{
    Up,
    Down,
    Left,
    Right,
    Forward,
    Backward,
}

public class GridSpaceManager : MonoBehaviour
{
    //public BoundsHandlerDemo myBoundsHandler;
    public ChunkBuilder myChunkBuilder;
    public GameObject ChunkSample;
    public Transform FrameOfRefrence;

    Collider ChunkCollider => ChunkSample.GetComponent<Collider>();
    //public Collider BoundingBox; //=> REAL[1, 1, 1].GetComponent<Collider>();

    GameObject[,,] REAL = new GameObject[3,3,3];
    GameObject[,,] BUFFER = new GameObject[3,3,3];
    Vector3 position_buffer;
    Vector3 chunk_size_cache;
    Vector3 extents_cache;

    public float border = 1f;

    public void ShiftRegions(CardinalDirection dir)
    {
        switch(dir)
        {
            case CardinalDirection.Left:
                ShiftRegions(1, 0, 0);
                break;

            case CardinalDirection.Right:
                ShiftRegions(-1, 0, 0);
                break;


            case CardinalDirection.Up:
                ShiftRegions(0, -1, 0);
                break;

            case CardinalDirection.Down:
                ShiftRegions(0, 1, 0);
                break;


            case CardinalDirection.Backward:
                ShiftRegions(0, 0, 1);
                break;

            case CardinalDirection.Forward:
                ShiftRegions(0, 0, -1);
                break;
        }
    }

    /*void ReParentFrameOfReference(int deltaX, int deltaY, int deltaZ)
    {
        FrameOfRefrence.parent = REAL[]
    }*/

    void ShiftRegions(int deltaX, int deltaY, int deltaZ)
    {
        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 3; y++)
                for (int z = 0; z < 3; z++)
                {
                    //GameObject nextRegion = REAL[x, y, z];
                    //Collider nextBounds = nextRegion.GetComponent<Collider>();

                    // Loop coord map
                    int newX = deltaX + x >= 3 ? 0 : deltaX + x < 0 ? 2 : deltaX + x;
                    int newY = deltaY + y >= 3 ? 0 : deltaY + y < 0 ? 2 : deltaY + y;
                    int newZ = deltaZ + z >= 3 ? 0 : deltaZ + z < 0 ? 2 : deltaZ + z;

                    // Re-activate children on roll-over
                    if (deltaX + x >= 3 || deltaX + x < 0 ||
                        deltaY + y >= 3 || deltaY + y < 0 ||
                        deltaZ + z >= 3 || deltaZ + z < 0)
                    {
                        for (int i = 0; i < REAL[x, y, z].transform.childCount; i++) // Holy fuck optimize this please you ape
                            REAL[x, y, z].transform.GetChild(i).gameObject.SetActive(true);
                    }

                    // Push to buffer
                    BUFFER[newX, newY, newZ] = REAL[x, y, z];

                    // Reposition in real space
                    UpdatePositionBuffer(newX, newY, newZ);
                    //position_buffer.x = (newX - 1) * nextBounds.bounds.size.x;
                    //position_buffer.y = (newY - 1) * nextBounds.bounds.size.y;
                    //position_buffer.z = (newZ - 1) * nextBounds.bounds.size.z;

                    BUFFER[newX, newY, newZ].transform.SetPositionAndRotation(position_buffer, Quaternion.identity);
                }

        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 3; y++)
                for (int z = 0; z < 3; z++)
                    REAL[x, y, z] = BUFFER[x, y, z];
                

                    //REAL = BUFFER;
        //BoundingBox = REAL[1, 1, 1].GetComponent<Collider>();
    }

    void UpdatePositionBuffer(int xIndice, int yIndice, int zIndice)
    {
        position_buffer.x = (xIndice - 1) * chunk_size_cache.x;
        position_buffer.y = (yIndice - 1) * chunk_size_cache.y;
        position_buffer.z = (zIndice - 1) * chunk_size_cache.z;
    }

    // Start is called before the first frame update
    void Start()
    {
        chunk_size_cache = ChunkCollider.bounds.size;
        extents_cache = chunk_size_cache / 2;
        ChunkSample.SetActive(false);

        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 3; y++)
                for (int z = 0; z < 3; z++)
                {
                    UpdatePositionBuffer(x, y, z);
                    REAL[x, y, z] = myChunkBuilder.GenerateChunk(ChunkSample, transform, position_buffer);
                }

        //BoundingBox = REAL[1, 1, 1].GetComponent<Collider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.H))
            ShiftRegions(CardinalDirection.Left);

        if (Input.GetKeyDown(KeyCode.K))
            ShiftRegions(CardinalDirection.Right);

        #region Pain...

        if (FrameOfRefrence.position.x < -extents_cache.x)
        {
            FrameOfRefrence.Translate(chunk_size_cache.x, 0, 0, Space.World);
            ShiftRegions(CardinalDirection.Left);
        }

        if (FrameOfRefrence.position.y < -extents_cache.y)
        {
            FrameOfRefrence.Translate(0, chunk_size_cache.y, 0, Space.World);
            ShiftRegions(CardinalDirection.Down);
        }

        if (FrameOfRefrence.position.z < -extents_cache.z)
        {
            FrameOfRefrence.Translate(0, 0, chunk_size_cache.z, Space.World);
            ShiftRegions(CardinalDirection.Backward);
        }

        if (FrameOfRefrence.position.x > extents_cache.x)
        {
            FrameOfRefrence.Translate(-chunk_size_cache.x, 0, 0, Space.World);
            ShiftRegions(CardinalDirection.Right);
        }

        if (FrameOfRefrence.position.y > extents_cache.y)
        {
            FrameOfRefrence.Translate(0, -chunk_size_cache.y, 0, Space.World);
            ShiftRegions(CardinalDirection.Up);
        }

        if (FrameOfRefrence.position.z > extents_cache.z)
        {
            FrameOfRefrence.Translate(0, 0, -chunk_size_cache.z, Space.World);
            ShiftRegions(CardinalDirection.Forward);
        }

        #endregion
    }
}
