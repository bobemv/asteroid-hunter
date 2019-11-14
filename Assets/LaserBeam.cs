using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _tickRate;
    [SerializeField] float _maxLaserLength;
    [SerializeField] float _radiusDestruction;
    [SerializeField] GameObject _explosionSoundPrefab;

    private List<Vector3> linePositions;
    private LineRenderer lineRenderer;
    private float laserLength = 0f;
    private Coroutine shootingLaserBeamCoroutine;

    // Start is called before the first frame update
    void Awake()
    {
        TryGetComponent<LineRenderer>(out lineRenderer);
        linePositions = new List<Vector3>();
        linePositions.Add(new Vector3(0,0,0));
        linePositions.Add(new Vector3(0,0, 0));
        transform.rotation = Quaternion.identity;
    }

    void OnEnable() {
        linePositions[1] = new Vector3(0, 0, 0);
        lineRenderer.SetPositions(linePositions.ToArray());
        shootingLaserBeamCoroutine = StartCoroutine(ShootingLaserBeam());
    }

    void OnDisable() {
        if (shootingLaserBeamCoroutine != null) {
            StopCoroutine(shootingLaserBeamCoroutine);
            shootingLaserBeamCoroutine = null;
        }
    }

    IEnumerator ShootingLaserBeam() {
        while (true) {
            laserLength += _speed;
            laserLength = Mathf.Min(laserLength, _maxLaserLength);
            RaycastHit hit;
            Physics.Raycast(transform.position, transform.forward, out hit, laserLength);
            if (hit.collider != null && hit.collider.tag == "Obstacle") {
                laserLength = hit.distance;

                hit.collider.GetComponent<MarchingCubes>().Destruction(hit.point, _radiusDestruction);
                RaycastHit next;
                Physics.Raycast(hit.point, Vector3.forward, out next, _radiusDestruction);
                if (next.collider != null && next.collider.tag == "Obstacle" && next.collider != hit.collider) {
                    next.collider.gameObject.GetComponent<MarchingCubes>().Destruction(hit.point, _radiusDestruction);
                }
                Instantiate(_explosionSoundPrefab, hit.point, Quaternion.identity);
            }
            linePositions[1] = new Vector3(0, 0, laserLength / transform.parent.localScale.y);
            lineRenderer.SetPositions(linePositions.ToArray());

            yield return new WaitForSeconds(_tickRate);
        }
    }

}
