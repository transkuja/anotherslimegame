using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterShooter : MonoBehaviour {

    // temp : bulletPrefab
    [SerializeField] GameObject bulletPrefab;

    public delegate void action();
    action CurAction;

    [SerializeField] private int bulletPerRafales;
    [SerializeField] private float chargeDelay;
    [SerializeField] private float rafalesChargeDelay;
    [SerializeField] private float bulletDistance;
    [SerializeField] private float bulletSpeed;

    private float timer;
    private float curBullet;
    Transform bulletSpawn;

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
    }
    public void CreateBullet()
    {
        GameObject bulletGo = ResourceUtils.Instance.poolManager.monsterShotsPool.GetItem(transform, bulletSpawn.position, Quaternion.identity, true, true);
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
	}
	
	// Update is called once per frame
	void Update () {
        
        if (CurAction!=null)
            CurAction();
        else
            MonsterActivation();
    }
}
