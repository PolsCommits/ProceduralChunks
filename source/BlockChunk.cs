using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockChunk : MonoBehaviour
{
    public BlockData[,,] chunkData;
    public List<BlockData> updateBlocks = new List<BlockData>();

    List<Vector3> vertices = new List<Vector3>();
    List<Vector3> normals = new List<Vector3>();
    List<Vector2> uv = new List<Vector2>();
    List<int> tri = new List<int>();
    Mesh mesh;

    MeshFilter mf;
    MeshRenderer mr;

    public static int width, height, depth;
    public int chunkX, chunkY;

    public BlockChunk[] adjChunks = new BlockChunk[8];

    public Material defaultMaterial;

    public bool pendingUpdate = false;
    // Number of adjacent chunks
    public int adjLen = 0;

    int vertexId = 0;

    int offsetX, offsetY;

    // Set up variables and components, with random offsets
    public void SetupChunk(int offX, int offY)
    {
        // Setup chunk offsets
        offsetX = offX;
        offsetY = offY;

        // Create new empty chunk data
        chunkData = new BlockData[width, height, depth];



        // Setup mesh filter component or add new one if needed
        mf = GetComponent<MeshFilter>();
        if (!mf)
            mf = gameObject.AddComponent<MeshFilter>();

        mr = GetComponent<MeshRenderer>();
        if (!mr)
            mr = gameObject.AddComponent<MeshRenderer>();

        mr.material = defaultMaterial;

        mesh = new Mesh();

        GenerateBlocks();
    }

    // Don't forget to turn on Gizmos
    private void ViewChunkCorners()
    {
        Debug.DrawLine(new Vector3(chunkX * width, 0, chunkY * depth), new Vector3(chunkX * width, 50, chunkY * depth), Color.red, 0f, true);
        Debug.DrawLine(new Vector3(chunkX * width + width, 0, chunkY * depth), new Vector3(chunkX * width + width, 50, chunkY * depth), Color.red, 0f, true);
        Debug.DrawLine(new Vector3(chunkX * width, 0, chunkY * depth + depth), new Vector3(chunkX * width, 50, chunkY * depth + depth), Color.red, 0f, true);
        Debug.DrawLine(new Vector3(chunkX * width + width, 0, chunkY * depth + depth), new Vector3(chunkX * width + width, 50, chunkY * depth + depth), Color.red, 0f, true);
    }

    public static int PerlinHeight(int atX, int atY, BlockChunk chunk, int maxHeight)
    {
        return (int)Mathf.Min(Mathf.PerlinNoise((chunk.chunkX * BlockChunk.width + atX) / 10.0f, (chunk.chunkY * BlockChunk.depth + atY) / 10f) * maxHeight, maxHeight - 1);
    }

    public static int ImprovedPerlinHeight(int atX, int atY, BlockChunk chunk, int maxHeight)
    {
        return (int)Mathf.Min(
            (
            Mathf.PerlinNoise((chunk.chunkX * BlockChunk.width + atX) / 10.0f, (chunk.chunkY * BlockChunk.depth + atY) / 10f) +
            Mathf.PerlinNoise((chunk.chunkX * BlockChunk.width + atX) / 20.0f, (chunk.chunkY * BlockChunk.depth + atY) / 20f)
            )
            * maxHeight,
            BlockChunk.height - 1
            );
    }

    public void GenerateBlocks()
    {
        for (int i = 0; i < chunkData.GetLength(0); i++)
        {
            for (int k = 0; k < chunkData.GetLength(2); k++)
            {
                //int localHeight = PerlinHeight(i, k, this, height);
                int localHeight = ImprovedPerlinHeight(i + offsetX, k + offsetY, this, 16);
                for (int j = 0; j < localHeight; j++)
                {
                    int chance = Random.Range(1, 2);

                    if (chance == 1)
                        chunkData[i, j, k] = new Block_Generic();
                    else
                        chunkData[i, j, k] = new Block_Torch();
                }
            }
        }
    }
    private void Update()
    {
        if (pendingUpdate)
        {
            GenerateMesh();
        }

        for(int i = 0; i < updateBlocks.Count; i++)
        {
            if(updateBlocks[i] != null)
            {
                if(updateBlocks[i] as Block_Liquid != null)
                {
                    Block_Liquid auxBlock = updateBlocks[i] as Block_Liquid;

                    if(auxBlock != null)
                    {
                        if(auxBlock.pendingUpdate)
                            auxBlock.UpdateBlock();
                    }
                }
            }
        }

        ViewChunkCorners();

        int aux = 0;
        for (int i = 0; i < 8; i++)
            if (adjChunks[i])
                aux++;

        if (adjLen != aux)
            adjLen = aux;
    }

    // Generate mesh based on chunkData
    public void GenerateMesh()
    {
        int blockId = 0;
        vertexId = 0;

        pendingUpdate = false;

        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        uv = new List<Vector2>();
        tri = new List<int>();
        mesh = new Mesh();

        for (int x = 0; x < chunkData.GetLength(0); x++)
        {
            for (int y = 0; y < chunkData.GetLength(1); y++)
            {
                for (int z = 0; z < chunkData.GetLength(2); z++)
                {
                    if (chunkData[x, y, z] != null)
                    {
                        chunkData[x, y, z].GenerateCustomMesh(new Vector3(x, y, z), this, ref mesh, ref vertices, ref normals, ref tri, ref uv, ref vertexId);
                        blockId++;
                    }
                }
            }
        }

        UpdateMeshFilter();
    }

    void UpdateMeshFilter()
    {
        mesh.vertices = vertices.ToArray();
        mesh.triangles = tri.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uv.ToArray();

        mf.mesh = mesh;

        if (!gameObject.GetComponent<MeshCollider>())
            gameObject.AddComponent<MeshCollider>();
        else
            gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    // Returns the position of the block currently pointed at, based on collisionn point and forward facing direction of player and the hit
    public Vector3Int GetRemoveAtPosition(Vector3 hitPoint, Vector3 dir, RaycastHit hit)
    {
        Vector3Int target = new Vector3Int(0, 0, 0);

        Vector3 normalised = hitPoint - new Vector3(chunkX * width, 0, chunkY * depth);

        Vector3Int voxelPosition = new Vector3Int(Mathf.FloorToInt(normalised.x), Mathf.FloorToInt(normalised.y), Mathf.FloorToInt(normalised.z));

        Vector3Int offset = new Vector3Int();

        if (hit.normal == Vector3.up)
        {
            if (chunkData[voxelPosition.x, voxelPosition.y, voxelPosition.z] != null && chunkData[voxelPosition.x, voxelPosition.y, voxelPosition.z].Id != 0)
                offset = new Vector3Int(0, 0, 0);
            else
                offset = new Vector3Int(0, -1, 0);
        }
        else if (hit.normal == Vector3.down)
        {
            offset = new Vector3Int(0, 0, 0);
        }
        else if (hit.normal == Vector3.forward)
        {
            if (chunkData[voxelPosition.x, voxelPosition.y, voxelPosition.z] != null && chunkData[voxelPosition.x, voxelPosition.y, voxelPosition.z].Id != 0)
                offset = new Vector3Int(0, 0, 0);
            else
                offset = new Vector3Int(0, 0, -1);
        }
        else if (hit.normal == Vector3.back)
        {
            offset = new Vector3Int(0, 0, 0);
        }
        else if (hit.normal == Vector3.right)
        {
            if (chunkData[voxelPosition.x, voxelPosition.y, voxelPosition.z] != null && chunkData[voxelPosition.x, voxelPosition.y, voxelPosition.z].Id != 0)
                offset = new Vector3Int(0, 0, 0);
            else
                offset = new Vector3Int(-1, 0, 0);
        }
        else if (hit.normal == Vector3.left)
        {
            offset = new Vector3Int(0, 0, 0);
        }

        return voxelPosition + offset;
    }

    public void RemoveBlock(Vector3Int at)
    {
        if (chunkData[at.x, at.y, at.z] != null && chunkData[at.x, at.y, at.z].Id != 0)
        {
            Debug.Log("Removing block at " + at);
            pendingUpdate = true;
            chunkData[at.x, at.y, at.z].Destroy();
            chunkData[at.x, at.y, at.z] = null;

            if (at.x == 0)
            {
                if (adjChunks[1])
                {
                    adjChunks[1].pendingUpdate = true;
                }
            }
            else if (at.x == width - 1 && adjChunks[5])
            {
                if (adjChunks[5])
                {
                    adjChunks[5].pendingUpdate = true;
                }
            }

            if (at.z == 0)
            {
                if (adjChunks[7])
                {
                    adjChunks[7].pendingUpdate = true;
                }
            }
            else if (at.z == depth - 1 && adjChunks[3])
            {
                if (adjChunks[3])
                {
                    adjChunks[3].pendingUpdate = true;
                }
            }
        }
        else
        {
            Debug.Log("No block at " + at);
        }
    }

    public Vector3Int GetAddAtPosition(Vector3 hitPoint, Vector3 dir, RaycastHit hit)
    {
        Vector3 normalised = hitPoint - new Vector3(chunkX * width, 0, chunkY * depth);

        Vector3Int voxelPosition = new Vector3Int(Mathf.FloorToInt(normalised.x), Mathf.FloorToInt(normalised.y), Mathf.FloorToInt(normalised.z));

        Vector3Int offset = new Vector3Int();

        if (hit.normal == Vector3.up)
        {
            if (chunkData[voxelPosition.x, voxelPosition.y, voxelPosition.z] == null || chunkData[voxelPosition.x, voxelPosition.y, voxelPosition.z].Id == 0)
                offset = new Vector3Int(0, 0, 0);
            else
                offset = new Vector3Int(0, 1, 0);
        }
        else if (hit.normal == Vector3.down)
        {
            offset = new Vector3Int(0, -1, 0);
        }
        else if (hit.normal == Vector3.forward)
        {
            if (chunkData[voxelPosition.x, voxelPosition.y, voxelPosition.z] == null || chunkData[voxelPosition.x, voxelPosition.y, voxelPosition.z].Id == 0)
                offset = new Vector3Int(0, 0, 0);
            else
                offset = new Vector3Int(0, 0, 1);
        }
        else if (hit.normal == Vector3.back)
        {
            offset = new Vector3Int(0, 0, -1);
        }
        else if (hit.normal == Vector3.right)
        {
            if (chunkData[voxelPosition.x, voxelPosition.y, voxelPosition.z] == null || chunkData[voxelPosition.x, voxelPosition.y, voxelPosition.z].Id == 0)
                offset = new Vector3Int(0, 0, 0);
            else
                offset = new Vector3Int(1, 0, 0);
        }
        else if (hit.normal == Vector3.left)
        {
            offset = new Vector3Int(-1, 0, 0);
        }

        return voxelPosition + offset;
    }

    public void AddBlock(Vector3Int at)
    {
        Debug.Log(at);

        // Place in separate chunk if neccessary
        BlockChunk auxChunk = null;

        if (at.x == -1)
        {
            if (at.z == -1)
            { auxChunk = adjChunks[0]; at = new Vector3Int(width - 1, at.y, depth - 1); }
            else if (at.z == depth)
            { auxChunk = adjChunks[2]; at = new Vector3Int(width - 1, at.y, 0); }
            else
            { auxChunk = adjChunks[1]; at = new Vector3Int(width - 1, at.y, at.z); }
        }
        else if (at.x == width)
        {
            if (at.z == -1)
            { auxChunk = adjChunks[6]; at = new Vector3Int(0, at.y, depth - 1); }
            else if (at.z == depth)
            { auxChunk = adjChunks[4]; at = new Vector3Int(0, at.y, 0); }
            else
            { auxChunk = adjChunks[5]; at = new Vector3Int(0, at.y, at.z); }
        }
        else
        {
            if (at.z == -1)
            { auxChunk = adjChunks[7]; at = new Vector3Int(at.x, at.y, depth - 1); }
            else if (at.z == depth)
            { auxChunk = adjChunks[3]; at = new Vector3Int(at.x, at.y, 0); }
        }

        Debug.Log("New position is " + at);

        if (auxChunk == null)
            auxChunk = this;

        if (auxChunk.chunkData[at.x, at.y, at.z] == null || auxChunk.chunkData[at.x, at.y, at.z].Id == 0)
        {
            Debug.Log("Adding voxel at " + at);
            auxChunk.pendingUpdate = true;
            auxChunk.chunkData[at.x, at.y, at.z] = new Block_Liquid(this, at);

            if (at.x == 0)
            {
                if (auxChunk.adjChunks[1])
                {
                    auxChunk.adjChunks[1].pendingUpdate = true;
                }
            }
            else if (at.x == width - 1 && auxChunk.adjChunks[5])
            {
                if (auxChunk.adjChunks[5])
                {
                    auxChunk.adjChunks[5].pendingUpdate = true;
                }
            }

            if (at.z == 0)
            {
                if (auxChunk.adjChunks[7])
                {
                    auxChunk.adjChunks[7].pendingUpdate = true;
                }
            }
            else if (at.z == depth - 1 && auxChunk.adjChunks[3])
            {
                if (auxChunk.adjChunks[3])
                {
                    auxChunk.adjChunks[3].pendingUpdate = true;
                }
            }
        }
        else
        {
            Debug.Log("Block already at " + at);
        }
    }
}
