using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using System.IO; 
using UnityEngine.UI; 

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI currentWordText;
    public TextMeshProUGUI wordChainText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TMP_InputField playerInput;
    public Button startButton;
    public GameObject gameOverPanel; 

    // --- Biến logic game ---
    private List<string> dictionary = new List<string>(); 
    private List<string> usedWords = new List<string>(); 
    private string currentWord;
    private float timeRemaining; 
    private bool isPlaying = false; 
    private int score = 0;

    private const float TURN_TIME = 10.0f; 

    void Start()
    {
        LoadDictionary();
        startButton.onClick.AddListener(StartGame); 
        playerInput.onSubmit.AddListener(OnSubmitWord);
        gameOverPanel.SetActive(false); 
    }

    void Update()
    {
        if (!isPlaying) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = $"Thời gian: {Mathf.CeilToInt(timeRemaining)}s";
        }
        else
        {           
             GameOver("Hết giờ!");
        }
    }

void LoadDictionary()
{

    string filePath = Path.Combine(Application.streamingAssetsPath, "New1.txt");

    if (File.Exists(filePath))
    {
        string[] words = File.ReadAllLines(filePath);

        foreach (string line in words)
        {
            string word = line.ToLower().Trim();
            if (!string.IsNullOrEmpty(word))
            {
                dictionary.Add(word);
            }
        }
        Debug.Log($"Đã tải thành công {dictionary.Count} từ vào từ điển.");
    }
    else
    {
        Debug.LogError($"LỖI: Không tìm thấy file từ điển tại: {filePath}");
        currentWordText.text = "LỖI: Không tải được từ điển!";
    }
}

    public void StartGame()
    {
        isPlaying = true;
        usedWords.Clear();
        score = 0;
        gameOverPanel.SetActive(false);
        playerInput.gameObject.SetActive(true);
        playerInput.interactable = true;

        currentWord = dictionary[Random.Range(0, dictionary.Count)];
        usedWords.Add(currentWord);

        currentWordText.text = $"Từ bắt đầu: {Capitalize(currentWord)}";
        wordChainText.text = Capitalize(currentWord);
        scoreText.text = "Điểm: 0";

        NextTurn();
    }

    private void OnSubmitWord(string submittedWord)
    {
        if (!isPlaying || string.IsNullOrWhiteSpace(submittedWord)) return;

        string finalWord = submittedWord.ToLower().Trim();

        if (IsValidWord(finalWord))
        {
            currentWord = finalWord;
            usedWords.Add(currentWord);
            score++;

            scoreText.text = $"Điểm: {score}";
            wordChainText.text += $" → {Capitalize(currentWord)}";

            NextTurn();
        }

        playerInput.text = "";
        playerInput.ActivateInputField();
    }

    private bool IsValidWord(string word)
    {
        if (!dictionary.Contains(word))
        {
            GameOver($"Từ '{word}' không có trong từ điển!");
            return false;
        }

        if (usedWords.Contains(word))
        {
            GameOver($"Từ '{word}' đã được sử dụng!");
            return false;
        }

        string lastSyllableOfCurrent = GetLastSyllable(currentWord);
        string firstSyllableOfNew = GetFirstSyllable(word);
        if (lastSyllableOfCurrent != firstSyllableOfNew)
        {
            GameOver($"Từ '{word}' phải bắt đầu bằng '{lastSyllableOfCurrent}'!");
            return false;
        }

        return true;
    }

    void NextTurn()
    {
        timeRemaining = TURN_TIME;
        currentWordText.text = $"Nối tiếp từ: {Capitalize(currentWord)}";
    }

    void GameOver(string reason)
    {
        isPlaying = false;
        playerInput.interactable = false; 
        gameOverPanel.SetActive(true);
        gameOverPanel.GetComponentInChildren<TextMeshProUGUI>().text = $"Thua rồi!\n{reason}\nĐiểm cuối cùng: {score}";
        Debug.Log("Game Over: " + reason);
    }


    private string GetLastSyllable(string word)
    {
        string[] syllables = word.Split(' ');
        return syllables[syllables.Length - 1];
    }

    private string GetFirstSyllable(string word)
    {
        string[] syllables = word.Split(' ');
        return syllables[0];
    }

    private string Capitalize(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return char.ToUpper(s[0]) + s.Substring(1);
    }
}