using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public List<GameObject> poolPrefabs;
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();
    private Queue<GameObject> soundQueue = new Queue<GameObject>();

    private void OnEnable()
    {
        EventHandler.ParticleEffectEvent += OnParticleEffectEvent;
        EventHandler.InitSoundEffect += InitSoundEffect;
    }

    private void OnDisable()
    {
        EventHandler.ParticleEffectEvent -= OnParticleEffectEvent;
        EventHandler.InitSoundEffect -= InitSoundEffect;
    }

    private void Start()
    {
        CreatePool();
    }
    private void CreatePool()
    {
        foreach (GameObject item in poolPrefabs)
        {
            Transform parent = new GameObject(item.name).transform;
            parent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
               () => Instantiate(item, parent),
               e => { e.SetActive(true); },
               e => { e.SetActive(false); },
               e => { Destroy(e); }
           );

            poolEffectList.Add(newPool);
        }
    }

    private void OnParticleEffectEvent(ParticleEffecType effecType, Vector3 pos)
    {
        //根据特效补全
        ObjectPool<GameObject> objPool = effecType switch
        {
        ParticleEffecType.LeavesFalling01 => poolEffectList[0],
        ParticleEffecType.LeavesFalling02 => poolEffectList[1],
        ParticleEffecType.Rock => poolEffectList[2],
        ParticleEffecType.ReapableSecnery => poolEffectList[3],
        _ => null,
        };

        GameObject obj = objPool.Get();
        obj.transform.position = pos;
        StartCoroutine(ReleaseRoutine(objPool,obj));
    }


    private IEnumerator ReleaseRoutine(ObjectPool<GameObject> objectPool ,GameObject obj)
    {
        yield return new WaitForSeconds(1.5F);
        objectPool.Release(obj);
    }

    //private void InitSoundEffect(SoundDetails soundDetails)
    //{
    //    ObjectPool<GameObject> objPool = poolEffectList[4];
    //    var obj = objPool.Get();
    //    obj.GetComponent<Sound>().SetSound(soundDetails);
    //    StartCoroutine(DisableSound(objPool, obj, soundDetails));
    //}

    //private IEnumerator DisableSound(ObjectPool<GameObject> pool ,GameObject obj ,SoundDetails soundDetails)
    //{
    //    yield return new WaitForSeconds(soundDetails.soundClip.length);
    //    pool.Release(obj);
    //}

    private void CreateSoundPool()
    {
        Transform parent;

        if (transform.GetChild(4))
        {
            parent = transform.GetChild(4);
        }
        else
        {
            parent = new GameObject(poolPrefabs[4].name).transform;
        }


        parent.SetParent(transform);

        for (int i = 0; i < 20; i++)
        {
            GameObject newObj = Instantiate(poolPrefabs[4], parent);
            newObj.SetActive(false);
            soundQueue.Enqueue(newObj);
        }
    }

    private GameObject GetPoolObject()
    {
        if (soundQueue.Count < 2)
            CreateSoundPool();
        return soundQueue.Dequeue();
    }

    private void InitSoundEffect(SoundDetails soundDetails)
    {
        var obj = GetPoolObject();
        obj.GetComponent<Sound>().SetSound(soundDetails);
        obj.SetActive(true);
        StartCoroutine(DisableSound(obj, soundDetails.soundClip.length));
    }

    private IEnumerator DisableSound(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        obj.SetActive(false);
        soundQueue.Enqueue(obj);
    }
}
