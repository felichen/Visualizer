using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//MAPPINGS
//SIZE OF OBJECTS IN BACKGROUND CAN GROW/SHRINK BASED ON: audiopeer's average amplitude, or certain frequency bins
//COLOR CHANGE BASED ON: different frequency bin spikes find colorchange)
public class AudioVisual : MonoBehaviour
{
    public AudioAnalyzer _audioAnalyzer;
    public Material matRef;
    public Material particleMat;
    ParticleSystem particles;
    ParticleSystemRenderer psRenderer;
    public ParticleSystem emphasisEmitter;
    private const int SAMPLE_SIZE = 1024;
    public float maxScale = 10.0f;
    public float visualModifier = 175.0f;
    public float smoothing = 20.0f; //buffer for smoother animation
    public float keep = 0.1f;
    public float rotationSpeed = 10f;

    //COLOR PALETTES
    //purple


    //BEAT DETECTION
    public int bpm;
    public float currSongTime;
    public bool isOnBeat;
    public bool spike;

    private AudioSource source;
    public float[] spectrum;

    //CIRCLE VISUALIZATION
    GameObject circleParent;
    private Transform[] cubeTransform; //contains transforms of cubes
    private Transform[] emphasisTransform;
    private GameObject[] emitters;
    private float[] scaleFactor;
    private float cubeWidth = 0.3f;
    private int numVisObjects = 64; //amount of objects

    private Transform[] rmsTransform; //transform for rms
    private Transform[] dbTransform;
    private Transform[] pitchTransform;
    private Transform[] beatTransform;

    private GameObject[] colorCubes;

    //FLYING OBJECTS
    private Transform cameraTransform; //store position of camera
    public Transform[] flyingObjects;
    public Vector3[] finalPos;
    private float flyingSpeed = 50.0f;
    private int numFlying = 50; //number of flying objects
    private float c = 30; //variance of final pos
    private int farBack = 200; //how far back objects spawn

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Main Camera").transform.position = new Vector3(0, 0, -65);
        GameObject.Find("Particle System").transform.position = new Vector3(0, 0, -30);
        //CREATE PARENT
        circleParent = this.transform.GetChild(0).gameObject;

        GameObject ps = GameObject.Find("Particle System");
        particles = ps.GetComponent<ParticleSystem>();
        psRenderer = ps.GetComponent<ParticleSystemRenderer>();


        cameraTransform = GameObject.Find("Main Camera").transform;

        source = GetComponent<AudioSource>();
        spectrum = new float[SAMPLE_SIZE];


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
        emphasisTransform = new Transform[numVisObjects];
        emitters = new GameObject[numVisObjects];

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
            go.transform.parent = circleParent.transform;
            go.transform.rotation = Quaternion.LookRotation(Vector3.forward, pos);
            go.transform.position = pos;
            go.transform.localScale = new Vector3(cubeWidth, 1, 1);
            cubeTransform[i] = go.transform;

            ////create emitter that comes out of bars in center visual
            //create emitter that comes out of bars in center visual
            var pgo = new GameObject("PS" + i);
            pgo.AddComponent<ParticleSystem>();
            ParticleSystem p = pgo.GetComponent<ParticleSystem>();

            //set material
            pgo.GetComponent<ParticleSystemRenderer>().material = particleMat;

            //change shape
            var shape = p.shape;
            ParticleSystemShapeType mesh = ParticleSystemShapeType.Mesh;
            shape.shapeType = mesh;
            var main = p.main;
            main.maxParticles = 1;
            main.startSize = 0.5f;
            //shape.angle = 0;
            p.transform.parent = circleParent.transform;
            p.transform.localPosition = go.transform.localPosition;
            p.transform.localRotation = Quaternion.LookRotation(Vector3.forward, pos);
            p.transform.Rotate(-90, 0, 0);
            p.enableEmission = false;
            //p.transform.localRotation = go.transform.localRotation;
            emphasisTransform[i] = p.transform;
            emitters[i] = pgo;
        }
    }

    void InstantiateRMSDBCube()
    {
        rmsTransform = new Transform[1];
        dbTransform = new Transform[1];
        pitchTransform = new Transform[1];
        beatTransform = new Transform[1];
        colorCubes = new GameObject[1];

        //GameObject rmsgo = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
        //rmsgo.transform.position = new Vector3(-4, 0, 0);
        //rmsTransform[0] = rmsgo.transform;

        //GameObject dbgo = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
        //dbgo.transform.position = new Vector3(-2, 0, 0);
        //dbTransform[0] = dbgo.transform;

        //GameObject pitchgo = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
        //pitchgo.transform.position = new Vector3(0, 0, 0);
        //pitchTransform[0] = pitchgo.transform;

        GameObject beatgo = GameObject.CreatePrimitive(PrimitiveType.Sphere) as GameObject;
        beatgo.GetComponent<Renderer>().receiveShadows = false;
        beatgo.GetComponent<Renderer>().material = matRef;
        beatgo.transform.localScale = new Vector3(2, 2, 2);
        beatTransform[0] = beatgo.transform;

        colorCubes[0] = beatgo;
    }

    void InstantiateFlying() //MUST CALL THIS EVERY SET NUMBER OF MINUTES
    {
        flyingObjects = new Transform[numFlying];
        finalPos = new Vector3[numFlying];

        for (int i = 0; i < numFlying; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere) as GameObject;
            go.transform.localScale *= 3;
            go.transform.position = new Vector3(0, 0, farBack);
            flyingObjects[i] = go.transform;

            //choose final position
            Vector3 cam = cameraTransform.position;
            float x = Random.Range(cam.x - c, cam.x + c);
            float y = Random.Range(cam.y - c, cam.y + c);
            Vector3 end = new Vector3(x, y, cam.z - 10);
            finalPos[i] = end;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //AnalyzeSound();
        UpdateVisual();
        //UpdateRMS();
        //UpdateFlying();
        //UpdateBeat();
    }

    void UpdateVisual() //modify scale of objects
    {
        //ROTATE CIRCLE PARENT
        circleParent.transform.Rotate(Vector3.forward * (rotationSpeed * Time.deltaTime));
        int index = 0;
        int spectrumIndex = 0;
        //only keep certain percentage so not every bar is flat/boring
        int averageSize = (int)((SAMPLE_SIZE * keep) / numVisObjects);

        while (index < numVisObjects)
        {
            float sum = 0;
            for (int j = 0; j < averageSize; j++)
            {
                sum += _audioAnalyzer._samplesLeft[spectrumIndex];
                spectrumIndex++;
            }

            //when moving down, slow; going up, snap
            float scaleY = sum / averageSize * visualModifier;
            scaleFactor[index] -= Time.deltaTime * smoothing; //previous value

            //if going down, scale down smoothly
            if (scaleFactor[index] < scaleY)
                scaleFactor[index] = scaleY;
            if (scaleFactor[index] > 0.3 * maxScale)
            {
                emitParticles(index);
            }
            //if at max size, snap up
            if (scaleFactor[index] > maxScale)
            {
                //ONLY DETECT CERTAIN BINS OF FREQUENCY TO SEE IF THEY REACH MAXSCALE
                if (index >= 5 && index <= 8)
                {
                    changeColor(); //****************************************************************************************************************************************
             
                }
                scaleFactor[index] = maxScale;
            }

            //Vector3 newTrans = new Vector3(0.5f, 1, 1);
            cubeTransform[index].localScale = new Vector3(cubeWidth, 1, 1) + Vector3.up * scaleFactor[index];
            index++;
        }
    }

    void emitParticles(int i)
    {
        ParticleSystem ps = emitters[i].GetComponent<ParticleSystem>();
        //emphasisTransform[i].parent = circleParent.transform;
        //ps.transform.parent = circleParent.transform;
        ps.enableEmission = true;
        ps.Emit(1);
        ps.enableEmission = false;

        //detach from circle so emitter follows one path
        //ps.transform.parent = null;
    }

    void changeColor()
    {
        psRenderer.material.color = UnityEngine.Random.ColorHSV();
        ParticleSystem.EmitParams emitOverride = new ParticleSystem.EmitParams();
        emitOverride.startLifetime = 0.3f;
        particles.Emit(1); 
        colorCubes[0].GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV();
    }

    void UpdateRMS()
    {
        rmsTransform[0].localScale = Vector3.one + Vector3.up * _audioAnalyzer.RMS * 100;
        dbTransform[0].localScale = Vector3.one + Vector3.up * Mathf.Abs(_audioAnalyzer.DB) * 3;
        //check for negative infinity
        if (double.IsNegativeInfinity(_audioAnalyzer.DB))
        {
            dbTransform[0].localScale = Vector3.one + Vector3.up * 0;
        }

        pitchTransform[0].localScale = Vector3.one + Vector3.up * _audioAnalyzer.PITCH / 100;
    }

    void UpdateFlying()
    {
        float c = 100;
        for (int i = 0; i < numFlying; i++)
        {
            flyingObjects[i].localPosition = Vector3.MoveTowards(flyingObjects[i].position, finalPos[i], flyingSpeed * Time.deltaTime);
        }
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
