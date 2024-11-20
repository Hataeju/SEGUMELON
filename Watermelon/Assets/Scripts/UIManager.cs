using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    private static UIManager instance = null;
    public static UIManager Instance { get { return instance; } }

    [Header("점수")]
    public TextMeshProUGUI tmpScore;
    [SerializeField] private TextMeshProUGUI tmpBestScore;




    [Header(" 게임 오버 관련 ")]
    public GameObject gameoverUI;
    public TextMeshProUGUI tempGameoverScore;
    [SerializeField] private Button buttonRestart;

    [Header("게임 시작 관련")]
    [SerializeField] private GameObject gameStartUI;
    [SerializeField] private GameObject borderLine;
    [SerializeField] private GameObject bottom;
    [SerializeField] private Button buttonStart;

    private WaitForSeconds waitButtonClick = new WaitForSeconds(0.5f);

    private void Awake()
    {
        if (instance == null) instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

        if (!PlayerPrefs.HasKey("BestScore"))
        {
            PlayerPrefs.SetInt("BestScore", 0);
        }

        tmpBestScore.text = PlayerPrefs.GetInt("BestScore").ToString();


        // 버튼에 기능 연결하는 코드 => 최초 한번만 호출하면 된다.
        // AddListener()에 함수 할당 가능

        // 1.
        //buttonRestart.onClick.AddListener(TEST);

        // 2. 람다식 : 식별자가 없는 무명의 함수를 만드는 것
        buttonRestart.onClick.AddListener(() => { StartCoroutine(Restart()); });

        buttonStart.onClick.AddListener(GameStart);
    }

    // Update is called once per frame
    void Update()
    {
        //합쳐질때
        //tmpScore.text = GameManager.Instance.score.ToString();
    }
    private void GameStart()
    {
        borderLine.SetActive(true);
        bottom.SetActive(true);

        tmpScore.gameObject.SetActive(true);
        tmpBestScore.gameObject.SetActive(true);

        gameStartUI.SetActive(false);

        SoundManager.Instance.PlaySFX(SFX.Button);

        tmpScore.text = "0";

        GameManager.Instance.NextSedol();
    }
    private void TEST()
    {
        StartCoroutine(Restart());
    }
    IEnumerator Restart()
    {
        SoundManager.Instance.PlaySFX(SFX.Button);

        yield return waitButtonClick;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
