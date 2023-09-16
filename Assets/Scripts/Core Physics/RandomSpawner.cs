using UnityEngine;
using System.Collections;

public class RandomSpawner : MonoBehaviour
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

    public GameObject character;

    private void Start()
    {
        SpawnObjects();
    }

    void Update()
    {
        character.SetActive(false);
    }

    private void SpawnObjects()
    {
        for (int i = 0; i < _numObjectsToSpawn; i++)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-_spawnArea.x, _spawnArea.x), 
                                           0f,
                                           Random.Range(-_spawnArea.z, _spawnArea.z));
            GameObject obj = Instantiate(_objectToSpawn, spawnPos, Quaternion.identity);
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
        GameObject obj = Instantiate(_objectToSpawn, spawnPos, Quaternion.identity);
        _currentObject = obj;

        StartCoroutine(SpawnNextObject());
    }
}
