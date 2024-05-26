using System.Collections.Generic;
using UnityEngine;

namespace TileGame
{
    [System.Serializable]
    public struct TileData
    {
        public int id;
        public Vector3 position;
        public string character;
        public List<int> children;
        public int score;
    }
}