using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster gm;

    void Awake()
    {
        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        }
    }

    public Transform playerPrefab;
    public Transform spawnPoint;
    public float spawnDelay = 3.5f;
    public Transform spawnPrefab;
    public CameraShake cameraShake;

    internal static void KillEnemy(Enemy enemy)
    {
        throw new NotImplementedException();
    }

    void Start()
    {
        if (cameraShake == null)
        {
            Debug.LogError("No camera shake referenced in GameMaster");
        }
    }
    
    public IEnumerator _RespawnPlayer()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.Play();
        yield return new WaitForSeconds(spawnDelay);
        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        Transform spawnParticles = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation) as Transform;
        Destroy(spawnParticles.gameObject, 3f);
    }

    // singleton pattern: ensure that there is only one instance of an object
    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
        gm.StartCoroutine(gm._RespawnPlayer());
    }

    public static void KillEn(Enemy enemy)
    {
        gm._KillEnemy(enemy);
    }

    public void _KillEnemy(Enemy _enemy)
    {
        Transform clone = Instantiate(_enemy.enemyDeathParticles, _enemy.transform.position, Quaternion.identity) as Transform;
        Destroy(clone.gameObject, 5f);
        cameraShake.Shake(_enemy.shakeAmount, _enemy.shakeLength);
        Destroy(_enemy.gameObject);
    }
}
