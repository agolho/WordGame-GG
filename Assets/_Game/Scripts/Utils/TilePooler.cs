using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TileGame
{
    public class TilePooler : MonoBehaviour, IInitializable
    {
        [SerializeField] private LetterTile tilePrefab;
        [SerializeField] private int poolSize = 10;

        private List<LetterTile> _tiles = new List<LetterTile>();

        public void Initialize(GameScriptsConfig config)
        {
            for (int i = 0; i < poolSize; i++)
            {
                var tile = Instantiate(tilePrefab, transform);
                tile.gameObject.SetActive(false);
                _tiles.Add(tile);
            }
        }

        public LetterTile GetTile()
        {
            foreach (var tile in _tiles.Where(tile => !tile.gameObject.activeInHierarchy))
            {
                return tile;
            }

            var newTile = Instantiate(tilePrefab, transform);
            _tiles.Add(newTile);
            return newTile;
        }

        public void ReturnTile(LetterTile tile)
        {
            tile.transform.SetParent(transform);
            tile.gameObject.SetActive(false);
            tile.ResetTile();
            _tiles.Add(tile);
        }

    }
}
