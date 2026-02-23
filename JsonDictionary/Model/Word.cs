using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonDictionary.Model
{
    public class Word
    {
        private static readonly Random _random = new Random();

        public string Type { get; set; }
        public string Meaning { get; set; }
        public string Root { get; set; }
        public string RootType { get; set; }
        public string RootMeaning { get; set; }

        public List<string> GetMeaningList()
        {
            if (string.IsNullOrWhiteSpace(Meaning))
                return new List<string>();

            return Meaning
                .Split(',')
                .Select(m => m.Trim())
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .ToList();
        }

        public string GetRandomMeaning()
        {
            var list = GetMeaningList();
            if (list.Count == 0)
                return string.Empty;

            return list[_random.Next(list.Count)];
        }
    }
}