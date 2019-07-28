using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockData
{
    // Voxel id for determining block 
    public int Id = 0;
    // Texture used by mesh
    public Texture texture;
    // Mesh, if any
    public Mesh mesh;

    public BlockData(BlockChunk chunk=null, Vector3Int position=new Vector3Int())
    { 
        SetId();
        SetMesh();
        SetupBlock(chunk, position);
        SetUpdate();
    }

    public virtual void Destroy() { }

    public void GetMesh(
        Vector3 position,
        BlockChunk chunk,
        ref Mesh refMesh,
        ref List<Vector3> verts,
        ref List<Vector3> normals,
        ref List<int> tris,
        ref List<Vector2> uvs,
        ref int meshIndex /* Index of last vertex placed. Used in determining which vertices determine a tris*/)
    {

        GenerateCustomMesh(position, chunk, ref refMesh, ref verts, ref normals, ref tris, ref uvs, ref meshIndex);
    }

    // Set ID
    protected virtual void SetId() { }
    // Set any block data 
    protected virtual void SetupBlock(BlockChunk chunk=null, Vector3Int position = new Vector3Int()) { }
    // Code for generating mesh
    protected virtual void SetMesh() { }
    // Use this function to invoke a repeating updatye function
    protected virtual void SetUpdate() { }

    public virtual void GenerateCustomMesh(Vector3 at, BlockChunk chunk, ref Mesh refMesh, ref List<Vector3> verts, ref List<Vector3> normals, ref List<int> tris, ref List<Vector2> uvs, ref int meshIndex) { }
}
