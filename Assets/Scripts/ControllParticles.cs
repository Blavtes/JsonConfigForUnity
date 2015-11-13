using UnityEngine;
using System.Collections;

public class ControllParticles : MonoBehaviour {
    ParticleSystem particleObject;

    void Start()
    {
        particleObject = gameObject.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (particleObject.isStopped)
        {
            Destroy(gameObject);
        }
    }
}
