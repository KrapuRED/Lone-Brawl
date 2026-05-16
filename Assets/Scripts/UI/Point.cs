using UnityEngine;
using TMPro;
using UnityEngine.Rendering; //required for text mesh pro canvas

public class Point : MonoBehaviour
{
    private int _totalPoints = 0;
    private TextMeshProUGUI _scoreText;

    void Awake()
    {
        _scoreText = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        // we subscribe to any Enemy using the class (needed static keyword on the broadcast)
        Enemy.OnEnemyDeath += AddPoints;
    }

    private void AddPoints(int value)
    {
        _totalPoints += value;
        UpdateText();
    }

    private void UpdateText()
    {
        _scoreText.text = "Point: " + _totalPoints.ToString();
    }

    void OnDestroy()
    {
        Enemy.OnEnemyDeath -= AddPoints;
    }
}
