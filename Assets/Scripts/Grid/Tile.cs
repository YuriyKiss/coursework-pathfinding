using System;
using UnityEngine;

namespace ThetaStar.Grid
{
    [Serializable]
    public struct Tile
    {
        public Vector3 Position;
        public bool IsBlocked;
        public int PosX;
        public int PosZ;

        public Tile(Vector3 position, bool isWalkable, int posX, int posZ)
        {
            Position = position;
            IsBlocked = isWalkable;
            PosX = posX;
            PosZ = posZ;
        }
    }
}
