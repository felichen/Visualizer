using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateCubes : MonoBehaviour
{
    public static int numSamples = 256;
    public GameObject _sampleCubePrefab;
    GameObject[] _sampleCube = new GameObject[numSamples];
    public float _maxScale;
    // Start is called before the first frame update
    void Start()
    {
        ////range is number of samples
        //for (int i = 0; i < numSamples; i++)
        //{
        //    //instantiate a gameobject called instanceSampleCube
        //    GameObject _instanceSampleCube = (GameObject)Instantiate(_sampleCubePrefab);
        //    _instanceSampleCube.transform.position = this.transform.position; //place in center of where it is spawned
        //    _instanceSampleCube.transform.parent = this.transform;
        //    _instanceSampleCube.name = "SampleCube" + i;

        //    //position of cubes into a circle
        //    float a = 360.0f / numSamples;
        //    //rotate around the y axis to form circle
        //    this.transform.eulerAngles = new Vector3(0, -a * i, 0);
        //    //radius of circle
        //    _instanceSampleCube.transform.position = Vector3.forward * 200;
        //    _sampleCube[i] = _instanceSampleCube;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //for (int i = 0; i < numSamples; i++)
        //{
        //    if (_sampleCube != null)
        //    {
        //        _sampleCube[i].transform.localScale = new Vector3(10, (AudioPlay._samples[i] * _maxScale) + 2, 10);
        //    }
        //}
    }
}
