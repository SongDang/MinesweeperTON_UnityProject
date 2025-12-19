using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanAndZoom : MonoBehaviour
{
    Vector3 touchStart;
    public float zoomOutMin = 1;
    public float zoomOutMax;
    public float moveMax = 7;

    void Start()
    {
        if (Camera.main != null)
        {
            zoomOutMax = Camera.main.orthographicSize;
        }
        else
        {
            Debug.LogError("No main camera found in the scene!"); 
        }
    }

    void Update()
    {
        if(Game.Instance.Bagscreen.activeSelf == false)
        {
            pan();
            zoom(Input.GetAxis("Mouse ScrollWheel"));
        }        
    }

    void pan()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            zoom(difference * 0.01f);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.transform.position += direction;

            float distance = Vector3.Distance(Camera.main.transform.position, Game.Instance.initCameraPosition);
            if (distance > moveMax)
            {
                //Dua camera ve vi tri hop le
                Vector3 directionBack = (Camera.main.transform.position - Game.Instance.initCameraPosition).normalized;
                Camera.main.transform.position = Game.Instance.initCameraPosition + directionBack * moveMax;
            }
        }
    }    
    void zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }

    public void ResetCamera()
    {
        Camera.main.orthographicSize = zoomOutMax; 
    }
        
}