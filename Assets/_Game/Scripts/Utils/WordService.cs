using System.Collections.Generic;
using UnityEngine;

namespace TileGame
{
    /// <summary>
    /// Word service can be used to check if a word is in the dictionary and if a list of letters can form a word.
    /// Loads the dictionary data from the Resources folder.
    /// Can be instantiated in any class.
    /// </summary>
    public class WordService
    {
        private HashSet<string> _validWords;

        public WordService()
        {
            var dictionaryData = Resources.Load<DictionaryData>("DictionaryData");
            if (dictionaryData != null)
            {
                _validWords = dictionaryData.GetWords();
            }
            else
            {
                Debug.LogError("DictionaryData not found in Resources folder!");
                _validWords = new HashSet<string>();
            }
        }

        public bool CheckIfInDictionary(string word)
        {
            return word.Length >= 2 && _validWords.Contains(word.ToUpper());
        }

        public bool CheckIfWordsCanBeFormed(List<char> letters)
        {
            var letterCounts = new Dictionary<char, int>();
            foreach (var letter in letters)
            {
                if (letterCounts.ContainsKey(letter))
                {
                    letterCounts[letter]++;
                }
                else
                {
                    letterCounts[letter] = 1;
                }
            }

            foreach (var word in _validWords)
            {
                var tempLetterCounts = new Dictionary<char, int>(letterCounts);
                var canFormWord = true;
                foreach (var letter in word)
                {
                    if (tempLetterCounts.ContainsKey(letter) && tempLetterCounts[letter] > 0)
                    {
                        tempLetterCounts[letter]--;
                    }
                    else
                    {
                        canFormWord = false;
                        break;
                    }
                }

                if (canFormWord)
                {
                    return true;
                }
            }
            return false;
        }
    }
}