using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour
{

    private float rightMax = 9f;
    private float leftMax = -9f;
    private float upMax = 12f;
    private float downMax = 4f;
    private float rotationRightMax = -120f;
    private float rotationLeftMax = -60f;
    private float rotationUpMax = -30f;
    private float rotationDownMax = 30f;
    private int goldMinerals = 0;
    private float currentShootTime = 0;
    private float currentTurnOffLightsTime = 0;
    [SerializeField] private Vector3 _startingPosition = new Vector3(0, 8, -30);
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _rotationSpeed = 1.0f;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Text _textGoldMineral;
    [SerializeField] private GameObject _laserBeamInstance;
    [SerializeField] private float _shootRate;
    [SerializeField] private List<GameObject> _flashLights;
    [SerializeField] private Vector3[] dirsToCheck = {
        new Vector3(0.2f, 0.3f, 1),
        new Vector3(-0.2f, 0.3f, 1),
        new Vector3(0, 1, 0),
    };
    [SerializeField] private float lengthRayCheckFlashlight = 20f;
    [SerializeField] private float turnOffLightsTime = 1f;

    [Header("Sounds")]

    [SerializeField] private GameObject _bulletShootingSound;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = _startingPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = Vector3.zero;
        Vector3 rotateHorizontal = Vector3.right;
        Vector3 rotateVertical = Vector3.up;

        if (Input.GetKey(KeyCode.A)) {
            move += Vector3.left;
            rotateHorizontal = new Vector3(1, 1, 0);
        }
        else if (Input.GetKey(KeyCode.D)) {
            move += Vector3.right;
            rotateHorizontal = new Vector3(1, -1, 0);            
        }
        if (Input.GetKey(KeyCode.S)) {
            move += Vector3.down;
            rotateVertical = new Vector3(0, 1, 1);
        }
        else if (Input.GetKey(KeyCode.W)) {
            move += Vector3.up;
            rotateVertical = new Vector3(0, 1, -1);
        }

        Vector3 previousPosition = transform.position;
        transform.Translate(move * _speed * Time.deltaTime, Space.World);
        
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(Vector3.right, rotateHorizontal) * Quaternion.FromToRotation(Vector3.forward, rotateVertical), Time.deltaTime * _rotationSpeed);
        
        if (transform.position.x > rightMax) {
            transform.position = new Vector3(rightMax, transform.position.y, transform.position.z);
        }
        if (transform.position.x < leftMax) {
            transform.position = new Vector3(leftMax, transform.position.y, transform.position.z);
        }
        if (transform.position.y > upMax) {
            transform.position = new Vector3(transform.position.x, upMax, transform.position.z);
        }
        if (transform.position.y < downMax) {
            transform.position = new Vector3(transform.position.x, downMax, transform.position.z);
        }

        Shoot();
        CameraMovement();
        Flashlights();
    }

    void Shoot() {
        currentShootTime += Time.deltaTime;
        if (Input.GetKey(KeyCode.Mouse0)) {
            if (currentShootTime > _shootRate) {
                currentShootTime = 0;
                Instantiate(_bulletPrefab, transform.position + Vector3.forward + Vector3.left, Quaternion.Euler(270, 0, 0)).GetComponent<Bullet>().SetDir(-transform.up);
                Instantiate(_bulletPrefab, transform.position + Vector3.forward + Vector3.right, Quaternion.Euler(270, 0, 0)).GetComponent<Bullet>().SetDir(-transform.up);
            }
        }
        if (Input.GetKey(KeyCode.Mouse1)) {
            _laserBeamInstance.SetActive(true);
        }
        else {
            _laserBeamInstance.SetActive(false);
        }
    }

    bool isCameraBehind = true;
    void CameraMovement() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            isCameraBehind = !isCameraBehind;
        }
        if (isCameraBehind) {
            Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, new Vector3(transform.position.x, transform.position.y + 1, -39.7f), 5 * Time.deltaTime);
        }
        else {
            Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, transform.position + Vector3.forward * 2f, 5 * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Obstacle") {
            Destroy(gameObject);

        }
        if (other.tag == "Pickup") {
            goldMinerals++;
            _textGoldMineral.text = "Gold minerals collected: " + goldMinerals;
            Destroy(other.gameObject);
        }
    }

    void Flashlights() {
        bool isShipInAsteroid = true;

        for (int i = 0; i < dirsToCheck.Length; i++) {
            RaycastHit hit;
            Physics.Raycast(transform.position + Vector3.forward, dirsToCheck[i], out hit, lengthRayCheckFlashlight);
            if (hit.collider == null) {
                isShipInAsteroid = false;
            }
        }

        if (!isShipInAsteroid) {
            currentTurnOffLightsTime += Time.deltaTime;
            if (currentTurnOffLightsTime > turnOffLightsTime) {
                SetLightsOff();
            }
        }
        else {
            SetLightsOn();
            currentTurnOffLightsTime = 0;
        }

    }

    void SetLightsOn() {
        _flashLights.ForEach(flashLight => {
            flashLight.SetActive(true);
        });
    }

    void SetLightsOff() {
        _flashLights.ForEach(flashLight => {
            flashLight.SetActive(false);
        });
    }
}
