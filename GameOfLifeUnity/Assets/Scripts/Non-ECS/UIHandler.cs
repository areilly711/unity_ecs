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
        [SerializeField] GameHandler m_GameHandler;
        [SerializeField] TMP_InputField widthField;
        [SerializeField] GameObject gridPanel;
        [SerializeField] GameObject startPanel;
        [SerializeField] GameObject controlsPanel;
        [SerializeField] Toggle m_PlayToggle;
        [SerializeField] Button m_NextCycleButton;

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