using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("�X�e�[�^�X")]
    [SerializeField, Tooltip("GameMNG")] private GameMNG GameMNG; // gameMng
    [SerializeField, Tooltip("�ړ����x")] private float moveForce = 5.0f;//�ړ����x

    [Header("���u���b�N")]
    [SerializeField, Tooltip("���u���b�N�Ȃ�`�F�b�N")] private bool bMirror=false;//�ړ����x
    [SerializeField, Tooltip("�΂ƂȂ�u���b�N")] private Block MirrorObj;//�ړ����x
    private float hitStopTime; // �q�b�g�X�g�b�v����
    private bool isHitStopping; // ���݃q�b�g�X�g�b�v�����ǂ���
    private bool hit;
    private bool bMove;
    private bool isLocked; // ������~�t���O
    private int Movenum;
    private Vector3 pPos;   //�v���C���[�̈ʒu
    private Vector3 bPos;   //���g�̈ʒu
    private Vector3 bScale; // ���g�̃T�C�Y
    private Rigidbody rb;
    private Vector3[] pushDir;    //�i�s�����i�z�񉻗\��j
    private Vector3 deltaMove; // ���߂̈ړ���

    [Header("���")]
    [SerializeField, Tooltip("�v���n�u")] public GameObject[] arrowPrefab = new GameObject[3];    // ���̃v���n�u
    private GameObject[] arrowInstance = new GameObject[3];
    private Arrow[] arrow = new Arrow[3];
    private Renderer renderer;
    private Vector3 worldScale; // �u���b�N�̃��[���h�X�P�[��

    [Header("�G�t�F�N�g")]
    [SerializeField] private GameObject stopEffectPrefab;  // �����̉���G�t�F�N�g
    [SerializeField] private GameObject SpeedEffectPrefab;  // ���x���G�t�F�N�g

    // ����(��) ����X�폜�\��
    [Header("����")]
    [SerializeField] private GameObject butlerPrefab;   // ����(��)�̃v���n�u
    private Animator butlerAnim; // �A�j���[�V�����؂�ւ��p(�G�l�~�[)

    const float destroyTime = 1.0f;
    private float timeCount = 0.0f;

    // �Q�[���p�b�h�֘A
    const float resetSec = 0.08f;   // �Q�[���p�b�h���������̑Ή��b��(80m�b)
    private float lastLBDownTime = -Mathf.Infinity;     // LB���Ō�ɉ���������
    private float lastRBDownTime = -Mathf.Infinity;     // RB
    private float lastYDownTime  = -Mathf.Infinity;     // Y
    private bool isYDown = false;           // Y(�����w��)�{�^������������

    private GameObject effect;      // �G�t�F�N�g�{��
    private FollowWorld follow;     // ���x���G�t�F�N�g�p
    private GameObject shituji;      // �����{�� 
    private Animator PlayerAnim;    // �A�j���[�V�����؂�ւ��p(�v���C���[)

    private bool bStartStop = false;
    private bool bStartMove_1 = false;
    private bool bStartMove_2 = false;

    // �O�ǂ̍��W�f�[�^
    float[,] wallPosition = new float[4, 2] {
        { 22.5f, -1.5f },       // �E
        { -22.5f,  -1.5f },     // ��
        { 0.0f,  15.0f },       // ��           
        { 0.0f, -18.0f }        // ��
    };

    void Start()
    {
        GameMNG = FindFirstObjectByType<GameMNG>();

        hit = false;
        // �u���b�N�̈ʒu
        bPos = transform.position;
        bScale = transform.localScale;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        pushDir = new Vector3[GameMNG.num];
        for (int i = 0; i < GameMNG.num; i++) { pushDir[i] = Vector3.zero; }

        hitStopTime = 0.05f;
        isHitStopping = false;

        bMove = false;
        Movenum = 0;

        isLocked = false;


        // ���
        for (int i = 0; i < GameMNG.num; i++)
        {
            arrowInstance[i] = Instantiate(arrowPrefab[i], bPos, Quaternion.identity);
            arrow[i] = arrowInstance[i].GetComponent<Arrow>();
        }
        renderer = GetComponent<Renderer>();
        worldScale = renderer.bounds.size;

        // ����(��)
        butlerPrefab = Instantiate(butlerPrefab, new Vector3(999.0f, 999.0f, 999.0f), Quaternion.identity);

        butlerAnim = butlerPrefab.GetComponent<Animator>();
        Debug.Log(butlerAnim);
        PlayerAnim = GameObject.Find("Player(Clone)").GetComponent<Animator>();
    }

    void Update()
    {
        if (isLocked) return;

        if (hit)
        {
            // ���͌��m(���͂��ꂽ���Ԃ��L�^)
            if (Input.GetKeyUp(KeyCode.P) || Input.GetButtonDown("Specific")) { lastYDownTime = Time.time; }
            if (Input.GetButtonDown("LB")) { lastLBDownTime = Time.time; }
            if (Input.GetButtonDown("RB")) { lastRBDownTime = Time.time; }
        }

        // ���Z�b�g
        if (lastLBDownTime != -Mathf.Infinity && lastLBDownTime + resetSec > Time.time)
        {
            if (Input.GetButtonDown("RB")) { MoveReset(); }
        }
        if (lastRBDownTime != -Mathf.Infinity && lastRBDownTime + resetSec > Time.time)
        {
            if (Input.GetButtonDown("LB")) { MoveReset(); }
        }

        if (Input.GetKeyUp(KeyCode.H))  
        {
            MoveReset();
        }

        if (Input.GetKeyUp(KeyCode.V))
        {
            PhaseSkip();
        }

        // �����w��(��莞�ԓ���LB�̓��͂��Ȃ���������)
        if (lastYDownTime != -Mathf.Infinity && lastYDownTime + resetSec < Time.time)    
        {
            // �X�L�b�v�@�\
            if (Input.GetButton("LB")) { PhaseSkip(); }

            // �v���C���[ �� �u���b�N �̕����x�N�g��
            pushDir[Movenum] = (bPos - pPos);
            pushDir[Movenum].y = 0.0f;

            //�u���b�N���΂߂ɍs���Ȃ��悤�ɒl���傫�����ɔ�΂�
            if (Mathf.Abs(pushDir[Movenum].x) >= Mathf.Abs(pushDir[Movenum].z))
            {
                Debug.Log(pushDir[Movenum]);
                pushDir[Movenum].z = 0.0f;
            }
            else
            {
                Debug.Log(pushDir[Movenum]);
                pushDir[Movenum].x = 0.0f;
            }

            //���K��
            pushDir[Movenum] = pushDir[Movenum].normalized;
            Debug.Log("������");

            // �U���A�j���[�V����
            PlayerAnim.SetTrigger("Attack");

            // ���̕`��
            Debug.Log(Movenum);
            arrow[Movenum].Draw(pushDir[Movenum], bPos, worldScale, Movenum);

            if (Movenum == 0)
            {
                AppearButler(0);               
            }

            if (bMirror)
            {
                if (MirrorObj != null)
                {
                    MirrorObj.SetPushDir(pushDir[Movenum]);
                }
            }

            addMovenum(false);

            lastYDownTime = -Mathf.Infinity;
        }
    }

    private void FixedUpdate()
    {
        if (isLocked) return;

        if (bMove && !isHitStopping)
        {
            // �ړ����̃A�j���[�V��������
            if (butlerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
          
            }
            else
            {
                if (CheckReserve(Movenum + 1))
                {
                    if (!bStartMove_1)
                    {
                        butlerAnim.SetTrigger("BeforePunch");
                        AudioManager.Instance.PlaySE("ButlerAttack");
                        bStartMove_1 = true;
                    }
                }
                
            }
            if (!bStartStop)
            {
                StopBlock();
                bStartStop = true;
            }
            // ���C����
            RayTest();
            // �ړ�����
            Vector3 move = pushDir[Movenum] * moveForce * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
            deltaMove = move; // �ړ��ʂ�ۑ�
            bPos = transform.position;
        }
        else
        {
            deltaMove = Vector3.zero;
        }
        

        // ����(��)
        //if(butlerPrefab.transform.position.x != 999.0f)
        //{
        //    timeCount += Time.deltaTime;
        //    if (timeCount > destroyTime)
        //    {
        //        butlerPrefab.transform.position = new Vector3(999.0f, 999.0f, 999.0f);
        //        timeCount = 0.0f;
        //    }            
        //}
    }

    //�������Ă���Ƃ�
    void OnCollisionEnter(Collision collision)
    {
        //�v���C���[���u���b�N�Ɠ������Ă��邩
        if (collision.gameObject.CompareTag("Player"))
        {
            hit = true;
            // �v���C���[�̈ʒu��ۑ�
            pPos = collision.transform.position;
        }

        // Enemy�ɓ��������ꍇ�̃q�b�g�X�g�b�v
        if (collision.gameObject.CompareTag("Enemy") && !isHitStopping)
        {
            StartCoroutine(HitStopCoroutine());
        }

        if (collision.gameObject.CompareTag("Object") || collision.gameObject.CompareTag("Block"))
        {
            Debug.Log("�����蔻��"+Movenum);
            // �ڐG�_�̖@��
            Vector3 contactNormal = collision.contacts[0].normal;
            // �����̐i�s�����i���O�� pushDir�j
            Vector3 moveDir = pushDir[Movenum].normalized;

            // �i�s�����Ɩ@���̓��ς��}�C�i�X�i���ʏՓˁj�̂Ƃ�������~����
            if (Vector3.Dot(moveDir, -contactNormal) > 0.5f)
            {
                Debug.Log("���ʂ���ǂɓ�������: " + collision.gameObject.name);
                StopMove();
            }
            else
            {
                Debug.Log("�ǂɎC�ꂽ�����Ȃ̂Ŗ���");
            }
        }
    }

    // �����葱���Ă��鎞
    private void OnCollisionStay(Collision collision)
    {
        //�v���C���[���u���b�N�Ɠ������Ă��邩
        if (collision.gameObject.CompareTag("Player"))
        {
            // �v���C���[�̈ʒu���X�V
            pPos = collision.transform.position;
        }
    }

    //�v���C���[�����ꂽ�Ƃ�
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hit = false;
        }
    }

    //���̃t�F�[�Y���̂��̃u���b�N�̓���
    public bool ReleaseStoredForce(int i)
    {

        Movenum = i;    //����p�K�{
        if (pushDir[i] != Vector3.zero)
        {
            rb.isKinematic = false; //�Œ艻����
            bMove = true;

            // �����o���u�Ԃ̃G�t�F�N�g
            SpawnStopEffect();
            SpeedEffect();
            Animator animator = butlerPrefab.GetComponent<Animator>();
            if(!bStartStop)
            {
                animator.SetTrigger("Attack");
            }
            return true;
        }
        return false;
    }

    //���̃u���b�N�������Ă��邩�`�F�b�N
    public bool CheckMove()
    {
        if (bMove) { return true; }
        return false;
    }

    public void StopMove()
    {
        if (isLocked) return;
        AudioManager.Instance.PlaySE("WallSE");

        bMove = false;
        rb.isKinematic = true;//�u���b�N�Œ�
        Debug.Log("�Ƃ܂���");

        // GameMNG�ɒʒm
        GameMNG.Check();
    }

    //�t�F�[�Y�i�s�p
    public void addMovenum(bool _move)
    {
        Movenum++;
        if (Movenum > 2) Movenum = 2;
        if (_move) ReleaseStoredForce(Movenum);
    }

    // ���O�̈ړ��ʂ��擾
    public Vector3 GetDeltaMove()
    {
        return deltaMove;
    }

    // ���폜
    public void DestroyArrow()
    {
        for (int i = 0; i < GameMNG.num; i++)
        {
            Destroy(arrowInstance[i]);
        }
    }
    // �ŏ��̃A�j���[�V�����p�q�b�g�X�g�b�v
    public void StopBlock()
    {
        hitStopTime = 3.0f;
        StartCoroutine(HitStopCoroutine());
        hitStopTime = 0.05f;
        
    }

    // �q�b�g�X�g�b�v�p�R���[�`��
    private IEnumerator HitStopCoroutine()
    {
        isHitStopping = true;

        // �ꎞ��~
        rb.isKinematic = true;

        yield return new WaitForSeconds(hitStopTime);

        // �ĊJ
        rb.isKinematic = false;
        isHitStopping = false;
    }

    // ���݂̃t�F�[�Y�擾
    public int GetPhase()
    {
        return Movenum;
    }

    // �u���b�N�ɓ������\�񂳂�Ă��邩
    // �\�񂳂�Ă���:true ����Ă��Ȃ�:false
    public bool CheckReserve(int i)
    {
        if(i < 3)
        {
            if (pushDir[i] != Vector3.zero) { return true; }
            else { return false; }
        }
        else
        {
            return false;
        }
    }

    // ���������̔ԍ���Ԃ�
    // �E:0 ��:1 ��:2 ��:3
    private int GetDirNumber(int Phase)
    {
        if (pushDir[Phase].x != 0)
        {
            if (pushDir[Phase].x > 0.0f)   // �E
            {
                return 0;
            }
            else                    // ��
            {
                return 1;
            }
        }
        else
        {
            if (pushDir[Phase].z > 0.0f)  // ��
            {
                return 2;
            }
            else                    // ��
            {
                return 3;
            }
        }
    }

    private float WhichLeftorRight(float bPos, int dirNum)
    {
        float subPos = bPos - wallPosition[dirNum, 0];
        if (subPos < 0) return bPos;
        else return -bPos;
    }

    private float WhichUporDown(float bPos, int dirNum)
    {
        float subPos = bPos - wallPosition[dirNum, 1];
        if (subPos > 0) return bPos;
        else return -bPos;
    }

    public void SetPushDir(Vector3 _Dir)
    {
        pushDir[Movenum] = -_Dir;
        arrow[Movenum].Draw(pushDir[Movenum], bPos, worldScale, Movenum);
        addMovenum(false);
    }

    private void SpawnStopEffect()
    {
        // ����̐i�s����
        Vector3 dir = pushDir[Movenum];

        // �����Ȃ牽�����Ȃ�
        if (dir == Vector3.zero || stopEffectPrefab == null) return;

        // ���Ε���
        Vector3 backDir = -dir;

        // ----------- �u���b�N�̔��a���v�Z -----------
        Vector3 scale = transform.localScale;   
        float radius;

        // X���������� �� X����
        if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.z))
        {
            radius = scale.x * 0.5f;
        }
        else // Z���������� �� Z����
        {
            radius = scale.z * 0.5f;
        }

        // ���ʂ֏������炷
        Vector3 spawnPos = transform.position + backDir * radius;
        spawnPos.y += 3.0f;

        // �G�t�F�N�g����
        shituji = Instantiate(stopEffectPrefab, spawnPos, Quaternion.LookRotation(backDir));
    }

    // �����o���p�֐�
    public void AppearButler(int Phase)
    {
        if(butlerAnim)
        if (pushDir[Phase].x != 0)
        {
            if (pushDir[Phase].x > 0.0f)   // �E
            {
                if(Phase == 0)  // �t�F�[�Y1�̏ꍇ�̓u���b�N�̂��΂ɏo��
                {
                    butlerAnim.SetTrigger("Ojigi");
                    butlerPrefab.transform.position = new Vector3(bPos.x - bScale.x * 1.5f, 1.5f, bPos.z);
                }
                else            // �t�F�[�Y2�ȍ~�͊O�ǂ̋߂��ɏo��
                {
                    // �A�j���[�V����
                    if(PlayerAnim)
                    {
                        PlayerAnim.SetTrigger("Call");
                    }
                        //if (CheckReserve(Phase + 1))
                        //{
                        //    butlerAnim.SetTrigger("BeforePunch");
                        //}
                        //else
                        //{
                        //    butlerAnim.SetTrigger("AfterPunch");
                        //}
                        int dirNum = GetDirNumber(Phase - 1);
                    butlerPrefab.transform.position = 
                        new Vector3(wallPosition[dirNum, 0] + WhichLeftorRight(bPos.x, dirNum) - bScale.x * 1.5f, 
                        1.5f, wallPosition[dirNum, 1]);
                }                    
                butlerPrefab.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f));
                ChangeEffect(butlerPrefab.GetComponentInChildren<ParticleSystem>());
            }
            else                    // ��
            {
                if (Phase == 0)
                {
                        butlerAnim.SetTrigger("Ojigi");
                        butlerPrefab.transform.position = new Vector3(bPos.x + bScale.x * 1.5f, 1.5f, bPos.z);
                }
                else
                {
                    // �A�j���[�V����  
                    if (PlayerAnim)
                    {
                        PlayerAnim.SetTrigger("Call");
                    }
                        //if (CheckReserve(Phase + 1))
                        //{
                        //    butlerAnim.SetTrigger("BeforePunch");
                        //}
                        //else
                        //{
                        //    butlerAnim.SetTrigger("AfterPunch");
                        //}
                        int dirNum = GetDirNumber(Phase - 1);
                    butlerPrefab.transform.position =
                    new Vector3(wallPosition[dirNum, 0] + WhichLeftorRight(bPos.x, dirNum) + bScale.x * 1.5f,
                    1.5f, wallPosition[dirNum, 1]);
                }
                butlerPrefab.transform.rotation = Quaternion.Euler(new Vector3(0.0f, -90.0f, 0.0f));
                ChangeEffect(butlerPrefab.GetComponentInChildren<ParticleSystem>());
            }
        }
        else
        {
            if (pushDir[Phase].z > 0.0f)  // ��
            {
                if (Phase == 0)
                {
                        butlerAnim.SetTrigger("Ojigi");
                        butlerPrefab.transform.position = new Vector3(bPos.x, 1.5f, bPos.z - bScale.z * 1.5f);
                }
                else
                {
                    if (PlayerAnim)
                    {
                        PlayerAnim.SetTrigger("Call");
                    }
                        //if (CheckReserve(Phase + 1))
                        //{
                        //    butlerAnim.SetTrigger("BeforePunch");
                        //}
                        //else
                        //{
                        //    butlerAnim.SetTrigger("AfterPunch");
                        //}
                        int dirNum = GetDirNumber(Phase - 1);
                    butlerPrefab.transform.position =
                    new Vector3(wallPosition[dirNum, 0], 1.5f, 
                    wallPosition[dirNum, 1] + WhichUporDown(bPos.z, dirNum) - bScale.z * 1.5f);
                }
                ChangeEffect(butlerPrefab.GetComponentInChildren<ParticleSystem>());
            }
            else                    // ��
            {
                if (Phase == 0)
                {
                        butlerAnim.SetTrigger("Ojigi");
                        butlerPrefab.transform.position = new Vector3(bPos.x, 1.5f, bPos.z + bScale.z * 1.5f);
                }
                else
                {
                    // �A�j���[�V����
                    if (PlayerAnim)
                    {
                        PlayerAnim.SetTrigger("Call");
                    }
                        //if (CheckReserve(Phase + 1))
                        //{
                        //    butlerAnim.SetTrigger("BeforePunch");
                        //}
                        //else
                        //{
                        //    butlerAnim.SetTrigger("AfterPunch");
                        //}
                        int dirNum = GetDirNumber(Phase - 1);
                    butlerPrefab.transform.position =
                    new Vector3(wallPosition[dirNum, 0], 1.5f,
                    wallPosition[dirNum, 1] + WhichUporDown(bPos.z, dirNum) + bScale.z * 1.5f);
                }
                butlerPrefab.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f));
                ChangeEffect(butlerPrefab.GetComponentInChildren<ParticleSystem>());
            }
        }
        Debug.Log(butlerPrefab.transform.position);
    }

    // ���x���G�t�F�N�g
    private void SpeedEffect()
    {
        if (transform.Find("SpeedLine"))
        {
            Destroy(transform.Find("SpeedLine").gameObject);
        }

        // ����̐i�s����
        Vector3 dir = pushDir[Movenum];

        // �����Ȃ牽�����Ȃ�
        if (dir == Vector3.zero || stopEffectPrefab == null) return;

        // ���Ε���
        Vector3 backDir = -dir;

        // ----------- �u���b�N�̔��a���v�Z -----------
        Vector3 scale = transform.localScale;
        Debug.Log(scale);
        float radius;

        // X���������� �� X����
        if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.z))
        {
            radius = scale.x * 0.5f;
        }
        else // Z���������� �� Z����
        {
            radius = scale.z * 0.5f;
        }

        // ���ʂ֏������炷
        Vector3 spawnPos = transform.position + backDir * radius;

        // �G�t�F�N�g����
        if (!effect)
        {
            //�G�t�F�N�g�𐶐�
            effect = Instantiate(SpeedEffectPrefab, spawnPos, Quaternion.LookRotation(backDir));
            follow = effect.GetComponent<FollowWorld>();
            //�^�[�Q�b�g����ݒ�
            follow.SetTarget(this.transform);
            follow.SetTransform(pushDir[Movenum], transform.localScale);
            butlerAnim.SetTrigger("Attack");

        }
        else
        {
            // �^�[�Q�b�g�����X�V
            follow.SetTransform(pushDir[Movenum], transform.localScale);
        }
    }

    private void MoveReset()
    {
        // �G��Ă���u���b�N�̓��������Z�b�g����
        Debug.Log("���Z�b�g");
        Reset();
        if (bMirror)
        {
            if (MirrorObj != null)
            {
                MirrorObj.Reset();
            }
        }
        // �A����true��ʂ�Ȃ��悤�Ƀ^�C���X�^���v�����Z�b�g
        lastLBDownTime = lastRBDownTime = -Mathf.Infinity; 
    }

    public void Reset()
    {
        for (int i = 0; i < GameMNG.num; i++) {
            pushDir[i] = Vector3.zero;
            arrow[i].gameObject.SetActive(false);
            butlerPrefab.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
        }
        Movenum = 0;
    }

    private void PhaseSkip()
    {  
        // �G��Ă���u���b�N�̃t�F�[�Y���X�L�b�v
        Debug.Log("�X�L�b�v");
        addMovenum(false);
        // �A����true��ʂ�Ȃ��悤�Ƀ^�C���X�^���v�����Z�b�g
        lastLBDownTime = lastYDownTime = -Mathf.Infinity;
    }

    private void ChangeEffect(ParticleSystem particle)
    {
        particle.Play();
    }

    // ���u���b�N�Z�b�^�[
    public void SetMirror(Block other)
    {
        bMirror = true;
        MirrorObj = other;
    }

    // �A�j���[�V�����J�n�p���C����
    private void RayTest()
    {
        Debug.Log(pushDir[Movenum] + "RayTest");
        //Ray�̍쐬�@�@�@�@�@�@�@��Ray���΂����_�@�@�@��Ray���΂�����
        // ����̐i�s����
        Vector3 dir = pushDir[Movenum];
        Debug.Log(dir);
        Ray ray = new Ray(transform.position, dir);

        //Ray�����������I�u�W�F�N�g�̏������锠
        RaycastHit hit;

        //Ray�̔�΂��鋗��
        int distance = 10;

        //Ray�̉���    ��Ray�̌��_�@�@�@�@��Ray�̕����@�@�@�@�@�@�@�@�@��Ray�̐F
        Debug.DrawLine(ray.origin, ray.direction * distance, Color.red);

        //����Ray�ɃI�u�W�F�N�g���Փ˂�����
        //                  ��Ray  ��Ray�����������I�u�W�F�N�g ������
        if (Physics.Raycast(ray, out hit, distance))
        {
            //Ray�����������I�u�W�F�N�g��tag��Object��������
            if (hit.collider.tag == "Object")
            // �A�j���[�V�����Đ��J�n����
            if (CheckReserve(Movenum + 2))
            {

            }
            else
            {
                if (!bStartMove_2)
                {
                    bStartMove_2 = true;
                    butlerAnim.SetTrigger("AfterPunch");
                }
            }
        }
    }

    public void ForceLockStop()
    {
        if (isLocked) return;

        isLocked = true;

        // �ړ����S��~
        bMove = false;
        isHitStopping = false;

        // ������~
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // ���́E�\���������
        for (int i = 0; i < pushDir.Length; i++)
        {
            pushDir[i] = Vector3.zero;
        }

        Debug.Log($"Block {name} locked and stopped");
    }

}
