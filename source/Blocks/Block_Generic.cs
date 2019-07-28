using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_Generic : BlockData
{
    // Set Id to desired value
    protected override void SetId()
    {
        this.Id = 1;
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
            if (chunk.chunkData[x, y + 1, z] == null || chunk.chunkData[x, y + 1, z].Id != 1)
            {
                vertices.Add(new Vector3(x, 1 + y, z));
                vertices.Add(new Vector3(x + 1, 1 + y, z));
                vertices.Add(new Vector3(x, 1 + y, z + 1));
                vertices.Add(new Vector3(x + 1, 1 + y, z + 1));

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

                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(.5f, 1));

                vertexId += 4;
            }
        }
        // BOTTOM
        if (y > 0)
        {
            if (chunk.chunkData[x, y - 1, z] == null || chunk.chunkData[x, y - 1, z].Id != 1)
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

                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(.5f, 1));

                vertexId += 4;
            }
        }
        // RIGHT
        if (x < width - 1)
        {
            if (chunk.chunkData[x + 1, y, z] == null || chunk.chunkData[x + 1, y, z].Id != 1)
            {
                vertices.Add(new Vector3(1 + x, y, z));
                vertices.Add(new Vector3(1 + x, y, z + 1));
                vertices.Add(new Vector3(1 + x, y + 1, z));
                vertices.Add(new Vector3(1 + x, y + 1, z + 1));

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

                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(.5f, 1));

                vertexId += 4;
            }
        }
        else if (chunk.adjChunks[5] != null)
        {
            if (chunk.adjChunks[5].chunkData[0, y, z] == null || chunk.adjChunks[5].chunkData[0, y, z].Id != 1)
            {
                vertices.Add(new Vector3(1 + x, y, z));
                vertices.Add(new Vector3(1 + x, y, z + 1));
                vertices.Add(new Vector3(1 + x, y + 1, z));
                vertices.Add(new Vector3(1 + x, y + 1, z + 1));

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

                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(.5f, 1));

                vertexId += 4;
            }
        }
        // FRONT
        if (z < depth - 1)
        {
            if (chunk.chunkData[x, y, z + 1] == null || chunk.chunkData[x, y, z + 1].Id != 1)
            {
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x, y + 1, z + 1));
                vertices.Add(new Vector3(x + 1, y + 1, z + 1));

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

                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(.5f, 1));

                vertexId += 4;
            }
        }
        else if (chunk.adjChunks[3] != null)
        {
            if (chunk.adjChunks[3].chunkData[x, y, 0] == null || chunk.adjChunks[3].chunkData[x, y, 0].Id != 1)
            {
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x, y + 1, z + 1));
                vertices.Add(new Vector3(x + 1, y + 1, z + 1));

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

                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(.5f, 1));

                vertexId += 4;
            }
        }
        // LEFT
        if (x > 0)
        {
            if (chunk.chunkData[x - 1, y, z] == null || chunk.chunkData[x - 1, y, z].Id != 1)
            {
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x, y + 1, z));
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x, y + 1, z + 1));

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

                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(.5f, 1));

                vertexId += 4;
            }
        }
        else if (chunk.adjChunks[1] != null)
        {
            if (chunk.adjChunks[1].chunkData[width - 1, y, z] == null || chunk.adjChunks[1].chunkData[width - 1, y, z].Id != 1)
            {
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x, y + 1, z));
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x, y + 1, z + 1));

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

                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(.5f, 1));

                vertexId += 4;
            }
        }
        // BACK
        if (z > 0)
        {
            if (chunk.chunkData[x, y, z - 1] == null || chunk.chunkData[x, y, z - 1].Id != 1)
            {
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x, y + 1, z));
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x + 1, y + 1, z));

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

                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(.5f, 1));

                vertexId += 4;
            }
        }
        else if (chunk.adjChunks[7] != null)
        {
            if (chunk.adjChunks[7].chunkData[x, y, depth - 1] == null || chunk.adjChunks[7].chunkData[x, y, depth - 1].Id != 1)
            {
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x, y + 1, z));
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x + 1, y + 1, z));

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

                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(.5f, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(.5f, 1));
                vertexId += 4;
            }
        }
        
    }
}
