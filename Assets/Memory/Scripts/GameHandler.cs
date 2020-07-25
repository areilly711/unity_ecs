using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Memory
{
    public class GameHandler : MonoBehaviour
    {
        public MatchingGrid m_matchingGrid;

        public void ResetGame()
        {
            Instantiate(m_matchingGrid);
        }
    }
}
