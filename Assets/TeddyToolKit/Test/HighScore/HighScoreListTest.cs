using System;
using System.Collections.Generic;
using UnityEngine;
using TeddyToolKit.HighScore;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TeddyToolKit.Test.HighScore
{
    
    /// <summary>
    /// Test script to show how to use the <see cref="TeddyToolKit.HighScore.HighScoreList" />
    /// 
    /// </summary>
    public class HighScoreListTest : MonoBehaviour
    {
        private HighScoreList _highScore;
        [SerializeField] private Text textBox;
        // [SerializeField] private int myScore = 555;
        [SerializeField] private Score newScore = new Score(555, "TED");
        // [SerializeField] private string myName = "TED";
        
        /// <summary>
        /// generate random score for demo purpose
        /// </summary>
        public void RandomiseList()
        {
            _highScore.Clear();
            for (int i=0; i<10; i++)
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

        /// <summary>
        /// initialise the list and randomise it for testing purpose
        /// </summary>
        public void Initialise()
        {
            _highScore = new HighScoreList();
            RandomiseList();
            //sort using insertion           
            _highScore.Sort();
            DisplayText(); 
        }
        
        /// <summary>
        /// display all the highscore as text.
        /// in real use, customise what to display
        /// </summary>
        /// <param name="withRank"></param>
        public void DisplayText(int withRank = -1)
        {
            string content = "";
            var i = 1;
            foreach (var score in _highScore)
            {
                if (withRank > 0 && (withRank == i))
                {
                    content = $"{content} You are rank {withRank} \n{score.ToString()}\n";
                }
                else
                {
                    content = $"{content}{score.ToString()}\n";
                }

                i++;
            }
            textBox.text = content;
        }

        /// <summary>
        /// get the rank of the last inserted score, in the high score list, after sorting
        /// </summary>
        public void GetMyRank()
        {
            var s = new Score(newScore._value, newScore._name);
            _highScore.Add(s);
            var r = _highScore.getRank(s);
            DisplayText(r);
        }
    }
}
