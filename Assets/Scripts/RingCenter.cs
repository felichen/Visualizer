using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingCenter : MonoBehaviour
{
    public AudioAnalyzer _audioAnalyzer;
    GameObject RingParent;

    private const int SAMPLE_SIZE = 1024;

    public float _maxScale = 10.0f;
    public float visualModifier = 175.0f;
    public float smoothing = 20.0f; //buffer for smoother animation
    public float keep = 0.1f;
    private AudioSource source;

    private Transform[] cubeTransform; //contains transforms of cubes
    private float[] scaleFactor;
    private float cubeWidth = 0.2f;
    private int numVisObjects = 64; //amount of objects

    GameObject[] _sampleCube;

    // Start is called before the first frame update
    void Start()
    {
        //CREATE PARENT
        this.transform.localRotation = Quaternion.identity;
        RingParent = this.transform.GetChild(0).gameObject;
        _sampleCube = new GameObject[numVisObjects];

        RingParent.transform.Rotate(-15, 0, 0);

        //range is number of samples
        for (int i = 0; i < numVisObjects; i++)
        {
            //instantiate a gameobject called instanceSampleCube
            GameObject _instanceSampleCube = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            _instanceSampleCube.transform.position = this.transform.position; //place in center of where it is spawned
            _instanceSampleCube.transform.parent = RingParent.transform;
            _instanceSampleCube.transform.localScale = new Vector3(cubeWidth, 0.1f, 0.1f);
            _instanceSampleCube.name = "SampleCube" + i;

            //position of cubes into a circle
            float a = 360.0f / numVisObjects;
            //rotate around the y axis to form circle
            this.transform.eulerAngles = new Vector3(0, -a * i, 0);
            //radius of circle
            _instanceSampleCube.transform.position = Vector3.forward * 20;
            _sampleCube[i] = _instanceSampleCube;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(string.Format("hello {0}", this.transform.localRotation.y));

        for (int i = 0; i < numVisObjects; i++)
        {
            if (_sampleCube != null)
            {
                _sampleCube[i].transform.localScale = new Vector3(0.1f, (_audioAnalyzer._audioBandBuffer64[i] * _maxScale) + 2 * 0.1f, 0.1f);
            }
        }
    }
}
