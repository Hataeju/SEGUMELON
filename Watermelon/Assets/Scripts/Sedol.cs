using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Sedol : MonoBehaviour
{
    const float Create_Position_Y = 8f;

    Camera mainCamera;

    [Header("���콺 ������� ���� (���� ���� ������ �����)")]
    [Range(0, 1)]
    [SerializeField] float mouseSensitivity = 0.5f;

    [Header("�������� �ӵ� (���� ���� ������ ���)")]
    [Range(0, 1)]
    [SerializeField] float mergeSpeed = 0.5f;

    [Header("��밡 ������ ���� �ð�")]
    [SerializeField] private float absorbTime = 0.2f;

    [Header("�ִϸ��̼� ��� �ð�")]
    [SerializeField] private float playAniamtion = 0.3f;

    private WaitForSeconds waitAbsorb;
    private WaitForSeconds waitAnimation;


    public bool isDrag;
    public Rigidbody2D circleRigidbody;

    public int level;
    public int max = 0;
    Animator animator;

    public bool isMerge;
    CircleCollider2D circlecollider;

    public ParticleSystem effect;

    private float stayTime;
    SpriteRenderer spriteRenderer;

    private WaitForSeconds waitTouch;
    private bool isTouch;

    private void Awake()
    {
        mainCamera = Camera.main;
        circleRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        circlecollider = GetComponent<CircleCollider2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        waitAbsorb = new WaitForSeconds(absorbTime);
        waitAnimation = new WaitForSeconds(playAniamtion);
        waitTouch = new WaitForSeconds(0.2f);
    }
    // 1. ���콺 Ŀ���� �����ٴϴ� ������Ʈ => �޴��� ���忡�� �հ��� ��ġ�� ������ �ȴ�.
    //
    // 2. 

    private void OnEnable()
    {
        animator.SetInteger("Level", level);
    }
    // Update is called once per frame
    void Update()
    {
        if (isDrag)
        {
            // Camera.main : ī�޶� �� ����ī�޶� �±׸� �ް� �ִ� ��
            // ScreenToWorldPoint : ����Ƽ Ư�� �� �� ������ ���� �ݸ��Ǿ� �ֱ� ������ ��ũ�� ����Ʈ�� ��ǥ�� ���� ��ǥ��� �������ּ���.
            // ScreenPoint : ȭ�� ���� ���� (UI����)
            // WorldPoint : ���� ������ ����Ǵ� ���� (���� ������Ʈ)
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // ��輱 ����
            float borderLeft = -4.2f + transform.localScale.x / 2;
            float borderRight = 4.2f - transform.localScale.x / 2;


            mousePosition.y = Create_Position_Y;
            mousePosition.z = 0;        // ���� ī�޶��� z���� -10�̱� ������ ȭ�鿡 ������ �ʴ´�
                                        //transform.position = mousePosition;
            if (mousePosition.x < borderLeft)
            {
                mousePosition.x = borderLeft;
            }
            else if (mousePosition.x > borderRight)
            {
                mousePosition.x = borderRight;
                //transform.position = new Vector3(borderRight, mousePosition.y, mousePosition.z);
            }

            //���콺�� �ε巴�� ���������
            // Lerp �� ������ ��������
            // Lerp(a, b, t) : a����, b����, �ð�(0~1) 1�� ����������� ����, 0�� 1 �ΰ��� �ǹ̰� ���� ����� ���´�.
            transform.position = Vector3.Lerp(transform.position, mousePosition, mouseSensitivity);
        }
    }

    // ���� �浹 ���� �� ȣ��Ǵ� ����Ƽ �̺�Ʈ �Լ�
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Sedol"))
        {
            // ������ ������Ʈ�� �����´�.
            Sedol other = collision.gameObject.GetComponent<Sedol>();

            // ��ü �߿� �ٸ� ��Ŭ�� ���� ����
            // ��ü�� 1:1�� �̷������ �Ѵ�.
            // �츮�� ���� ��Ȳ������ �ƽ������� 7�̴�.
            if (level.Equals(other.level) && !isMerge && !other.isMerge && level < 7)
            {
                // ��ü�� �� ������ ����ϴ� ǥ���ϱ����� ��ġ�� �ʿ��ϴ�.
                // ���� ������ ��ġ�� �����´�.
                float x = transform.position.x;
                float y = transform.position.y;

                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;

                // ���� y���� �ٸ��ٸ� ���� �ִ� ģ���� �Ʒ��� �������� ���
                // ���� y���� �ٸ��ٸ� ������ ������ ���

                /*if(y > otherY)
                {
                    {
                        x = otherX;
                        y = otherY;
                    }
                }
                else if(otherY > y)
                {
                    otherX = x;
                    otherY = y;
                }
                else
                {
                    if(x > otherX)
                    {
                        otherX = x;
                    }
                    if(otherX > x)
                    {
                        x = otherX;
                    }
                }  */

                if (y < otherY || (y.Equals(otherY) && x > otherX))
                {
                    // ������ ������ ���
                    other.Absorb(transform.position);
                    // ���� ������
                    LevelUp();
                }
            }
        }
    }

    // ��밡 ������ ����Ǵ� �Լ�
    public void Absorb(Vector3 target)
    {
        isMerge = true;
        circleRigidbody.simulated = false;
        circlecollider.enabled = false;
        if(GameManager.Instance.isGameOver) PlayEffect();
        StartCoroutine(Move(target));
    }

    // ���� ��ġ�� �̵��ϴ� �ڷ�ƾ
    private IEnumerator Move(Vector3 target)
    {
        int frame = 0;
        while (frame < 20)
        {
            frame++;
            transform.position = Vector3.Lerp(transform.position, target, mergeSpeed);
            yield return null; // ���������� ����
        }
        isMerge = false;
        gameObject.SetActive(false);

        // ������ ����Ͽ� ���� �ο�
        // �� ���� ���� ������ ������ ȿ���� �ְ�;���
        GameManager.Instance.score += (int)Mathf.Pow(2, level);

        UIManager.Instance.tmpScore.text = GameManager.Instance.score.ToString();

    }

    /// <summary>
    /// ������ �Լ� 
    /// </summary>
    private void LevelUp()
    {
        isMerge = true;

        // ��ġ�� ���� �ӷ±� ȸ���� 0���� �ؾ��Ѵ�.
        circleRigidbody.velocity = Vector2.zero;
        circleRigidbody.angularVelocity = 0;

        StartCoroutine(NextLevel());

    }


    // ���� ���� �ڷ�ƾ
    private IEnumerator NextLevel()
    {
        // ��밡 ������ ����Ǵ� �ð����� ��ٸ�
        yield return waitAbsorb;

        animator.SetInteger("Level", level + 1);
        PlayEffect();
        SoundManager.Instance.PlaySFX(SFX.LevelUP);
        // �ִϸ��̼� ��̽ð� ���� ��ٸ���.
        yield return waitAnimation;

        level = level + 1;
        /*if(level > GameManager.Instance.maxLevel)
        {
            GameManager.Instance.maxLevel = level;
        }*/

        // �Ŵ����� �ƽ������� �ùٸ� ���� �����
        // Math.Max(a,b); a,b�� ���� �� ����
        GameManager.Instance.maxLevel = Mathf.Max(level, GameManager.Instance.maxLevel);

        isMerge = false;
    }

    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag = false;
        circleRigidbody.simulated = true;
    }

    //����Ʈ ��� �Լ�
    private void PlayEffect()
    {
        // ����Ʈ�� �������� ���� ��ġ��
        effect.transform.position = transform.position;
        // ����Ʈ�� ũ�⸦ ���� �����Ϸ�
        effect.transform.localScale = transform.localScale;
        // ����Ʈ ���
        effect.Play();
    }

    //������ ��� 2�ʰ� ����ϸ� ���������� ���, 

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Finish"))
        {
            stayTime += Time.deltaTime;
            if (stayTime >= 5f)
            {
                GameManager.Instance.Gameover();
                
            }
            else if(stayTime > 2f)
            {
                spriteRenderer.color = Color.red;
                
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
        {
            spriteRenderer.color = Color.white;
            stayTime = 0;
        }
    }

    private void OnCollisonEnter2D(Collision2D collision)
    {
        StartCoroutine(TouchSound());  
    }

    private IEnumerator TouchSound()
    {
        if (isTouch) yield break;   // yield break �ڷ�ƾ Ż��

        isTouch = true;
        SoundManager.Instance.PlaySFX(SFX.Touch);

        yield return waitTouch;
        isTouch = false;

    }







    // id,pw �Է�
    // �α��� ��ư Ŭ��
    // �������� ����
    // �������� �˻���
    // �������� ok or no sign �ش�
    // Ŭ�󿡼� ������ ���� �����޽��� �Ǵ� �α��� ����
}
