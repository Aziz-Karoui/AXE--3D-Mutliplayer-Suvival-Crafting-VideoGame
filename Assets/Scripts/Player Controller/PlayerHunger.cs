using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerHunger : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] public float health;
    [SerializeField] private Image frontHealthBar;
    [SerializeField] private Image backHealthBar;
    [SerializeField] private float chipSpeed = 2f;
    [SerializeField] private GameObject gameOverUI;

    private ThirdPersonMovement moveScript;
    private Animator animator;
    private float lerpTimer;

    public delegate void DeathEventHandler(NetworkConnection conn);
    public event DeathEventHandler deathEvent;

    public delegate void WinEventHandler(NetworkConnection conn);
    public event WinEventHandler winEvent;

    public float Health
    {
        get { return health; }
        set
        {
            health = value;
            RpcUpdateHealthUI();
        }
    }

    private void Start()
    {
        moveScript = GetComponent<ThirdPersonMovement>();
        animator = GetComponent<Animator>();
        health = maxHealth;
        InvokeRepeating("ReduceHealth", 1f, 1f);
    }

    private void ReduceHealth()
    {
        Health -= 1f;
    }
    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.K))
            TakeDamage(Random.Range(5, 10));
        
        if (Input.GetKeyDown(KeyCode.L))
            RestoreHealth(Random.Range(5, 10));

        if (Input.GetKeyDown(KeyCode.K))
            TakeDamage(5);

        if (Input.GetKeyDown(KeyCode.L))
            RestoreHealth(5);

        if (health <= 0)
            deathEvent?.Invoke(connectionToServer);
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
    }

    public void RestoreHealth(float healAmount)
    {
        Health += healAmount;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSubscribeToEvents();
    }

    [Command]
    private void CmdSubscribeToEvents()
    {
        NetworkManager.singleton.playerPrefab.GetComponent<PlayerHealth>().deathEvent += OnPlayerDeath;
        NetworkManager.singleton.playerPrefab.GetComponent<PlayerHealth>().winEvent += OnPlayerWin;
    }

    private void OnPlayerDeath(NetworkConnection conn)
    {
        if (isLocalPlayer && conn == connectionToServer)
            gameOverUI.SetActive(true);
    }

    private void OnPlayerWin(NetworkConnection conn)
    {
        if (isLocalPlayer && conn == connectionToServer)
            animator.SetTrigger("Win");
    }

    [ClientRpc]
    private void RpcUpdateHealthUI()
    {
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;
        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }


        if (fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }
    }
        public float GetHealth()
    {
        return health;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("fruit"))
        {
            Health += 15f;
            Destroy(collision.gameObject);
        }
    }
}
