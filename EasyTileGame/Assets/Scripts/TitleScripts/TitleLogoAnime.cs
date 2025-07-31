using UnityEngine;

public class TitleLogoAnime : MonoBehaviour
{
    [SerializeField] private GameObject heartbeatObj;
    [SerializeField] private GameObject carnivalObj;

    private Animator heartbeatAnimator;
    private Animator carnivalAnimator;

    private bool isFalseHeartbeatObj = false;   // heartbeatObj�� ��Ȱ��ȭ���� �����ϴ� ����
    private bool isFalsecarnivalObj = false;    // carnivalObj�� ��Ȱ��ȭ���� �����ϴ� ����

    private bool isLogoEffect = true; // �ΰ� ����Ʈ�� ���� �Լ��� ������ �� �ִ� ���� 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        // ĳ��
        heartbeatAnimator = heartbeatObj.GetComponent<Animator>();
        carnivalAnimator = carnivalObj.GetComponent<Animator>();
        // �� ���� �ڱ� �ּ� ������
        heartbeatObj.GetComponent<TitleHeartbeat>().InitAnimeScript(this);
        carnivalObj.GetComponent<TitleCarnival>().InitAnimeScript(this);
    }

	private void Start()
	{
        isLogoEffect = true;
        heartbeatObj.SetActive(true);
	}

    // Ÿ��Ʋ�ΰ� �ִϸ��̼� Ư�� �����ӿ��� Ȯ�������� Ÿ��Ʋ �ΰ� ȿ���� �ش�.
    public void ThinkCGHeartbeatLogo()
    {
        if (!isLogoEffect) { return;  }

        int r = Random.Range(0, 10);
        // 20% Ȯ���� carnival ������ Ȱ��ȭ�ϰ�, heartbeat ������ �����.
        if (r > 7)
        {
            heartbeatAnimator.SetTrigger("IsDisappear");
            carnivalObj.SetActive(true);
        }
        // 30% Ȯ���� �۸�ġ ȿ���� ����Ų��.
        else if (r > 4)
        {
			heartbeatAnimator.SetTrigger("IsGlitch");
		}
        // 50% Ȯ���� �״�� ����.
        else
        {
            return;
        }
    }
    // Ÿ��Ʋ �ΰ� �� heartbeat�� ��Ȱ��ȭ��Ų��.
    public void ActiveFalseHeartbeatLogo()
    {
        if (!isLogoEffect) { return; }

        heartbeatObj.SetActive(false);
        if (!carnivalObj.activeSelf)
        {
            carnivalObj.SetActive(true);
        }
    }

    public void ThinkCGCarnivalLogo()
    {
        if (!isLogoEffect) { return; }

        int r = Random.Range(0, 10);
        // 20% Ȯ���� heartbeat ������ Ȱ��ȭ�ϰ�, carnival ������ �����.
        if (r > 7)
        {
            carnivalAnimator.SetTrigger("IsDisappear");
            heartbeatObj.SetActive(true);
        }
        // 30% Ȯ���� �۸�ġ ȿ���� ����Ų��.
        else if (r > 4)
        {
            carnivalAnimator.SetTrigger("IsGlitch");
        }
        // 50% Ȯ���� �״�� ����.
        else
        {
            return;
        }
    }
    // Ÿ��Ʋ �ΰ� �� carnival�� ��Ȱ��ȭ��Ų��.
    public void ActiveFalseCarnivalLogo()
    {
        if (!isLogoEffect) { return; }

        carnivalObj.SetActive(false);
        if (!heartbeatObj.activeSelf)
        {
            heartbeatObj.SetActive(true);
        }
    }

    public void DisappearTitleLogo()
    {
        isLogoEffect = false;

		carnivalAnimator.SetTrigger("IsDisappear");
        heartbeatAnimator.SetTrigger("IsDisappear");

        Invoke("SetActiveFalseLogo", 3f);
    }
    private void SetActiveFalseLogo()
    {
        carnivalObj.SetActive(false);
        heartbeatObj.SetActive(false);
    }
}
