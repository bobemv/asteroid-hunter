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

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Obstacle") {
            Debug.Log("Contact points: " + collision.contactCount);
            Vector3 furthestContactPoint = transform.position;
            float furthestDistance = 0;
            foreach (ContactPoint contact in collision.contacts) {
                float currentDistance = Vector3.Distance(transform.position, contact.point);
                if (currentDistance > furthestDistance) {
                    furthestDistance = currentDistance;
                    furthestContactPoint = contact.point;
                }
            }
            collision.gameObject.GetComponent<MarchingCubes>().Destruction(furthestContactPoint, _radiusExplosion);
            RaycastHit hit;
            Physics.Raycast(furthestContactPoint, Vector3.forward, out hit, _radiusExplosion);
            if (hit.collider != null && hit.collider.tag == "Obstacle" && hit.collider != collision.collider) {
                hit.collider.gameObject.GetComponent<MarchingCubes>().Destruction(furthestContactPoint, _radiusExplosion);
            }
            Instantiate(_explosionSound, transform.position, Quaternion.identity);
            //Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    public void SetDir(Vector3 _dir) {
        dir = _dir;
    }
}
