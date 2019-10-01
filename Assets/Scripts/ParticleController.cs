using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    ParticleSystem ps;
    public AudioPeer _audioPeer;
    public int _band;
    public float _startScale, _scaleMultiplier;
    // Start is called before the first frame update
    void Start()
    {
        GameObject particles = GameObject.Find("Particle System");
        ps = particles.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.localScale = new Vector3(transform.localScale.x, (_audioPeer._bandBuffer[_band] * _scaleMultiplier) + _startScale, transform.localScale.z);
        var main = ps.main;
        main.startSize = _audioPeer._AmplitudeBuffer * _scaleMultiplier +_startScale;
        //sh.scale = new Vector3((_audioPeer._AmplitudeBuffer * _scaleMultiplier) + _startScale, (_audioPeer._AmplitudeBuffer * _scaleMultiplier) + (_audioPeer._AmplitudeBuffer * _scaleMultiplier) + _startScale);
    }
}
