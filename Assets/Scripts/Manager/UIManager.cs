using TMPro;
using UnityEngine;

namespace Manager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [SerializeField] private TextMeshProUGUI baseHealthText;
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private GameObject startGamePanel;
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject losePanel;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void OnStartGameClicked()
        {
            DeactivateAllPanels();
            GameManager.Instance.Init();
            hudPanel.SetActive(true);
        }

        public void UpdateWave(int current, int total)
        {
            if (waveText != null)
                waveText.text = $"Wave: {current} / {total}";
        }

        public void UpdateBaseHealth(int current, int max)
        {
            if (baseHealthText != null)
                baseHealthText.text = $"Base Health: {current} / {max}";
        }

        public void HandleLoseGame()
        {
            DeactivateAllPanels();
            losePanel.SetActive(true);
        }

        private void DeactivateAllPanels()
        {
            startGamePanel.SetActive(false);
            hudPanel.SetActive(false);
            losePanel.SetActive(false);
        }
    }
}