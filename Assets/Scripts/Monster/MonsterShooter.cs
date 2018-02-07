using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterShooter : MonoBehaviour {

    // temp : bulletPrefab
    [SerializeField] GameObject bulletPrefab;

    public delegate void action();
    action CurAction;
    [SerializeField] private bool obeyAlpha;
    [SerializeField] private MonsterShooter leaderShooter;
    [SerializeField] private float afterLeaderDelay;

    [SerializeField] private int bulletPerRafales;
    [SerializeField] private float chargeDelay;
    [SerializeField] private float rafalesChargeDelay;
    [SerializeField] private float bulletDistance;
    [SerializeField] private float bulletSpeed;

    private float timer;
    private float curBullet;
    private Transform bulletSpawn;
    public action OnShoot;

    #region getterSetters
    public int BulletPerRafales
    {
        get
        {
            return bulletPerRafales;
        }

        set
        {
            bulletPerRafales = value;
        }
    }

    public float ChargeDelay
    {
        get
        {
            return chargeDelay;
        }

        set
        {
            chargeDelay = value;
        }
    }

    public float RafalesChargeDelay
    {
        get
        {
            return rafalesChargeDelay;
        }

        set
        {
            rafalesChargeDelay = value;
        }
    }

    public float BulletDistance
    {
        get
        {
            return bulletDistance;
        }

        set
        {
            bulletDistance = value;
        }
    }

    public float BulletSpeed
    {
        get
        {
            return bulletSpeed;
        }

        set
        {
            bulletSpeed = value;
        }
    }

    public MonsterShooter LeaderShooter
    {
        get
        {
            return leaderShooter;
        }

        set
        {
            leaderShooter = value;
        }
    }

    public bool ObeyAlpha
    {
        get
        {
            return obeyAlpha;
        }

        set
        {
            obeyAlpha = value;
        }
    }

    public float AfterLeaderDelay
    {
        get
        {
            return afterLeaderDelay;
        }

        set
        {
            afterLeaderDelay = value;
        }
    }
    #endregion


    void OnBegin()
    {
        timer = 0;
        CurAction = ChargeWait;
    }

    void ChargeWait()
    {
        timer += Time.deltaTime;
        if (timer > chargeDelay)
        {
            timer = 0;
            CurAction = Shoot;
        }
    }
    void RafaleWait()
    {
        timer += Time.deltaTime;
        if (timer > rafalesChargeDelay)
        {
            timer = 0;
            CurAction = Shoot;
            MonsterActivation();
        }
    }
  
    void DelayedShoot()
    {
        Invoke("Shoot", afterLeaderDelay);
    }
    void Shoot()
    {
        curBullet++;
        CreateBullet();
        if (curBullet >= bulletPerRafales)
        {
            curBullet = 0;
            CurAction = RafaleWait;
        }
        else
        {
            CurAction = ChargeWait;
        }
        if (OnShoot!=null)
        {
            OnShoot();
        }
    }
    public void CreateBullet()
    {
        GameObject bulletGo = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.MonsterShots).GetItem(transform, bulletSpawn.position, Quaternion.identity, true, true);
        Bullet bullet = bulletGo.GetComponent<Bullet>();
        bullet.Init(this.gameObject);
        bullet.Fire(transform.forward, bulletSpeed, bulletDistance);
    }

    void MonsterActivation()
    {
        bool playerIsNear = false;
        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            float distance = Vector3.Distance(GameManager.Instance.PlayerStart.PlayersReference[i].transform.position, transform.position);
            if (distance < 100)
            {
                playerIsNear = true;
                break;
            }
           
        }
        if (playerIsNear)
            OnBegin();
        else
            CurAction = null;
    }

    // Use this for initialization
    private void Awake()
    {
        bulletSpawn = transform.Find("BulletSpawn");
    }
    void Start () {
        if (obeyAlpha)
        {
            if (leaderShooter != null)
                leaderShooter.OnShoot += DelayedShoot;
            else
                Debug.LogWarning("Monster missing leader");
        }
	}
	
	// Update is called once per frame
	void Update () {
        
        if (!obeyAlpha)
        {
            if (CurAction != null)
                CurAction();
            else
                MonsterActivation();
        }
    }
}
