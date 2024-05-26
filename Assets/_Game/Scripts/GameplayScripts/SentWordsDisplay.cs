using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TileGame
{
    public class SentWordsDisplay : MonoBehaviour
    {
        [SerializeField] private List<string> sentWords = new List<string>();
        [SerializeField] private GameObject sentWordPrefab;
        [SerializeField] private VerticalLayoutGroup layoutGroup1, layoutGroup2;

        public void AddWord(string word)
        {
            sentWords.Add(word);
            var sentWord = Instantiate(sentWordPrefab, layoutGroup1.transform);
            sentWord.GetComponentInChildren<TextMeshProUGUI>().text = word;

            var targetLayoutGroup = (sentWords.Count % 2 == 0) ? layoutGroup2 : layoutGroup1;
            sentWord.transform.SetParent(targetLayoutGroup.transform);
        }

        public List<string> GetSentWords()
        {
            return sentWords;
        }

        public void ResetSentWords()
        {
            sentWords.Clear();
            foreach (Transform child in layoutGroup1.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in layoutGroup2.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}