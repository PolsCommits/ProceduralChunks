using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralBlockMap : MonoBehaviour
{
    // List of generated chunks
    List<GameObject> chunks = new List<GameObject>();
    // Reference to closest chunk
    public GameObject currentChunk;
    // Player reference
    public PlayerController player;
    // Number of chunks to render around player
    public int renderDistance = 3;
    // Size of spawn area
    public int spawnAreaSize = 3;
    // Chunk variables
    public int chunkWidth, chunkHeight, chunkDepth;
    // Default mesh material
    public Material defMaterial;
    // World seed
    public int seed;
    // World offsets
    private int offX, offY;

    private void Start()
    {
        for (int i = 0; i < spawnAreaSize; i++)
        {
            for (int j = 0; j < spawnAreaSize; j++)
            {
                chunks.Add(NewChunk(i, j, defMaterial));
            }
        }

        Random.InitState(seed);
        offX = Random.Range(0, 1000000);
        offY = Random.Range(0, 1000000);
    }

    // Returns an instantiated chunk
    private GameObject NewChunk(int posX, int posY, Material defaultMaterial)
    {
        GameObject chunk = new GameObject();
        chunk.name = "Chunk " + posX + " " + posY;
        chunk.transform.position = new Vector3(chunkWidth * posX, 0, chunkDepth * posY);
        chunk.transform.parent = transform;
        chunk.tag = "Chunk";

        Debug.Log(transform.position);

        chunk.AddComponent<BlockChunk>();
        chunk.GetComponent<BlockChunk>().chunkX = posX;
        chunk.GetComponent<BlockChunk>().chunkY = posY;
        BlockChunk.width = chunkWidth;
        BlockChunk.height = chunkHeight;
        BlockChunk.depth = chunkDepth;
        chunk.GetComponent<BlockChunk>().defaultMaterial = defMaterial;

        chunk.GetComponent<BlockChunk>().SetupChunk(0, 0);
        chunk.GetComponent<BlockChunk>().GenerateMesh();

        UpdateNeighbours(chunk.GetComponent<BlockChunk>());

        return chunk;
    }

    private void Update()
    {
        FindClosestChunk();

        AddNewChunks();
    }

    // Check if new chuynks need to be generated
    private void AddNewChunks()
    {
        int x = currentChunk.GetComponent<BlockChunk>().chunkX;
        int y = currentChunk.GetComponent<BlockChunk>().chunkY;

        for (int i = x - renderDistance; i < x + renderDistance; i++)
        {
            for (int j = y - renderDistance; j < y + renderDistance; j++)
            {
                bool exists = false;
                foreach (GameObject chunkObj in chunks)
                {
                    if (i == chunkObj.GetComponent<BlockChunk>().chunkX && j == chunkObj.GetComponent<BlockChunk>().chunkY)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    GameObject newChunk = NewChunk(i, j, defMaterial);
                    chunks.Add(newChunk);
                }

            }
        }
    }

    private void FindClosestChunk()
    {
        float dist = 999999f;
        GameObject auxChunk = null;

        foreach (GameObject chunkObj in chunks)
        {
            if (Vector3.Distance(player.transform.position, chunkObj.transform.position) < dist)
            {
                dist = Vector3.Distance(player.transform.position, chunkObj.transform.position);
                auxChunk = chunkObj;
            }
        }

        currentChunk = auxChunk;
    }

    private void UpdateNeighbours(BlockChunk targetChunk)
    {
        //Update neighbours for single chunk
        foreach (GameObject chunkObj in chunks)
        {
            BlockChunk chunk = chunkObj.GetComponent<BlockChunk>();

            if (chunk != targetChunk)
            {
                if (adjCheck(chunk, targetChunk))
                {
                    setAdjChunk(targetChunk, chunk);
                }
            }
        }
    }

    private bool adjCheck(BlockChunk a, BlockChunk b)
    {
        if (a != b)
        {
            if (a.chunkX >= b.chunkX - 1 && a.chunkX <= b.chunkX + 1)
            {
                if (a.chunkY >= b.chunkY - 1 && a.chunkY <= b.chunkY + 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void setAdjChunk(BlockChunk source, BlockChunk target)
    {
        int x = target.chunkX - source.chunkX;
        int y = target.chunkY - source.chunkY;

        if (x == -1)
        {
            if (y == -1)
            {
                source.adjChunks[0] = target;
                target.adjChunks[4] = source;

                source.pendingUpdate = true;
                target.pendingUpdate = true;

                return;
            }
            else if (y == 0)
            {
                source.adjChunks[1] = target;
                target.adjChunks[5] = source;

                source.pendingUpdate = true;
                target.pendingUpdate = true;


                return;
            }
            else if (y == 1)
            {
                source.adjChunks[2] = target;
                target.adjChunks[6] = source;

                source.pendingUpdate = true;
                target.pendingUpdate = true;


                return;
            }
        }
        else if (x == 0)
        {
            if (y == -1)
            {
                source.adjChunks[7] = target;
                target.adjChunks[3] = source;

                source.pendingUpdate = true;
                target.pendingUpdate = true;


                return;
            }
            else if (y == 1)
            {
                source.adjChunks[3] = target;
                target.adjChunks[7] = source;

                source.pendingUpdate = true;
                target.pendingUpdate = true;


                return;
            }
        }
        else if (x == 1)
        {
            if (y == -1)
            {
                source.adjChunks[6] = target;
                target.adjChunks[2] = source;

                source.pendingUpdate = true;
                target.pendingUpdate = true;


                return;
            }
            else if (y == 0)
            {
                source.adjChunks[5] = target;
                target.adjChunks[1] = source;

                source.pendingUpdate = true;
                target.pendingUpdate = true;


                return;
            }
            else if (y == 1)
            {
                source.adjChunks[4] = target;
                target.adjChunks[0] = source;

                source.pendingUpdate = true;
                target.pendingUpdate = true;


                return;
            }
        }
    }

}
