using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisual : MonoBehaviour
{
    private const int SAMPLE_SIZE = 1024;

    public float RMS; //avg output of sound
    public float DB; //sound volume at frame
    public float PITCH; //pitch of note

    public float maxScale = 10.0f;
    public float visualModifier = 175.0f;
    public float smoothing = 20.0f; //buffer for smoother animation
    public float keep = 0.1f;

    public int bpm;
    public float currSongTime;
    public bool isOnBeat;
    public bool spike;

    private AudioSource source;
    private float[] samples;
    public float[] spectrum;
    private float sampleRate;

    private Transform[] cubeTransform; //contains transforms of cubes
    private float[] scaleFactor;
    private int numVisObjects = 64; //amount of objects

    private Transform[] rmsTransform; //transform for rms
    private Transform[] dbTransform;
    private Transform[] pitchTransform;
    private Transform[] beatTransform;

    private GameObject[] colorCubes;

    //FLYING OBJECTS
    private Transform cameraTransform; //store position of camera

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Main Camera").transform.position = new Vector3(0, 0, -65);
        GameObject.Find("Particle System").transform.position = new Vector3(0, 0, -30);
        cameraTransform = GameObject.Find("Main Camera").transform;

        source = GetComponent<AudioSource>();
        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];
        sampleRate = AudioSettings.outputSampleRate;


        InstantiateCircle(); //creates circle at center of screen
        InstantiateRMSDBCube();
        //InstantiateFlying();

        bpm = BPMAnalyzer.AnalyzeBpm(source.clip);

        Debug.Log(string.Format("what is bpm: {0}", bpm));

    }

    void InstantiateCircle()
    {
        scaleFactor = new float[numVisObjects];
        cubeTransform = new Transform[numVisObjects];

        Vector3 center = Vector3.zero;
        float radius = 10.0f;

        for (int i = 0; i < numVisObjects; i++)
        {
            float ang = i * 1.0f / numVisObjects;
            ang = ang * Mathf.PI * 2;

            float x = center.x + Mathf.Cos(ang) * radius;
            float y = center.y + Mathf.Sin(ang) * radius;

            Vector3 pos = center + new Vector3(x, y, 0);
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            go.transform.rotation = Quaternion.LookRotation(Vector3.forward, pos);
            go.transform.position = pos;
            cubeTransform[i] = go.transform;
        }
    }

    void InstantiateRMSDBCube()
    {
        rmsTransform = new Transform[1];
        dbTransform = new Transform[1];
        pitchTransform = new Transform[1];
        beatTransform = new Transform[1];
        colorCubes = new GameObject[1];

        GameObject rmsgo = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
        rmsgo.transform.position = new Vector3(-4, 0, 0);
        rmsTransform[0] = rmsgo.transform;

        GameObject dbgo = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
        dbgo.transform.position = new Vector3(-2, 0, 0);
        dbTransform[0] = dbgo.transform;

        GameObject pitchgo = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
        pitchgo.transform.position = new Vector3(0, 0, 0);
        pitchTransform[0] = pitchgo.transform;

        GameObject beatgo = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
        beatgo.transform.position = new Vector3(2, 0, 0);
        beatTransform[0] = beatgo.transform;

        colorCubes[0] = beatgo;
    }

    // Update is called once per frame
    void Update()
    {
        AnalyzeSound();
        UpdateVisual();
        UpdateRMS();
        //UpdateBeat();
    }

    void UpdateVisual() //modify scale of objects
    {
        if (DB < 0)
        {
            if (Mathf.Abs(Mathf.Abs(DB) - RMS) < 0.5)
            {
                //changeColor();//****************************************************************************************************************************************
            }
        } else if (DB > RMS)
        { 
            //changeColor(); //****************************************************************************************************************************************
        }
        int index = 0;
        int spectrumIndex = 0;
        //only keep certain percentage so not every bar is flat/boring
        int averageSize = (int)((SAMPLE_SIZE * keep) / numVisObjects);

        while (index < numVisObjects)
        {
            float sum = 0;
            for (int j = 0; j < averageSize; j++)
            {
                sum += spectrum[spectrumIndex];
                spectrumIndex++;
            }

            //when moving down, slow; going up, snap
            float scaleY = sum / averageSize * visualModifier;
            scaleFactor[index] -= Time.deltaTime * smoothing; //previous value

            //if going down, scale down smoothly
            if (scaleFactor[index] < scaleY)
                scaleFactor[index] = scaleY;

            //if at max size, snap up
            if (scaleFactor[index] > maxScale)
            {
                //ONLY DETECT CERTAIN BINS
                if (index >= 5 && index <= 10)
                {
                    changeColor(); //****************************************************************************************************************************************
                }
                scaleFactor[index] = maxScale;
            } 


                cubeTransform[index].localScale = Vector3.one + Vector3.up * scaleFactor[index];
            index++;
        }
    }

    void changeColor()
    {
        colorCubes[0].GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV();
    }

    void UpdateRMS()
    {
        rmsTransform[0].localScale = Vector3.one + Vector3.up * RMS * 100;
        dbTransform[0].localScale = Vector3.one + Vector3.up * Mathf.Abs(DB) * 3;
        //check for negative infinity
        if (double.IsNegativeInfinity(DB))
        {
            dbTransform[0].localScale = Vector3.one + Vector3.up * 0;
        }

        pitchTransform[0].localScale = Vector3.one + Vector3.up * PITCH / 100;
    }


    void UpdateBeat()
    {
        //if (AudioProcessor.isOnBeat == true)
        //    beatTransform[0].localScale = Vector3.one + Vector3.up *5;
        //else
        //    beatTransform[0].localScale = Vector3.one + Vector3.up * 1;

        //USE FIRST ONE
        if (SpectralFluxAnalyzer.isOnBeat == true)
            colorCubes[0].GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV();
        //if (spike == true)
        //{
        //    colorCubes[0].GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV();
        //}
        //if (onBeat() == true)
        //{
        //    colorCubes[0].GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV();
        //}
    }
    void AnalyzeSound()
    {
        //get sound spectrum
        source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

   

        source.GetOutputData(samples, 0); //samples array is modified
        //get RMS
        float sum = 0;
        for (int i = 0; i < SAMPLE_SIZE; i++)
        {
            sum += samples[i] * samples[i];
        }
        RMS = Mathf.Sqrt(sum / SAMPLE_SIZE);

        //get DB value 
        DB = 20 * Mathf.Log10(RMS / 0.1f);

        //get pitch
        float maxV = 0;
        var maxN = 0;
        for (int i = 0; i < SAMPLE_SIZE; i++)
        {
            if (!(spectrum[i] > maxV) || !(spectrum[i] > 0.0f))
                continue;
            maxV = spectrum[i];
            maxN = i;
        }

        float freqN = maxN;
        if (maxN > 0 && maxN < SAMPLE_SIZE - 1)
        {
            var dL = spectrum[maxN - 1] / spectrum[maxN];
            var dR = spectrum[maxN + 1] / spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        PITCH = freqN * (sampleRate / 2) / SAMPLE_SIZE;

    }

    bool onBeat()
    {
        currSongTime = source.time;
        float divisor = bpm / 60;
        float epsilon = 0.05f;
        if (currSongTime % divisor < epsilon)
        {
            return true;
        }
        return false;

    }

}
