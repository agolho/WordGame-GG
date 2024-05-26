using UnityEngine;
using System.Collections.Generic;

namespace TileGame
{
    [CreateAssetMenu(fileName = "NewDictionaryData", menuName = "ScriptableObjects/DictionaryData", order = 1)]
    public class DictionaryData : ScriptableObject
    {
        public TextAsset dictionaryText;

        private HashSet<string> _validWords;

        public HashSet<string> GetWords()
        {
            if (_validWords == null)
            {
                LoadWords();
            }

            return _validWords;
        }

        private void LoadWords()
        {
            _validWords = new HashSet<string>();

            if (dictionaryText == null)
            {
                Debug.LogError("Dictionary text asset is null!");
                return;
            }

            var words = dictionaryText.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                _validWords.Add(word.ToUpper()); // Ensure the words are in uppercase for case-insensitive comparison
            }
        }
    }
}