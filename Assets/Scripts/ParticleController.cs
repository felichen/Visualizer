using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.Particle[] m_particles;
    public AudioAnalyzer _audioAnalyzer;
    public int _band;
    public float _startScale, _scaleMultiplier;
    // Start is called before the first frame update
    void Start()
    {
        GameObject particles = GameObject.Find("Particle System");
        ps = particles.GetComponent<ParticleSystem>();
        var main = ps.main;
        int num = main.maxParticles;
        m_particles = new ParticleSystem.Particle[num];
    }

    // Update is called once per frame
    void Update()
    {
        //transform.localScale = new Vector3(transform.localScale.x, (_audioPeer._bandBuffer[_band] * _scaleMultiplier) + _startScale, transform.localScale.z);
        var main = ps.main;
        float scale = _audioAnalyzer._AmplitudeBuffer * _scaleMultiplier +_startScale;

        int numAlive = ps.GetParticles(m_particles);

        for (int i = 0; i < numAlive; i++)
        {
            m_particles[i].startSize = scale; 
        }

        ps.SetParticles(m_particles, numAlive);
        //sh.scale = new Vector3((_audioPeer._AmplitudeBuffer * _scaleMultiplier) + _startScale, (_audioPeer._AmplitudeBuffer * _scaleMultiplier) + (_audioPeer._AmplitudeBuffer * _scaleMultiplier) + _startScale);
    }
}
