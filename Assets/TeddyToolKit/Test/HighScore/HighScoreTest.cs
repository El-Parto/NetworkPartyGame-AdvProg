using System;
using System.Collections.Generic;
using UnityEngine;
using TeddyToolKit.HighScore;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TeddyToolKit.Test.HighScore
{
    
    public class HighScoreTest : MonoBehaviour
    {
        private List<Score> _highScore;
        [SerializeField] private Text textBox;
        
        public void RandomiseList()
        {
            if (_highScore.Count == _highScore.Capacity) _highScore.Clear();
            for (int i=0; i<_highScore.Capacity; i++)
            {
                var s = Random.Range(100, 1000);
                var score = new Score(s, $"NAME{s}");
                score += 10;
                score -= 10;
                _highScore.Add(score);
            }
        }

        public void Start()
        {
            Initialise();
        }

        public void Initialise()
        {
            // if (_highScore is null) _highScore = new List<Score>(10);
            // shorthand by rider:
            _highScore ??= new List<Score>(10);
            RandomiseList();
            //sort using IComparable           
            _highScore.Sort();
            DisplayText(); 
        }
        public void DisplayText()
        {
            string content = "";
            foreach (var score in _highScore)
            {
                content = $"{content}{score.ToString()}\n";
            }
            textBox.text = content;
        }
    }
}