using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamCube : MonoBehaviour
{
    public AudioPeer _audioPeer;
    public int _band;
    public float _startScale, _scaleMultiplier;
    public float currScale;

    public bool _useBuffer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_useBuffer)
        {
            transform.localScale = new Vector3(transform.localScale.x, (_audioPeer._bandBuffer[_band] * _scaleMultiplier) + _startScale, transform.localScale.z);
            currScale = transform.localScale.y;
            if (transform.localScale.y >= 10)
            {
                this.GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV();
            }
        } else
        {
            transform.localScale = new Vector3(transform.localScale.x, (_audioPeer._freqBand[_band] * _scaleMultiplier) + _startScale, transform.localScale.z);
        }
    }
}
