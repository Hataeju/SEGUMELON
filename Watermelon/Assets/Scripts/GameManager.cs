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




    public Sedol lastSedol;

    [SerializeField] private GameObject sedolPrefab;
    [SerializeField] private Transform sedolGroup;

    private WaitForSeconds waitNextCircle;

    public int maxLevel;


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
        NextSedol();
    }

    private void NextSedol()
    {
        Sedol newSedol = GetSedol();
        lastSedol = newSedol;

        lastSedol.level = Random.Range(0, maxLevel);
        lastSedol.gameObject.SetActive(true);

        // �ڷ�ƾ�� ȣ�� ����
        StartCoroutine(WaitLastSedolNull());
        //StartCoroutine("WaitLastSedolNull");
    }

    private Sedol GetSedol()
    {
        // instantiate ���� ������Ʈ�� �����ϴ� �ڵ�
        GameObject temp = Instantiate(sedolPrefab, sedolGroup);
        Sedol sedol = temp.GetComponent<Sedol>();
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
}
