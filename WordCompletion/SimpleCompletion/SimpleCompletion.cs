﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordCompletion
{
    public class SimpleCompletion : IComplementarable
    {
        private Dictionary<string, int> wordsDictionary = new Dictionary<string, int>();
        Dictionary<string, int> wordsFromPLVocabulary = new Dictionary<string, int>();
        bool usingPLVocabulary;

        public SimpleCompletion(bool usePLVocabulary)
        {
            UsePLVocabulary(usePLVocabulary);
        }

        public void Insert(string word, int usesCount = 1)
        {
            if (wordsDictionary.ContainsKey(word))
            {
                wordsDictionary[word] += usesCount;
            }
            else
            {
                wordsDictionary.Add(word, usesCount);
            }
        }

        public void InsertWords(Dictionary<string, int> dictionary)
        {
            foreach (var word in dictionary)
            {
                Insert(word.Key, word.Value);
            }
        }

        public void ResetWords(Dictionary<string, int> dictionary)
        {
            wordsDictionary.Clear();
            wordsDictionary = dictionary.ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        public void UsePLVocabulary(bool enable)
        {
            if (enable)
            {
                wordsFromPLVocabulary = new VocabularyFromTxt().GetVocabulary();
            }
            else
            {
                wordsFromPLVocabulary.Clear();
            }

            usingPLVocabulary = enable;
        }

        public Dictionary<string, int> GetAllWords()
        {
            return wordsDictionary;
        }

        public Dictionary<string, int> FindMatches(string prefix, int max = 0)
        {
            Dictionary<string, int> matches = new Dictionary<string, int>();
            foreach (var word in wordsDictionary)
            {
                if (word.Key.StartsWith(prefix))
                {
                    matches.Add(word.Key, word.Value);
                }
            }
            if(max > 0)
            {
                if (matches.Count > max)
                {
                    return matches.Take(max) as Dictionary<string, int>;
                }
                if(usingPLVocabulary)
                {
                    int i = 0;
                    while (matches.Count < max && i < wordsFromPLVocabulary.Count)
                    {
                        var element = wordsFromPLVocabulary.ElementAt(i);
                        if (element.Key.StartsWith(prefix) && !matches.ContainsKey(element.Key))
                        {
                            matches.Add(element.Key, element.Value);
                        }
                        i++;
                    }  
                }
                return matches;
            }
            else
            {
                if (usingPLVocabulary)
                {
                    foreach(var element in wordsFromPLVocabulary)
                    {
                        if (element.Key.StartsWith(prefix) && !matches.ContainsKey(element.Key))
                        {
                            matches.Add(element.Key, element.Value);
                        }
                    }
                }
                return matches;
            }
        }

        public Dictionary<string, int> FindMostUsedMatches(string prefix, int max = 0)
        {
            Dictionary<string, int> output = new Dictionary<string, int>();
            Dictionary<string, int> matches = FindMatches(prefix);
            if (matches.Any())
            {
                var orderedMatches = matches.OrderByDescending(key => key.Value);
                int iterMax = orderedMatches.Count() > max && max > 0 ? max : orderedMatches.Count();
                for (int i = 0; i < iterMax; i++)
                {
                    var element = orderedMatches.ElementAt(i);
                    output.Add(element.Key, element.Value);
                }
            }
            return output;
        }

        public void Clear()
        {
            wordsDictionary.Clear();
        }
    }
}
