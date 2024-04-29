using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    private List<Card> allCards;
    private Card flippedCard;
    private bool isFlipping = false;
    private List<Health> allHealths;

    [SerializeField]
    private Slider timeoutSlider;

    [SerializeField]
    private TextMeshProUGUI timeoutText;

    [SerializeField]
    private TextMeshProUGUI gameOverText;

    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private GameObject gamePausePanel;
    

    private bool isGameOver = false;
    private bool isGamePaused = false;

    //보스 체력
    public int bossHp = 20;


    [SerializeField]
    private float timeLimit = 60f;
    private float currentTime;
    private int totalMatches = 10;
    private int matchesFound = 0;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Board board  = FindObjectOfType<Board>();
        allCards = board.GetCards();

        currentTime = timeLimit;
        SetCurrentTimeText();
        StartCoroutine("FlipAllCardsRoutine");
    }

    void SetCurrentTimeText() {
        int timeSec = Mathf.CeilToInt(currentTime);
        timeoutText.SetText(timeSec.ToString());
    }

    IEnumerator FlipAllCardsRoutine() {
        isFlipping = true;
        yield return new WaitForSeconds(0.5f);
        FlipAllCards();
        yield return new WaitForSeconds(3f);
        FlipAllCards();
        yield return new WaitForSeconds(0.5f);
        isFlipping = false;

        yield return StartCoroutine("CountDownTimerRoutine");
    }

    IEnumerator CountDownTimerRoutine() {
        while (currentTime > 0) {
            currentTime -= Time.deltaTime;
            timeoutSlider.value = currentTime / timeLimit;
            SetCurrentTimeText();
            yield return null;
        }

        GameOver(false);
    }

    void FlipAllCards() {
        foreach (Card card in allCards) {
            card.FlipCard();
        }
    }

    public void CardClicked(Card card) {
        if(isGamePaused || isFlipping || isGameOver) {
            return;
        }

        card.FlipCard();

        if(flippedCard == null) {
            flippedCard = card;
        } else {
            StartCoroutine(CheckMatchRoutine(flippedCard, card));
        }
    }

    IEnumerator CheckMatchRoutine(Card card1, Card card2) {
        isFlipping = true;

        if(card1.cardID == card2.cardID) {
            // Debug.Log("Same Card!!!");
            card1.SetMatched();
            card2.SetMatched();
            matchesFound++;

            RestartTimer();

            if (matchesFound == totalMatches) {
                GameOver(true);
            }
        } else {
            //Health health = GameObject.Find("Health");//GetComponent<Health>();
            DecreaseHealth();
            // Debug.Log("Different Card!!!");
            yield return new WaitForSeconds(1f);

            card1.FlipCard();
            card2.FlipCard();

            StopCoroutine("CountDownTimerRoutine");
            yield return new WaitForSeconds(0.4f);
            
            RestartTimer();

            if(isGameOver)
                StopCoroutine("CountDownTimerRoutine");
        }

        isFlipping = false;
        flippedCard = null;
    }

    void RestartTimer()
    {
        // 현재 타이머를 다시 설정하고 코루틴을 중지하고 다시 시작
        currentTime = timeLimit;
        StopCoroutine("CountDownTimerRoutine");
        StartCoroutine("CountDownTimerRoutine");
    }

    void GameOver(bool success) {

        if(!isGameOver) {
            isGameOver = true;
            StopCoroutine("CountDownTimerRoutine");

            if(success) {
                gameOverText.SetText("Great Job!!!");
            } else {
                gameOverText.SetText("Game Over!!!");
            }

            Invoke("ShowGameOverPanel", 2f);
        }
    }

    void ShowGameOverPanel() {
        gameOverPanel.SetActive(true);
    }

    public void Restart() {
        Time.timeScale = 1;
        isGamePaused = false;
        SceneManager.LoadScene("MainScene");
    }

    public void Resume() {
        gamePausePanel.SetActive(false);
        isGamePaused = false;
        Time.timeScale = 1;
    }

    public void pauseBtnClicked() {
        if(isGameOver)
            return;
        Debug.Log("pauseBtnClicked!!");
        isGamePaused = true;

        Time.timeScale = 0;
        gamePausePanel.SetActive(true);

    }

    public void BackTitle() {
        Time.timeScale = 1;
        SceneManager.LoadScene("StartScene");
    }

    public void gameStart()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void DecreaseHealth() {
        Debug.Log("DecreaseHealth Called!");
        Board board = FindObjectOfType<Board>();

        if(board.healthCount <= 1) {
            GameOver(false);
        }

        allHealths = board.GetHealths();

        Debug.Log("allHealths.Count?" + allHealths.Count);
        
        //allHealths[4].unfilledHealthSprite();
        // foreach (Health health in allHealths) {
            // health.healthRenderer.sprite = health.unfilledHealthSprite;
        // }


        allHealths[board.healthCount-1].healthRenderer.sprite = allHealths[board.healthCount-1].unfilledHealthSprite;
        board.healthCount -= 1;
    }
}
