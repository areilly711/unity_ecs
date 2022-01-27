/*
The MIT License (MIT)
Copyright 2021 Adam Reilly, Call to Action Software LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;

#pragma warning disable CS0649 
namespace GameLife
{
    /// <summary>
    /// Handles UI interactions
    /// </summary>
    public class UIHandler : MonoBehaviour
    {
        [SerializeField] GameHandler m_GameHandler = null;
        [SerializeField] TMP_InputField widthField = null;
        [SerializeField] GameObject gridPanel = null;
        [SerializeField] GameObject startPanel = null;
        [SerializeField] GameObject controlsPanel = null;
        [SerializeField] Toggle m_PlayToggle = null;
        [SerializeField] Button m_NextCycleButton= null;

        private void Start()
        {
            widthField.text = m_GameHandler.GridSize.ToString();
            //heightField.text = m_GameHandler.GridHeight.ToString();

            gridPanel.SetActive(true);
            startPanel.SetActive(false);
            controlsPanel.SetActive(false);
            m_NextCycleButton.interactable = !m_PlayToggle.isOn;
        }

        public void SetGridWidth(string size)
        {
            int s = int.Parse(size);
            if (s > GameHandler.MaxSize)
            {
                widthField.text = GameHandler.MaxSize.ToString();
                return;
            }

            m_GameHandler.SetGridSize(s);
        }
        

        public void CreateGrid()
        {
            m_GameHandler.CreateGrid();
            gridPanel.SetActive(false);
            startPanel.SetActive(true);
        }

        public void StartGame()
        {
            m_GameHandler.StartGame();
            startPanel.SetActive(false);
            controlsPanel.SetActive(true);
        }

        public void Restart()
        {
            m_GameHandler.RestartGame();
        }

        public void NextCycle()
        {
            m_GameHandler.NextCycle();
        }

        public void PlayModeToggled(bool play)
        {
            if (play)
            {
                Time.timeScale = 1;
                m_NextCycleButton.interactable = false;
                m_PlayToggle.GetComponentInChildren<Text>().text = "Pause";
            }
            else
            {
                Time.timeScale = 0;
                m_NextCycleButton.interactable = true;
                m_PlayToggle.GetComponentInChildren<Text>().text = "Play";
            }
        }
    }
}
#pragma warning restore CS0649