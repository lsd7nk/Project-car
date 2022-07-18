using UnityEngine;

public sealed class CarSoundController : MonoBehaviour
{
    [SerializeField] private AudioSource _idleSound;

    public void ReproduceIdleSound(bool state = true)
    {
        if (state)
        {
            _idleSound?.Play();
        }
        else
        {
            _idleSound?.Stop();
        }
    }
}
