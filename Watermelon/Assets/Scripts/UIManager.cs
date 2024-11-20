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

    [Header("����")]
    public TextMeshProUGUI tmpScore;
    [SerializeField] private TextMeshProUGUI tmpBestScore;




    [Header(" ���� ���� ���� ")]
    public GameObject gameoverUI;
    public TextMeshProUGUI tempGameoverScore;
    [SerializeField] private Button buttonRestart;

    [Header("���� ���� ����")]
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


        // ��ư�� ��� �����ϴ� �ڵ� => ���� �ѹ��� ȣ���ϸ� �ȴ�.
        // AddListener()�� �Լ� �Ҵ� ����

        // 1.
        //buttonRestart.onClick.AddListener(TEST);

        // 2. ���ٽ� : �ĺ��ڰ� ���� ������ �Լ��� ����� ��
        buttonRestart.onClick.AddListener(() => { StartCoroutine(Restart()); });

        buttonStart.onClick.AddListener(GameStart);
    }

    // Update is called once per frame
    void Update()
    {
        //��������
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
