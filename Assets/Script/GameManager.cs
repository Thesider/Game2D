using UnityEngine;
using TMPro;


public class GameManager : MonoBehaviour
{

    public int score = 0;
    [SerializeField] TextMeshProUGUI scoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateScore();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
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
