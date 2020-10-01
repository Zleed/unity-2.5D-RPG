using System;
using UnityEngine;

namespace script
{
    public class CameraControls : MonoBehaviour
    {
        public GameObject map;

        public float panSpeed = 20f;
        public float panBorderThickness = 20f;
        public Vector2 panLimit = new Vector2(300, 300);
        public float scrollSpeed = 20f;
        public float minY = 3f;
        public float maxY = 10f;

        private Camera _cam;
        private TransparencySortMode _sortMode;

        private void Start()
        {
            _cam = gameObject.GetComponent<Camera>();
        }

        private void Update()
        {
            var pos = transform.position;

            if (Input.GetKey("w"))
                // if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
                pos.z += _cam.orthographicSize * Time.deltaTime * 2;

            if (Input.GetKey("s"))
                // if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
                pos.z -= _cam.orthographicSize * Time.deltaTime * 2;

            if (Input.GetKey("d"))
                // if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
                pos.x += _cam.orthographicSize * Time.deltaTime;

            if (Input.GetKey("a"))
                // if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
                pos.x -= _cam.orthographicSize * Time.deltaTime;

            if (Input.GetKeyDown("e"))
            {
                RotateMap();
            }

            // if (Input.touchCount > 0)
            // {
            //     var touch = Input.GetTouch(0);
            //     var xx = Screen.width / 2;
            //     var yy = Screen.height / 2;
            //     if (touch.position.x > xx)
            //         pos.x += panSpeed * Time.deltaTime;
            //     if (touch.position.x < xx)
            //         pos.x -= panSpeed * Time.deltaTime;
            //     if (touch.position.y > yy)
            //         pos.z -= panSpeed * Time.deltaTime;
            //     if (touch.position.y < yy)
            //         pos.z += panSpeed * Time.deltaTime;
            // }

            var scroll = Input.GetAxis("Mouse ScrollWheel");
            var camSize = _cam.orthographicSize - scroll * scrollSpeed * 25f * Time.deltaTime;
            _cam.orthographicSize = Mathf.Clamp(camSize, minY, maxY);

            pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
            pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);
            
            transform.position = pos;
        }

        private void RotateMap()
        {
            _cam.transform.RotateAround(map.transform.position, Vector3.up, 30);
            // _cam.transparencySortAxis = new Vector3();
        }
    }
}