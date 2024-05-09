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
    private TextMeshProUGUI comboCountText;

    [SerializeField]
    private TextMeshProUGUI roundCountText;

    [SerializeField]
    private Slider bossHealthSlider;

    [SerializeField]
    private TextMeshProUGUI gameOverText;

    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private GameObject gamePausePanel;
    [SerializeField]
    private TextMeshProUGUI maxComboText;

    public int round;

    private bool isGameOver = false;
    private bool isGamePaused = false;

    public bool perfectEnable = false;

    //보스 체력
    public int bossHpCur = Boss.bossHealthCur;
    public int bossHpMax = Boss.bossHealthMax;

    //콤보, 대미지
    private int combo = 0;
    private int maxComboCount = 0;
    private int damage;
    private int comboPerRound = 0;

    [SerializeField]
    private float timeLimit = 5f;
    private float currentTime;
    private int totalMatches = 6;
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
        SetRoundCountText(round);
        StartCoroutine("FlipAllCardsRoutine");
        CountDownTimerRoutine();
    }

    void SetCurrentTimeText() {
        int timeSec = Mathf.CeilToInt(currentTime);
        timeoutText.SetText(timeSec.ToString());
    }

    void SetComboCountText(int comboCount) {
        comboCountText.SetText(comboCount.ToString() + " Combo");
        if(comboCount > maxComboCount)
            SetMaxComboText(comboCount);
    }

    void SetMaxComboText(int comboCount) {
        maxComboCount = comboCount;
        maxComboText.SetText("Max Combo: " + maxComboCount.ToString());
    }

    void SetRoundCountText(int roundCount) {
        roundCountText.SetText("Round " + roundCount.ToString());
    }

    IEnumerator FlipAllCardsRoutine() {
        isFlipping = true;
        SetRoundCountText(round);
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

        DecreaseHealth();
        Board board  = FindObjectOfType<Board>();
        if(board.healthCount > 0) {
            RestartTimer();
        } else {
            GameOver(false);
        }
    }

    public void updateBossHealthBar()
    {
        //Debug.Log("bossHealthCur: " + Boss.bossHealthCur + "bossHealthMax: " +Boss.bossHealthMax);
        bossHealthSlider.value = (float)((float)Boss.bossHealthCur / (float)Boss.bossHealthMax);
        //Debug.Log("bossHealthSlider.value: " + bossHealthSlider.value);
    }

    void FlipAllCards() {
        foreach (Card card in allCards) {
            if (card != null) {
                SetRoundCountText(round);
                card.FlipCard();
            }
        }
    }

    public void CardClicked(Card card) {
        if(isGamePaused || isFlipping || isGameOver) {
            return;
        }

        card.FlipCard();

        if(flippedCard == null) {
            flippedCard = card;
            SetRoundCountText(round);
        } else {
            StartCoroutine(CheckMatchRoutine(flippedCard, card));
            SetRoundCountText(round);
        }
    }

    IEnumerator CheckMatchRoutine(Card card1, Card card2) {
        isFlipping = true;

        if(card1.cardID == card2.cardID) {

            // Debug.Log("Same Card!!!");
            card1.SetMatched();
            card2.SetMatched();
            matchesFound++;
            combo++;
            comboPerRound++;
            SetRoundCountText(round);
            SetComboCountText(combo);

            RestartTimer();

            if (matchesFound == totalMatches) {
                SetRoundCountText(round);

                if (perfectEnable && comboPerRound == 6) {
                    Boss.bossHealthCur -= combo * 2;
                    comboPerRound = 0;
                }
                else Boss.bossHealthCur -= combo;

                //updateBossHealthBar();

                yield return new WaitForSeconds(0.5f);

                Debug.Log("Boss.bossHealthCur: " + Boss.bossHealthCur + ", combo: " + combo);
                if(Boss.bossHealthCur > 0) {
                    //refresh card 
                    Board board  = FindObjectOfType<Board>();
                    board.ResetBoard();

                    //init timer and stop timer
                    currentTime = timeLimit;
                    StopCoroutine("CountDownTimerRoutine");

                    //flip all cards
                    SetRoundCountText(round);
                    allCards = board.GetCards();
                    yield return new WaitForSeconds(0.5f);
                    FlipAllCards();
                    yield return new WaitForSeconds(3.15f - (0.15f * round));
                    Debug.Log("Waiting : " + (3.15f - (0.15f * round)));
                    FlipAllCards();
                    yield return new WaitForSeconds(0.5f);

                    matchesFound = 0;
                    SetComboCountText(combo);
                    SetRoundCountText(round);

                    //refresh health
                    allHealths = board.GetHealths();
                    for(int i = 0; i < 5; i++) {
                        allHealths[i].healthRenderer.sprite = allHealths[i].filledHealthSprite;
                    }
                    board.healthCount = 5;
                    SetRoundCountText(round);
                    //start timer
                    StartCoroutine("CountDownTimerRoutine");
                }
                else {
                    GameOver(true);
                }
            }

        } else {
            //Health health = GameObject.Find("Health");//GetComponent<Health>();
            DecreaseHealth();
            //Debug.Log("Boss.bossHealthCur: " + Boss.bossHealthCur + ", combo: " + combo);
            Boss.bossHealthCur -= combo;
            //updateBossHealthBar();
            SetRoundCountText(round);
            combo = 0;
            comboPerRound--;
            SetComboCountText(combo);
            //Debug.Log(Boss.bossHealthCur);
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
        //Debug.Log("pauseBtnClicked!!");
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
        //Debug.Log("DecreaseHealth Called!");
        Board board = FindObjectOfType<Board>();

        if(board.healthCount <= 1) {
            GameOver(false);
        }

        allHealths = board.GetHealths();

        //Debug.Log("allHealths.Count?" + allHealths.Count);
        
        //allHealths[4].unfilledHealthSprite();
        // foreach (Health health in allHealths) {
            // health.healthRenderer.sprite = health.unfilledHealthSprite;
        // }


        allHealths[board.healthCount-1].healthRenderer.sprite = allHealths[board.healthCount-1].unfilledHealthSprite;
        board.healthCount -= 1;
    }

    void RestartTimer()
    {
        // 현재 타이머를 다시 설정하고 코루틴을 중지하고 다시 시작
        currentTime = timeLimit;
        StopCoroutine("CountDownTimerRoutine");
        StartCoroutine("CountDownTimerRoutine");
    }
}