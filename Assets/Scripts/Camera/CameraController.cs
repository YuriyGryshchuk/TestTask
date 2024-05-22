using Cinemachine;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _gunCamera;

    private float _shakeElapsedTime;

    public async void ShakeCamera(float shakeDuration, float maxShakeAmplitude, float maxShakeFrequency)
    {
        var noise = _gunCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (noise == null) return;

        _shakeElapsedTime = shakeDuration;

        while (_shakeElapsedTime > 0)
        {
            if (_shakeElapsedTime > 0)
            {
                float shakeAmplitude = Mathf.Lerp(0, maxShakeAmplitude, _shakeElapsedTime / shakeDuration);
                float shakeFrequency = Mathf.Lerp(0, maxShakeAmplitude, _shakeElapsedTime / shakeDuration);

                noise.m_AmplitudeGain = shakeAmplitude;
                noise.m_FrequencyGain = shakeFrequency;

                _shakeElapsedTime -= Time.deltaTime;
            }
            else
            {
                noise.m_AmplitudeGain = 0f;
                noise.m_FrequencyGain = 0f;
            }

            await UniTask.Yield();
        }

        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }
}
