using System.Collections;
using UnityEngine;

public class LightCulling : MonoBehaviour
{
    const float LIGHT_CULLING_DISTANCE = 220;
    static int _lightsCount;
    int _lightId;

    Camera _mainCamera;
    Light _light;

    void Start()
    {
        _lightId = _lightsCount++;
        _mainCamera = Camera.main;
        _light = GetComponent<Light>();

        if (_mainCamera == null || _light == null)
            enabled = false;
        else
            StartCoroutine(UpdateCore());
    }

    IEnumerator UpdateCore()
    {
        bool isInActiveRange = false;

        yield return new WaitForSeconds(0.0005f * _lightId); // different delay for each light to compute smoothly
        while (true)
        {
            yield return new WaitForSeconds(0.7f);
            isInActiveRange = Vector3.Distance(transform.position, _mainCamera.transform.position) < LIGHT_CULLING_DISTANCE;
            _light.enabled = isInActiveRange;
        }
    }
}