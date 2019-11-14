using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _radiusExplosion;
    [SerializeField] private GameObject _explosionSound;

    private Vector3 dir;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        transform.Translate(dir * _speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Obstacle") {
            other.gameObject.GetComponent<MarchingCubes>().Destruction(transform.position, _radiusExplosion);
            Instantiate(_explosionSound, transform.position, Quaternion.identity);
            //Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    public void SetDir(Vector3 _dir) {
        dir = _dir;
    }
}
