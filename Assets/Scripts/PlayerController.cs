using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    RatKingInputs playerInput;
    InputAction move;
    InputAction jump;
    InputAction dash;
    InputAction pause;

    Rigidbody playerRigidbody;

    bool isGrounded;

    Vector3 lastSavePosition;

    [SerializeField]
    bool doTrapsThrewChildrenAway;

    [SerializeField]
    float moveSpeed = 2, dashGrowth = 0.1f, currentDashPower, maxDashSpeed = 15f, speedDecrease = 0.05f, jumpForce = 3, ratReleaseForce = 3, defaultLightIntensity = 1f, maxLightIntensity = 50f;

    public float DashGrowth {get => dashGrowth; set { dashGrowth = value;} }

    float dashReleaseForce;
    public float MaxDashSpeed { get => maxDashSpeed; }

    [SerializeField]
    LayerMask Ground;

    float dashTimeStart;

    [SerializeField]
    GameObject outerBall;

    List<GameObject> ratChildren = new List<GameObject>();


    public delegate void CountChange(int newCount);
    public static event CountChange OnCountChange;

    public delegate void AreaCountChange(EAreas newBelonging, bool isRatAdded);
    public static event AreaCountChange OnAreaCountChange;

    public static Action<float> OnDashChange;

    public delegate void AreaChange(EAreas newArea);
    public static event AreaChange OnAreaChange;

    bool hasGameEnded;

    public bool HasGameEnded { set => hasGameEnded = value; }

    ///TESTESTEST
    public static Action OnOverPopulation;
    public List<AudioSource> ratAudios = new List<AudioSource>();

    [SerializeField]
    int overpopulationBorder = 25;

    [SerializeField]
    float targetDistanceToKing = 1.2f;

    Light royalLight;


    bool isStopped = true;
    public bool IsStopped {

        get => isStopped;

        set
        {
            isStopped = value;

            if (!hasGameEnded && doTrapsThrewChildrenAway)
                LetChildrenGo();

            if (isStopped)
            {
                PlayDeathSound();
            }
        }
    }

    [SerializeField] bool useRatsRepelWithHardHit = true;
    [SerializeField] int hardHitTolerance = 6;

    Material defaultMaterial;

    MenuMethods regisseur = null;

    [SerializeField]
    AudioClip hitSound, unityOfRatsSound, deathSound, dashReleaseSound;

    AudioSource royalAudioSource;

    EAreas area = EAreas.Error;
    public EAreas Area
    {
        get => area;

        set
        {
            area = value;

            AreaToChange(value);
        }
    }

    void Awake()
    {
        playerInput = new RatKingInputs();
        move = playerInput.RatKingActionmap.Movement;
        jump = playerInput.RatKingActionmap.Jump;
        dash = playerInput.RatKingActionmap.Dash;
        pause = playerInput.RatKingActionmap.Pause;

        playerRigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        defaultMaterial = outerBall.GetComponent<Renderer>().material;

        royalLight = GetComponentInChildren<Light>();
        defaultLightIntensity = royalLight.intensity;
        lastSavePosition = transform.position;
        royalAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (regisseur == null) //regisseur is a singleton, that has to be found wenn scene is reseted
        {
            regisseur = FindObjectOfType<MenuMethods>();

            if (regisseur == null)
            {
                Debug.LogError("No regisseur for this scene! Menu wont work.");
            }
        }
    }

    private void OnEnable()
    {
        move.Enable();
        jump.Enable();
        dash.Enable();
        pause.Enable();

        dash.started += OnDashStarted;
        dash.canceled += OnDashCanceled;
        jump.started += Jump;
        pause.started += StartPause;
    }

    private void OnDisable()
    {
        jump.started -= Jump;
        dash.started -= OnDashStarted;
        dash.canceled -= OnDashCanceled;
        pause.started -= StartPause;

        move.Disable();
        jump.Disable();//.Enable();
        dash.Disable();
        pause.Disable();
    }

    void FixedUpdate()
    {
        if (!isStopped)
        {
            if (dash.IsPressed() && isGrounded)
            {
                DashLoad();
            }
            else
            {
                Move();
            }

            isGrounded = CheckForGround();

            if (IsInDashTime())
            {
                if (dashReleaseForce > 0)
                {
                    dashReleaseForce -= dashGrowth;

                    //Delegate
                    DashChange(dashReleaseForce / maxDashSpeed - 0.1f);
                }
            }

            if (useRatsRepelWithHardHit)
                CheckForWallRepel();
        }
    }

    bool CheckForGround()
    {
        Debug.DrawRay(transform.position, Vector3.down * transform.GetComponent<Collider>().bounds.size.y *0.65f, Color.blue);

        bool isOnGround = Physics.Raycast(transform.position, Vector3.down, transform.GetComponent<Collider>().bounds.size.y * 0.65f, Ground);

        if (isOnGround && !isGrounded)
        {
            PlayHitSound();
        }

        return isOnGround;
    }

    void Move()
    {
        if (IsInDashTime()) return;

        if (move.IsPressed())
        {
            Vector3 input = MoveDirection() * moveSpeed;
            playerRigidbody.AddForce(input, ForceMode.Force);
        }
        else
        {
            SlowStandstill();
        }
    }

    private Vector3 MoveDirection()
    {
        Vector2 moveInput = move.ReadValue<Vector2>();
        Vector3 input = new Vector3(moveInput.x, 0, moveInput.y);// * moveSpeed;
        input = Camera.main.transform.rotation * input;
        input = input = new Vector3(input.x, playerRigidbody.velocity.y, input.z).normalized;

        return input;
    }

    #region Dash
    bool IsInDashTime()
    {
        if (dashTimeStart + dashReleaseForce > Time.time)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnDashStarted(InputAction.CallbackContext callbackContext)
    {
        currentDashPower = 0;
    }

    void DashLoad()
    {
        if (IsInDashTime()) return;

        SlowStandstill();

        if (currentDashPower <= maxDashSpeed)
        {
            currentDashPower += dashGrowth;
        }

        if (currentDashPower>0.1f)
            royalLight.color = Color.red;

        transform.Rotate(transform.forward, 10 * (currentDashPower / maxDashSpeed));

        royalLight.intensity = maxLightIntensity * (currentDashPower / maxDashSpeed);

        //Delegate
        DashChange(currentDashPower / maxDashSpeed);
    }

    void OnDashCanceled(InputAction.CallbackContext callbackContext)
    {
        if (IsInDashTime()) return;

        DashRelease(currentDashPower);

        //Light
        royalLight.color = Color.white; //default light color
        royalLight.intensity = defaultLightIntensity;
    }
    #endregion

    public void DashRelease(float currentDashPower)
    {
        Vector3 dashDirection;

        if (move.IsPressed())
        {
            dashDirection = MoveDirection();
        }
        else
        {
            dashDirection = Camera.main.transform.forward;
            dashDirection = new Vector3(dashDirection.x, 0, dashDirection.z);
        }

        dashTimeStart = Time.time;

        dashReleaseForce = currentDashPower;
        playerRigidbody.AddForce(dashDirection * dashReleaseForce, ForceMode.Impulse);

        PlayDashReleaseSound();

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Servant"))
        {
            RatBrain newRatsBrain = collision.gameObject.GetComponent<RatBrain>();
            Rigidbody newRatsRigidbody = collision.gameObject.GetComponent<Rigidbody>();

            if (newRatsBrain.IsTrapped && !newRatsRigidbody.isKinematic)
                newRatsRigidbody.isKinematic = true;

            if (newRatsRigidbody.isKinematic)
            {
                collision.gameObject.GetComponentInChildren<ParticleSystem>().Play();

                if (royalAudioSource.isPlaying)
                {
                    royalAudioSource.Stop();
                }
                PlayRatsUnitySound();

                StartCoroutine(TurnToKing(collision.gameObject));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Save"))
        {
            lastSavePosition = transform.position;
        }
    }

    private void CheckForWallRepel()
    {
        RaycastHit wallHit;
        if (Physics.SphereCast(transform.position, outerBall.GetComponent<SphereCollider>().radius, transform.position, out wallHit, outerBall.GetComponent<SphereCollider>().radius * 2, LayerMask.GetMask("Wall")))
        {
            if (Vector3.Dot(transform.forward, wallHit.transform.position - transform.position) >= 0) //Check if direction of ball is towards wall
                LetChildrenGo((int)playerRigidbody.velocity.magnitude);
        }
    }

    IEnumerator TurnToKing(GameObject servant)
    {
        Destroy(servant.GetComponent<NavMeshAgent>());
        servant.GetComponent<Collider>().enabled = false;
        servant.GetComponent<RatBrain>().IsTrapped = true;
        servant.transform.parent = this.transform;

        Vector3 direction = servant.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        //Positioning
        float currentDistance = (servant.transform.position - transform.position).magnitude;
        while (currentDistance > targetDistanceToKing + 0.1f || currentDistance < targetDistanceToKing - 0.1f)
        {
            Vector3 ratDirection = (servant.transform.position - transform.position).normalized;
            Vector3 targetPosition = transform.position + ratDirection * targetDistanceToKing;
            servant.transform.position = Vector3.Lerp(servant.transform.position, targetPosition, 0.1f);
            currentDistance = (servant.transform.position - transform.position).magnitude;

            yield return null;
        }

        //Rotationing
        while (Quaternion.Angle(servant.transform.rotation, targetRotation) > 10f)
        {
            direction = servant.transform.position - transform.position;
            targetRotation = Quaternion.LookRotation(direction, Vector3.up);

            servant.transform.rotation = Quaternion.Slerp(servant.transform.rotation, targetRotation, 0.1f);

            yield return null;
        }


        ratChildren.Add(servant);

        OnCountChange(ratChildren.Count);

        OnAreaCountChange?.Invoke(servant.GetComponent<RatBrain>().AreaBelonging, true);


        //TESTESTEST
        ratAudios.Add(servant.GetComponentInChildren<AudioSource>());
        if(ratChildren.Count > overpopulationBorder)
        {
            OnOverPopulation?.Invoke();

            foreach (var rat in ratAudios)
            {
                rat.pitch += 0.01f;
            }
        }

        defaultLightIntensity += 0.01f;
        royalLight.intensity = defaultLightIntensity;

        royalLight.range += 0.01f;

        Destroy(servant.GetComponentInChildren<Light>());

    }

    private void LetChildrenGo()
    {
        if (ratChildren.Count == 0) return;

        GetComponent<Collider>().enabled = false;
        StartCoroutine("WaitForCollider");

        for (int i = ratChildren.Count - 1; i >= 0; i--)
        {
            if (ratChildren[i].transform.position.y > transform.position.y)
            {
                ratChildren[i].transform.parent = null;
                ratChildren[i].GetComponent<Rigidbody>().isKinematic = false;
                ratChildren[i].GetComponent<Collider>().enabled = true;

                Vector3 direction = (ratChildren[i].transform.position - transform.position).normalized * ratReleaseForce;
                ratChildren[i].GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse);

                OnAreaCountChange(ratChildren[i].GetComponent<RatBrain>().AreaBelonging, false);

                ratChildren.Remove(ratChildren[i]);
            }
        }

        OnCountChange(ratChildren.Count);
    }

    IEnumerator WaitForCollider() //collider is paused for a moment to easier throw away children
    {
        float waitingTime = 1f;

        while (waitingTime > 0f)
        {
            yield return new WaitForSeconds(1f);
            waitingTime--;
        }

        GetComponent<SphereCollider>().enabled = true;
    }

    private void LetChildrenGo(int force)
    {
        int minForce = hardHitTolerance;

        if (force < minForce || ratChildren.Count == 0) return;

        int currentForce = force;

        for (int i = ratChildren.Count - 1; i >= 0; i--)
        {
            if (currentForce - minForce <= 0) break;

            if (ratChildren[i].transform.position.y > transform.position.y)
            {
                ratChildren[i].transform.parent = null;
                ratChildren[i].GetComponent<Rigidbody>().isKinematic = false;
                ratChildren[i].GetComponent<Collider>().enabled = true;

                Vector3 direction = (ratChildren[i].transform.position - transform.position) * ratReleaseForce;
                ratChildren[i].GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse);

                ratChildren.Remove(ratChildren[i]);

                currentForce--;

                OnAreaCountChange(ratChildren[i].GetComponent<RatBrain>().AreaBelonging, false);
            }
        }

        OnCountChange(ratChildren.Count);
    }

    private void Jump(InputAction.CallbackContext callbackContext)
    {
        if (!isGrounded) return;
        if (isStopped) return;

        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void SlowStandstill()
    {
        playerRigidbody.velocity = Vector3.Slerp(playerRigidbody.velocity, Vector3.zero, speedDecrease);
    }

    //Delegate for GUI
    public void UpdateGUI(int newCount, EAreas newBelonging, bool isRatAdded)
    {
        OnCountChange?.Invoke(newCount);
        OnAreaCountChange?.Invoke(newBelonging, isRatAdded);
    }

    public void DashChange(float newAmount)
    {
        OnDashChange?.Invoke(newAmount);
    }

    public void AreaToChange(EAreas newArea)
    {
        OnAreaChange?.Invoke(newArea);
    }

    void StartPause(InputAction.CallbackContext callbackContext)
    {
        if (regisseur.CurrentScene == EScenes.DeathAdditionalScene) return;

        if (Time.timeScale==1f)
        {
            regisseur.CurrentScene = EScenes.PauseScene;// "PauseScene";
            SceneManager.LoadScene(EScenes.PauseScene.ToString(), LoadSceneMode.Additive);
            Time.timeScale = 0;
        }
        else
        {
            regisseur.QuickContinue();
        }
    }

    public void ResetAndReturnToLastSave() //used by menu
    {
        if (outerBall.GetComponent<Renderer>().material != defaultMaterial)
        {
            outerBall.GetComponent<Renderer>().material = defaultMaterial;
            isStopped = false;
            playerRigidbody.constraints = RigidbodyConstraints.None;
        }

        transform.position = lastSavePosition;
    }

    void PlayHitSound()
    {
        if (royalAudioSource.isPlaying) return;

        royalAudioSource.clip = hitSound;
        royalAudioSource.Play();
    }

    void PlayRatsUnitySound()
    {
        royalAudioSource.clip = unityOfRatsSound;
        royalAudioSource.Play();
    }

    void PlayDeathSound()
    {
        royalAudioSource.clip = deathSound;
        royalAudioSource.Play();
    }


    //
    void PlayDashReleaseSound()
    {
        if (isStopped) return;

        if (royalAudioSource.isPlaying) return;
        
        float previousPitch = royalAudioSource.pitch;
        royalAudioSource.pitch = Mathf.Max(0.3f, currentDashPower / maxDashSpeed);
        royalAudioSource.PlayOneShot(dashReleaseSound, Mathf.Max(0.3f, currentDashPower / maxDashSpeed));
        royalAudioSource.pitch = previousPitch;
        
    }

    //void RegulateDashPitch()
    //{
    //    royalAudioSource.pitch = currentDashPower / maxDashSpeed;
    //}
    //
    //void ResetDashPitch()
    //{
    //    royalAudioSource.pitch = 1f;
    //}
}
