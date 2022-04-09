using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{

    enum Cubeside { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK };
    public enum BlockType { GRASS, DIRT, STONE,AIR };

    BlockType bType;
    Material material;
    Chunk owner;
    Vector3 pos;
    bool isSolid;
    //UVs for blocks
    static Vector2 lbcGrassSide = new Vector2(3f, 15f) / 16;    //Grass side
    static Vector2 lbcGrassTop = new Vector2(2f, 6f) / 16;      //Grass top
    static Vector2 lbcDirt = new Vector2(2f, 15f) / 16;         //Dirt
    static Vector2 lbcStone = new Vector2(0f, 14f) / 16;        //Stone

    static Vector2 rightOne = new Vector2(1f, 0f) / 16;         //(1,0)
    static Vector2 upOne = new Vector2(0f, 1f) / 16;            //(0,1)
    static Vector2 upRight = new Vector2(1f, 1f) / 16;          //(1,1)

    Vector2[,] blockUVs =
    {
        {lbcGrassTop, lbcGrassTop+rightOne, lbcGrassTop+upOne, lbcGrassTop+upRight},
        {lbcGrassSide, lbcGrassSide+rightOne, lbcGrassSide+upOne, lbcGrassSide+upRight},
        {lbcDirt, lbcDirt+rightOne, lbcDirt+upOne, lbcDirt+upRight},
        {lbcStone, lbcStone+rightOne, lbcStone+upOne, lbcStone+upRight},
        {lbcStone, lbcStone+rightOne, lbcStone+upOne, lbcStone+upRight}
    };

    public Block(BlockType b, Vector3 pos, Chunk owner, Material material)
    {
        this.bType = b;
        this.pos = pos;
        this.owner = owner;
        this.material = material;
        if (this.bType == BlockType.AIR)
            isSolid = false;
        else
            isSolid = true;
    }

    void CreateQuad(Cubeside side)
    { 
        Mesh myMesh = new Mesh();

        Vector3 v0 = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 v1 = new Vector3(0.5f, -0.5f, 0.5f);
        Vector3 v2 = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 v3 = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 v4 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 v5 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 v6 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 v7 = new Vector3(-0.5f, 0.5f, -0.5f);

        Vector3 front = Vector3.forward;
        Vector3 back = -Vector3.forward;
        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 left = Vector3.left;
        Vector3 right = Vector3.right;

        Vector2 uv00 = new Vector2(0, 0);
        Vector2 uv01 = new Vector2(0, 1);
        Vector2 uv10 = new Vector2(1, 0);
        Vector2 uv11 = new Vector2(1, 1);

        if (bType == BlockType.GRASS && side == Cubeside.TOP)
        {
            uv00 = blockUVs[0, 0];
            uv10 = blockUVs[0, 1];
            uv01 = blockUVs[0, 2];
            uv11 = blockUVs[0, 3];
        }
        else if (bType == BlockType.GRASS && side == Cubeside.BOTTOM)
        {
            uv00 = blockUVs[2, 0];
            uv10 = blockUVs[2, 1];
            uv01 = blockUVs[2, 2];
            uv11 = blockUVs[2, 3];
        }
        else
        {
            uv00 = blockUVs[(int)(bType + 1), 0];
            uv10 = blockUVs[(int)(bType + 1), 1];
            uv01 = blockUVs[(int)(bType + 1), 2];
            uv11 = blockUVs[(int)(bType + 1), 3];
        }

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        int[] triangles = new int[6];
        Vector2[] uv = new Vector2[4];

        switch (side)
        {
            case Cubeside.FRONT:
                vertices = new Vector3[] { v4, v5, v1, v0 };
                normals = new Vector3[] { front, front, front, front };
                break;
            case Cubeside.BACK:
                vertices = new Vector3[] { v6, v7, v3, v2 };
                normals = new Vector3[] { back, back, back, back };
                break;
            case Cubeside.TOP:
                vertices = new Vector3[] { v7, v6, v5, v4 };
                normals = new Vector3[] { up, up, up, up };
                break;
            case Cubeside.BOTTOM:
                vertices = new Vector3[] { v0, v1, v2, v3 };
                normals = new Vector3[] { down, down, down, down };
                break;
            case Cubeside.LEFT:
                vertices = new Vector3[] { v7, v4, v0, v3 };
                normals = new Vector3[] { left, left, left, left };
                break;
            case Cubeside.RIGHT:
                vertices = new Vector3[] { v5, v6, v2, v1 };
                normals = new Vector3[] { right, right, right, right };
                break;
        }
        uv = new Vector2[] { uv11, uv01, uv00, uv10 };
        triangles = new int[] { 3, 1, 0, 3, 2, 1 };
        myMesh.vertices = vertices;
        myMesh.normals = normals;
        myMesh.triangles = triangles;
        myMesh.uv = uv;

        myMesh.RecalculateBounds();

        GameObject quad = new GameObject("quad");
        quad.transform.position = this.pos;
        quad.transform.parent = owner.goChunk.transform;

        MeshFilter mf = quad.AddComponent<MeshFilter>();
        mf.mesh = myMesh;
    }

    int ConvertToLocalIndex(int i)
    {
        if (i == -1)
            return World.chunkSize - 1;
        if(i == World.chunkSize)
            return 0;
        return i;
    }

    bool HasSolidNeighbour(int x, int y, int z)
    {
        Block[,,] chunkdata = owner.chunkdata;
        if(x<0 || x>= World.chunkSize || y < 0 || y >= World.chunkSize || z < 0 || z >= World.chunkSize)
        {
            Vector3 neighbourChunkPos = owner.goChunk.transform.position + new Vector3(
                    (x - (int)pos.x) * World.chunkSize,
                    (y - (int)pos.y) * World.chunkSize,
                    (z - (int)pos.z) * World.chunkSize);
            string chunkName = World.CreateChunkName(neighbourChunkPos);

            x = ConvertToLocalIndex(x);
            y = ConvertToLocalIndex(y);
            z = ConvertToLocalIndex(z);

            Chunk neighbourChunk;
            if (World.chunkDict.TryGetValue(chunkName, out neighbourChunk))
                chunkdata = neighbourChunk.chunkdata;
            else
                return false;
        }
        else chunkdata = owner.chunkdata;
            try
            {
                return chunkdata[x, y, z].isSolid;
            }
            catch (System.IndexOutOfRangeException ex) { }
            return false;
        


    }

    public void Draw()
    {
        if (bType != BlockType.AIR)
        {
            if (!HasSolidNeighbour((int)pos.x - 1, (int)pos.y, (int)pos.z))
                CreateQuad(Cubeside.LEFT);
            if (!HasSolidNeighbour((int)pos.x + 1, (int)pos.y, (int)pos.z))
                CreateQuad(Cubeside.RIGHT);
            if (!HasSolidNeighbour((int)pos.x, (int)pos.y - 1, (int)pos.z))
                CreateQuad(Cubeside.BOTTOM);
            if (!HasSolidNeighbour((int)pos.x, (int)pos.y + 1, (int)pos.z))
                CreateQuad(Cubeside.TOP);
            if (!HasSolidNeighbour((int)pos.x, (int)pos.y, (int)pos.z - 1))
                CreateQuad(Cubeside.BACK);
            if (!HasSolidNeighbour((int)pos.x, (int)pos.y, (int)pos.z + 1))
                CreateQuad(Cubeside.FRONT);
        }
    }
}
