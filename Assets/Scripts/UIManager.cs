using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // ============ UI =================
    // Score in Game
    public int score = 0;
    // Text of UI
    //public Text scoreText;
    public TextMeshProUGUI scoreText;
    // Text of Final Score
    //public Text finalScore;
    public TextMeshProUGUI finalScoreText;
    // tips Text
    public TextMeshProUGUI tipsText;

    // GameObject to Show "Game Over" when the game ends

    // Restart button
    public Button restartButton;

    // Start is called before the first frame update
    void Start()
    {
        // subscribe the delegate of "RestartButton.onClick"
        restartButton.onClick.AddListener(OnRestartButtonDown);
        scoreText.enabled = true;
        Destroy(tipsText, 5f); // use Coroutine
    }

    void OnRestartButtonDown()
    {
        // Click restart button to reload the scene
        SceneManager.LoadScene(0);
    }

    public void OnAddScore(int lastReward)
    {
        score += lastReward;
        scoreText.text = "Score:  " + score;
    }

    /// <summary>
    /// End the game
    /// </summary>
    public void OnGameOver()
    {
        scoreText.enabled = false;
        finalScoreText.text = "Final Score:  " + score;
        restartButton.gameObject.SetActive(true);
        finalScoreText.gameObject.SetActive(true);
    }
}
