using UnityEngine;
using TMPro;


public class GameManager : MonoBehaviour
{

    public int score = 0;
    [SerializeField] TextMeshProUGUI scoreText;
    public void addScore(int point)
    {
        score += point;
        updateScore();
    }
    private void updateScore()
    {
        scoreText.text = score.ToString();

    }
}
