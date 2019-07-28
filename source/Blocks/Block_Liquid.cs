using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_Liquid : BlockData
{
    private BlockChunk chunk;
    public bool pendingUpdate = false;
    Vector3Int position;
    public float lastUpdate = 0;
    float flowSpeed = .25f;
    int maxFlow = 5;
    public int currentFlow;

    public Block_Liquid(BlockChunk chunk, Vector3Int position = new Vector3Int(), int flow=3)
    {
        lastUpdate = Time.time;
        currentFlow = flow;
        SetId();
        SetMesh();
        SetupBlock(chunk, position);
        SetUpdate();
    }

    // Set Id to desired value
    protected override void SetId()
    {
        this.Id = 3;
    }

    protected override void SetupBlock(BlockChunk chunk, Vector3Int position= new Vector3Int())
    {
        if(chunk != null)
        {
            this.chunk = chunk;
            this.position = position;
            chunk.updateBlocks.Add(this);
        }
        else
        {
            //Debug.Log("NULL chunk passed to liquid block at " + position);
        }
    }

    protected override void SetUpdate()
    {
        if(chunk != null)
        {
            pendingUpdate = true;
        }
    }

    public void UpdateBlock()
    {
        if(Time.time - flowSpeed > lastUpdate)
        {
            BlockChunk auxChunk = chunk;
            Vector3Int at = position;

            // Only check LEFT, RIGHT, FORWARD and BACK positions if the block below this one can support flow
            if(chunk.chunkData[position.x, position.y - 1, position.z] as Block_Liquid == null)
            {
                // BOTTOM
                at = new Vector3Int(at.x, at.y - 1, at.z);

                if (auxChunk.chunkData[at.x, at.y, at.z] == null)
                {
                    auxChunk.chunkData[position.x, position.y - 1, position.z] = new Block_Liquid(auxChunk, at, maxFlow);
                    auxChunk.pendingUpdate = true;
                    lastUpdate = Time.time;
                    pendingUpdate = false;
                    return;
                }

                // Only flow vertically if flow is > 0
                if(currentFlow > 0)
                {

                    auxChunk = chunk;
                    at = position;

                    // FORWARD
                    if (position.z + 1 >= BlockChunk.depth)
                    {
                        auxChunk = chunk.adjChunks[3];
                        at = new Vector3Int(at.x, at.y, 0);
                    }
                    else
                    {
                        at = new Vector3Int(at.x, at.y, at.z + 1);
                    }

                    if (auxChunk.chunkData[at.x, at.y, at.z] == null && chunk.chunkData[position.x, position.y - 1, position.z] != null)
                    {
                        auxChunk.chunkData[at.x, at.y, at.z] = new Block_Liquid(auxChunk, at, currentFlow - 1);
                        auxChunk.pendingUpdate = true;
                    }

                    auxChunk = chunk;
                    at = position;

                    // RIGHT
                    if (position.x + 1 >= BlockChunk.width)
                    {
                        auxChunk = chunk.adjChunks[5];
                        at = new Vector3Int(0, at.y, at.z);
                    }
                    else
                    {
                        at = new Vector3Int(at.x + 1, at.y, at.z);
                    }

                    if (auxChunk.chunkData[at.x, at.y, at.z] == null && chunk.chunkData[position.x, position.y - 1, position.z] != null)
                    {
                        auxChunk.chunkData[at.x, at.y, at.z] = new Block_Liquid(auxChunk, at, currentFlow - 1);
                        auxChunk.pendingUpdate = true;
                    }

                    auxChunk = chunk;
                    at = position;

                    // LEFT
                    if (position.x - 1 < 0)
                    {
                        auxChunk = chunk.adjChunks[1];
                        at = new Vector3Int(BlockChunk.width - 1, at.y, at.z);
                    }
                    else
                    {
                        at = new Vector3Int(at.x - 1, at.y, at.z);
                    }

                    if (auxChunk.chunkData[at.x, at.y, at.z] == null && chunk.chunkData[position.x, position.y - 1, position.z] != null)
                    {
                        auxChunk.chunkData[at.x, at.y, at.z] = new Block_Liquid(auxChunk, at, currentFlow - 1);
                        auxChunk.pendingUpdate = true;
                    }

                    auxChunk = chunk;
                    at = position;

                    // BACK
                    if (position.z - 1 < 0)
                    {
                        auxChunk = chunk.adjChunks[7];
                        at = new Vector3Int(at.x, at.y, BlockChunk.depth - 1);
                    }
                    else
                    {
                        at = new Vector3Int(at.x, at.y, at.z - 1);
                    }

                    if (auxChunk.chunkData[at.x, at.y, at.z] == null && chunk.chunkData[position.x, position.y - 1, position.z] != null)
                    {
                        auxChunk.chunkData[at.x, at.y, at.z] = new Block_Liquid(auxChunk, at, currentFlow - 1);
                        auxChunk.pendingUpdate = true;
                    }

                    lastUpdate = Time.time;
                    pendingUpdate = false;
                    return;
                }
            }
        }
    }

    public override void GenerateCustomMesh(Vector3 at,
    BlockChunk chunk,
    ref Mesh refMesh,
    ref List<Vector3> vertices,
    ref List<Vector3> normals,
    ref List<int> tri,
    ref List<Vector2> uv,
    ref int vertexId)
    {
        int x = (int)at.x, y = (int)at.y, z = (int)at.z;
        int width = BlockChunk.width, height = BlockChunk.height, depth = BlockChunk.depth;

        // TOP
        if (y < height - 1)
        {
            if ((chunk.chunkData[x, y + 1, z] == null || chunk.chunkData[x, y + 1, z].Id != 1))
            {
                vertices.Add(new Vector3(x, y + (currentFlow / (float)maxFlow), z));
                vertices.Add(new Vector3(x + 1, y + (currentFlow / (float)maxFlow), z));
                vertices.Add(new Vector3(x, y + (currentFlow / (float)maxFlow), z + 1));
                vertices.Add(new Vector3(x + 1, y + (currentFlow / (float)maxFlow), z + 1));

                tri.Add(vertexId);
                tri.Add(vertexId + 2);
                tri.Add(vertexId + 1);

                tri.Add(vertexId + 2);
                tri.Add(vertexId + 3);
                tri.Add(vertexId + 1);

                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);

                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(1, 0));
                uv.Add(new Vector2(.5f, 1));
                uv.Add(new Vector2(1, 1));

                vertexId += 4;
            }
        }
        // BOTTOM
        if (y > 0)
        {
            if ((chunk.chunkData[x, y - 1, z] == null || chunk.chunkData[x, y - 1, z].Id != 1))
            {
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x + 1, y, z + 1));

                tri.Add(vertexId);
                tri.Add(vertexId + 2);
                tri.Add(vertexId + 1);

                tri.Add(vertexId + 2);
                tri.Add(vertexId + 3);
                tri.Add(vertexId + 1);

                normals.Add(-Vector3.up);
                normals.Add(-Vector3.up);
                normals.Add(-Vector3.up);
                normals.Add(-Vector3.up);

                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(1, 0));
                uv.Add(new Vector2(.5f, 1));
                uv.Add(new Vector2(1, 1));

                vertexId += 4;
            }
        }
        // RIGHT
        if (x < width - 1)
        {
            if ((chunk.chunkData[x + 1, y, z] == null || chunk.chunkData[x + 1, y, z].Id != 1))
            {
                vertices.Add(new Vector3(1 + x, y, z));
                vertices.Add(new Vector3(1 + x, y, z + 1));
                vertices.Add(new Vector3(1 + x, y + (currentFlow / (float)maxFlow), z));
                vertices.Add(new Vector3(1 + x, y + (currentFlow / (float)maxFlow), z + 1));

                tri.Add(vertexId);
                tri.Add(vertexId + 2);
                tri.Add(vertexId + 1);

                tri.Add(vertexId + 2);
                tri.Add(vertexId + 3);
                tri.Add(vertexId + 1);

                normals.Add(Vector3.right);
                normals.Add(Vector3.right);
                normals.Add(Vector3.right);
                normals.Add(Vector3.right);

                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(1, 0));
                uv.Add(new Vector2(.5f, 1));
                uv.Add(new Vector2(1, 1));

                vertexId += 4;
            }
        }
        else if (chunk.adjChunks[5] != null)
        {
            if ((chunk.adjChunks[5].chunkData[0, y, z] == null || chunk.adjChunks[5].chunkData[0, y, z].Id != 1))
            {
                vertices.Add(new Vector3(1 + x, y, z));
                vertices.Add(new Vector3(1 + x, y, z + 1));
                vertices.Add(new Vector3(1 + x, y + (currentFlow / (float)maxFlow), z));
                vertices.Add(new Vector3(1 + x, y + (currentFlow / (float)maxFlow), z + 1));

                tri.Add(vertexId);
                tri.Add(vertexId + 2);
                tri.Add(vertexId + 1);

                tri.Add(vertexId + 2);
                tri.Add(vertexId + 3);
                tri.Add(vertexId + 1);

                normals.Add(Vector3.right);
                normals.Add(Vector3.right);
                normals.Add(Vector3.right);
                normals.Add(Vector3.right);

                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(1, 0));
                uv.Add(new Vector2(.5f, 1));
                uv.Add(new Vector2(1, 1));

                vertexId += 4;
            }
        }
        // FRONT
        if (z < depth - 1)
        {
            if ((chunk.chunkData[x, y, z + 1] == null || chunk.chunkData[x, y, z + 1].Id != 1))
            {
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x, y + (currentFlow / (float)maxFlow), z + 1));
                vertices.Add(new Vector3(x + 1, y + (currentFlow / (float)maxFlow), z + 1));

                tri.Add(vertexId + 1);
                tri.Add(vertexId + 2);
                tri.Add(vertexId);

                tri.Add(vertexId + 1);
                tri.Add(vertexId + 3);
                tri.Add(vertexId + 2);

                normals.Add(Vector3.forward);
                normals.Add(Vector3.forward);
                normals.Add(Vector3.forward);
                normals.Add(Vector3.forward);

                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(1, 0));
                uv.Add(new Vector2(.5f, 1));
                uv.Add(new Vector2(1, 1));

                vertexId += 4;
            }
        }
        else if (chunk.adjChunks[3] != null)
        {
            if ((chunk.adjChunks[3].chunkData[x, y, 0] == null || chunk.adjChunks[3].chunkData[x, y, 0].Id != 1))
            {
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x, y + (currentFlow / (float)maxFlow), z + 1));
                vertices.Add(new Vector3(x + 1, y + (currentFlow / (float)maxFlow), z + 1));

                tri.Add(vertexId + 1);
                tri.Add(vertexId + 2);
                tri.Add(vertexId);

                tri.Add(vertexId + 1);
                tri.Add(vertexId + 3);
                tri.Add(vertexId + 2);

                normals.Add(Vector3.forward);
                normals.Add(Vector3.forward);
                normals.Add(Vector3.forward);
                normals.Add(Vector3.forward);

                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(1, 0));
                uv.Add(new Vector2(.5f, 1));
                uv.Add(new Vector2(1, 1));

                vertexId += 4;
            }
        }
        // LEFT
        if (x > 0)
        {
            if ((chunk.chunkData[x - 1, y, z] == null || chunk.chunkData[x - 1, y, z].Id != 1))
            {
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x, y + (currentFlow / (float)maxFlow), z));
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x, y + (currentFlow / (float)maxFlow), z + 1));

                tri.Add(vertexId);
                tri.Add(vertexId + 2);
                tri.Add(vertexId + 1);

                tri.Add(vertexId + 2);
                tri.Add(vertexId + 3);
                tri.Add(vertexId + 1);

                normals.Add(-Vector3.right);
                normals.Add(-Vector3.right);
                normals.Add(-Vector3.right);
                normals.Add(-Vector3.right);

                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(1, 0));
                uv.Add(new Vector2(.5f, 1));
                uv.Add(new Vector2(1, 1));

                vertexId += 4;
            }
        }
        else if (chunk.adjChunks[1] != null)
        {
            if ((chunk.adjChunks[1].chunkData[width - 1, y, z] == null || chunk.adjChunks[1].chunkData[width - 1, y, z].Id != 1))
            {
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x, y + (currentFlow / (float)maxFlow), z));
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x, y + (currentFlow / (float)maxFlow), z + 1));

                tri.Add(vertexId);
                tri.Add(vertexId + 2);
                tri.Add(vertexId + 1);

                tri.Add(vertexId + 2);
                tri.Add(vertexId + 3);
                tri.Add(vertexId + 1);

                normals.Add(-Vector3.right);
                normals.Add(-Vector3.right);
                normals.Add(-Vector3.right);
                normals.Add(-Vector3.right);

                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(1, 0));
                uv.Add(new Vector2(.5f, 1));
                uv.Add(new Vector2(1, 1));

                vertexId += 4;
            }
        }
        // BACK
        if (z > 0)
        {
            if ((chunk.chunkData[x, y, z - 1] == null || chunk.chunkData[x, y, z - 1].Id != 1))
            {
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x, y + (currentFlow / (float)maxFlow), z));
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x + 1, y + (currentFlow / (float)maxFlow), z));

                tri.Add(vertexId + 1);
                tri.Add(vertexId + 2);
                tri.Add(vertexId);

                tri.Add(vertexId + 1);
                tri.Add(vertexId + 3);
                tri.Add(vertexId + 2);

                normals.Add(-Vector3.forward);
                normals.Add(-Vector3.forward);
                normals.Add(-Vector3.forward);
                normals.Add(-Vector3.forward);

                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(1, 0));
                uv.Add(new Vector2(.5f, 1));
                uv.Add(new Vector2(1, 1));

                vertexId += 4;
            }
        }
        else if (chunk.adjChunks[7] != null)
        {
            if ((chunk.adjChunks[7].chunkData[x, y, depth - 1] == null || chunk.adjChunks[7].chunkData[x, y, depth - 1].Id != 1))
            {
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x, y + (currentFlow / (float)maxFlow), z));
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x + 1, y + (currentFlow / (float)maxFlow), z));

                tri.Add(vertexId + 1);
                tri.Add(vertexId + 2);
                tri.Add(vertexId);

                tri.Add(vertexId + 1);
                tri.Add(vertexId + 3);
                tri.Add(vertexId + 2);

                normals.Add(-Vector3.forward);
                normals.Add(-Vector3.forward);
                normals.Add(-Vector3.forward);
                normals.Add(-Vector3.forward);

                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(1, 0));
                uv.Add(new Vector2(.5f, 1));
                uv.Add(new Vector2(1, 1));

                vertexId += 4;
            }
        }

    }

    public override void Destroy()
    {
        // If block is NOT on the border of the chunk
        if( position.x > 0 && position.x < BlockChunk.depth - 1 && position.z > 0 && position.z < BlockChunk.width - 1)
        {
            Block_Liquid auxBlock = chunk.chunkData[position.x - 1, position.y, position.z] as Block_Liquid;

            if(auxBlock != null)
            {
                auxBlock.pendingUpdate = true;
            }

            auxBlock = chunk.chunkData[position.x + 1, position.y, position.z] as Block_Liquid;

            if (auxBlock != null)
            {
                auxBlock.pendingUpdate = true;
            }

            auxBlock = chunk.chunkData[position.x, position.y, position.z - 1] as Block_Liquid;

            if (auxBlock != null)
            {
                auxBlock.pendingUpdate = true;
            }

            auxBlock = chunk.chunkData[position.x, position.y, position.z + 1] as Block_Liquid;

            if (auxBlock != null)
            {
                auxBlock.pendingUpdate = true;
            }

            auxBlock = chunk.chunkData[position.x, position.y, position.z - 1] as Block_Liquid;

            if (auxBlock != null)
            {
                auxBlock.pendingUpdate = true;
            }
        }
    }
}
