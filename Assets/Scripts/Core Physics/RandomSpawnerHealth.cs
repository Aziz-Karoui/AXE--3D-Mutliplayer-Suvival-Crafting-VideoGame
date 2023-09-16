using UnityEngine;
using System.Collections;

public class RandomSpawnerHealth : MonoBehaviour
{
    [SerializeField]
    private GameObject _objectToSpawn;

    [SerializeField]
    private int _numObjectsToSpawn = 10;

    [SerializeField]
    private Vector3 _spawnArea;

    [SerializeField]
    private float _spawnDelay = 1f;

    private GameObject _currentObject;


    //********************  OY OY OY METANSECH IDHE KEN OBJECCT FOU9 OBJECT E5ER ********************
    private void Start()
    {
        SpawnObjects();
    }

    void Update()
    {
        
    }

    private void SpawnObjects()
    {
        for (int i = 0; i < _numObjectsToSpawn; i++)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-_spawnArea.x, _spawnArea.x), 
                                           0f,
                                           Random.Range(-_spawnArea.z, _spawnArea.z));
            GameObject obj = Instantiate(_objectToSpawn, spawnPos, Quaternion.Euler(-90f, 0f, 0f));
            _currentObject = obj;
        }
        StartCoroutine(SpawnNextObject());
    }

    private IEnumerator SpawnNextObject()
    {
        yield return new WaitForSeconds(_spawnDelay);

        Destroy(_currentObject);

        Vector3 spawnPos = new Vector3(Random.Range(-_spawnArea.x, _spawnArea.x), 
                                       0f,
                                       Random.Range(-_spawnArea.z, _spawnArea.z));
        GameObject obj = Instantiate(_objectToSpawn, spawnPos, Quaternion.Euler(-90f, 0f, 0f));
        _currentObject = obj;

        StartCoroutine(SpawnNextObject());
    }
}
