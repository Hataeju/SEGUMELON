using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����������
// �̱��� : ���� ���߿��� ���� ���Ǵ� ����, ������ ��ü�� �����, ��𼭳� ������ �����ϵ��� �Ѵ�.
// static : �����ϰ� ����� , �׻� �޸𸮿� �¿��.
// �̱����� ���� ��ũ��Ʈ���� �����ؾ߸� �ϴ� �Ŵ����� ���� Ŭ�������� ����ϼ���.

public class GameManager : MonoBehaviour
{
    // �̱��� �ۼ� ���
    private static GameManager instance = null;
    public static GameManager Instance { get { return instance; } }

    [Header("��밡 ������ ���� �ð�")]
    [SerializeField] private float absorbTime = 0.2f;

    private WaitForSeconds inOrderTime;

    public Sedol lastSedol;

    [SerializeField] private GameObject sedolPrefab;
    [SerializeField] private Transform sedolGroup;
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private Transform effectGroup;

    private WaitForSeconds waitNextCircle;

    public int maxLevel;
    public int score;

    public ParticleSystem effect;

    public bool isGameOver;

    private void Awake()
    {
        if (instance == null) instance = this;

        // ������ ����(������� ���)
        Application.targetFrameRate = 60;
        waitNextCircle = new WaitForSeconds(1.5f);
    }
    // Start is called before the first frame update
    void Start()
    {
        inOrderTime = new WaitForSeconds(absorbTime * 0.5f);
        NextSedol();
    }

    private void NextSedol()
    {
        if (!isGameOver)
        {
            Sedol newSedol = GetSedol();
            lastSedol = newSedol;

            lastSedol.level = Random.Range(0, maxLevel);
            lastSedol.gameObject.SetActive(true);

            SoundManager.Instance.PlaySFX(SFX.Next);

            // �ڷ�ƾ�� ȣ�� ����
            StartCoroutine(WaitLastSedolNull());
            //StartCoroutine("WaitLastSedolNull");
        }
    }

    private Sedol GetSedol()
    {
        ParticleSystem effect = Instantiate(effectPrefab, effectGroup).GetComponent<ParticleSystem>();
        // instantiate ���� ������Ʈ�� �����ϴ� �ڵ�
        GameObject temp = Instantiate(sedolPrefab, sedolGroup);
        Sedol sedol = temp.GetComponent<Sedol>();
        sedol.effect = effect;
        return sedol;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void TouchDown()
    {   if( lastSedol == null)
        {
            return;
        }
        lastSedol.Drag();
    }


    public void TouchUp()
    {
        if (lastSedol == null)
        {
            return;
        }
        lastSedol.Drop();
        lastSedol = null;
    }

    // �ڷ�ƾ : ������ ������ �����Ͽ� ���� ������ �����ϰ� ���ư����� �����ִ� �����ƾ�̴�.
    // �� �� ��ٸ��� �� ���� ���⵵ ��
    IEnumerator WaitLastSedolNull()
    {
        // �츮�� ��� �ϰ� ���� ��
        while (lastSedol != null)
        {
            yield return null; // �ڷ�ƾ������ return ��� yield return null;������Ѵ�
        }
        yield return waitNextCircle;
        NextSedol();
    }

    public void Gameover()
    {
        if (isGameOver) return;

        isGameOver = true;

        Debug.Log("���ӿ���!");

        // ���̾��Ű�� Ȱ��ȭ�� ��Ŭ ������Ʈ ã��
        // 1. Sedol[] sedols = FindObjectsOfType<Sedol>(); --> �޸� ���ٻ��

        // 2.
        Sedol[] sedols = sedolGroup.GetComponentsInChildren<Sedol>();
        for (int i = 0; i < sedols.Length; i++)
        {
            sedols[i].circleRigidbody.simulated=false;
        }
        
        // !gameObject.activeSelf ��Ȱ��

        StartCoroutine(InOrder(sedols));
    }

    private IEnumerator InOrder(Sedol[] sedols)
    {
        for (int i = 0; i < sedols.Length; i++)
        {
            // ���ӿ����ÿ��� �����̴� ȿ���� �ʿ�����Ƿ� ���� ��ġ�� �ѱ��.
            sedols[i].Absorb(sedols[i].transform.position);
            SoundManager.Instance.PlaySFX(SFX.Next);
            yield return inOrderTime;
        }

        SoundManager.Instance.PlaySFX(SFX.GameOver);
    }

}
