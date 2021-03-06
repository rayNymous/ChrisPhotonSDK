﻿using System;
using Detour;

[Serializable]
public class NavMeshSerializer
{
    public NavMeshSerializer()
    {
    }

    public NavMeshSerializer(NavMesh navMesh) : this()
    {
        Decompose(navMesh);
    }

    public NavMeshParams Param { get; set; }
    public NavMeshBuilder[] NavMeshBuilders { get; set; }

    public void Decompose(NavMesh navMesh)
    {
        Param = navMesh.Param;

        NavMeshBuilders = new NavMeshBuilder[navMesh._tiles.Length];
        for (int i = 0; i < navMesh._tiles.Length; i++)
        {
            if (navMesh._tiles[i].Data != null)
            {
                NavMeshBuilders[i] = navMesh._tiles[i].Data;
            }
        }
    }

    public NavMesh Reconstitute()
    {
        var navMesh = new NavMesh();
        navMesh.Init(Param);

        long tempRef = 0;
        long temp = 0;

        for (int i = 0; i < NavMeshBuilders.Length; i++)
        {
            if (NavMeshBuilders[i] != null)
            {
                navMesh.AddTile(NavMeshBuilders[i], NavMesh.TileFreeData, tempRef, ref temp);
                //tempRef = temp;
            }
        }
        return navMesh;
    }
}