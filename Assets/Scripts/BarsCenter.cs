using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarsCenter : MonoBehaviour
{
    public AudioAnalyzer _audioAnalyzer;
    GameObject BarParent;

    private const int SAMPLE_SIZE = 1024;

    public float _maxScale = 10.0f;
    public float visualModifier = 175.0f;
    public float smoothing = 20.0f; //buffer for smoother animation
    public float keep = 0.1f;
    private AudioSource source;

    private Transform[] cubeTransform; //contains transforms of cubes
    private float[] scaleFactor;
    private float cubeWidth = 0.2f;
    private int numVisObjects = 32; //amount of objects

    GameObject[] _cubesLeft;
    GameObject[] _cubesRight;

    // Start is called before the first frame update
    void Start()
    {
        //CREATE PARENT
        BarParent = this.transform.GetChild(0).gameObject;
        _cubesLeft = new GameObject[numVisObjects];
        _cubesRight = new GameObject[numVisObjects];

        //create left side
        for (int i = 0; i < numVisObjects; i++)
        {
            //instantiate a gameobject called instanceSampleCube
            GameObject _instanceSampleCube = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            _instanceSampleCube.transform.position = this.transform.position; //place in center of where it is spawned
            _instanceSampleCube.transform.parent = BarParent.transform;
            _instanceSampleCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            _instanceSampleCube.name = "SampleCube" + i;

            _instanceSampleCube.transform.position = new Vector3(i - 33, 0, 0);
            _cubesLeft[i] = _instanceSampleCube;
        }

        //create right side
        //range is number of samples
        for (int i = 0; i < numVisObjects; i++)
        {
            //instantiate a gameobject called instanceSampleCube
            GameObject _instanceSampleCube = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            _instanceSampleCube.transform.position = this.transform.position; //place in center of where it is spawned
            _instanceSampleCube.transform.parent = BarParent.transform;
            _instanceSampleCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            _instanceSampleCube.name = "SampleCube" + i;

            _instanceSampleCube.transform.position = new Vector3(i + 2, 0, 0);
            _cubesRight[i] = _instanceSampleCube;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < numVisObjects; i++)
        {
            if (_cubesLeft != null && _cubesRight != null)
            {
                _cubesLeft[i].transform.localScale = new Vector3(0.1f, (_audioAnalyzer._audioBandBuffer64[i] * _maxScale) + 2 * 0.1f, 0.1f);
                _cubesRight[_cubesRight.Length-1-i].transform.localScale = new Vector3(0.1f, (_audioAnalyzer._audioBandBuffer64[i] * _maxScale) + 2 * 0.1f, 0.1f);

            }
        }
    }
}
