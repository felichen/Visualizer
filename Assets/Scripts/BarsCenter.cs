using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarsCenter : MonoBehaviour
{
    public AudioAnalyzer _audioAnalyzer;
    public AudioVisual _audioVisual;
    GameObject BarParent;

    private const int SAMPLE_SIZE = 1024;

    public float _maxScale = 10.0f;
    public float visualModifier = 175.0f;
    public float smoothing = 20.0f; //buffer for smoother animation
    public float keep = 0.1f;
    public float startSize = 0.2f; //particle size;
    private AudioSource source;

    private Transform[] cubeTransform; //contains transforms of cubes
    private float[] scaleFactor;
    private float cubeWidth = 0.3f;
    private int numVisObjects = 32; //amount of objects

    GameObject[] _cubesLeft;
    GameObject[] _cubesRight;
    public int start;

    //particles
    public ParticleSystem emphasisEmitter;
    public Material matRef;
    private Transform[] emphasisTransformLeftUp;
    private Transform[] emphasisTransformRightUp;
    private Transform[] emphasisTransformLeftDown;
    private Transform[] emphasisTransformRightDown;
    public Material particleMat;

    // Start is called before the first frame update
    void Start()
    {
        //CREATE PARENT
        BarParent = this.transform.GetChild(0).gameObject;
        _cubesLeft = new GameObject[numVisObjects];
        _cubesRight = new GameObject[numVisObjects];
        emphasisTransformLeftUp = new Transform[numVisObjects];
        emphasisTransformRightUp = new Transform[numVisObjects];
        emphasisTransformLeftDown = new Transform[numVisObjects];
        emphasisTransformRightDown = new Transform[numVisObjects];


        //create left side
        for (int i = 0; i < numVisObjects; i++)
        {
            //instantiate a gameobject called instanceSampleCube
            GameObject _instanceSampleCube = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            _instanceSampleCube.transform.position = this.transform.position; //place in center of where it is spawned
            _instanceSampleCube.transform.parent = BarParent.transform;
            _instanceSampleCube.transform.localScale = new Vector3(cubeWidth, 0.1f, 0.1f);
            Color newcol = _audioVisual.lerp((float)i / numVisObjects);
            _instanceSampleCube.GetComponent<Renderer>().material.color = newcol;
            _instanceSampleCube.name = "SampleCube" + i;

            _instanceSampleCube.transform.position = new Vector3(i - 33, 0, 0);
            _cubesLeft[i] = _instanceSampleCube;

            ////create emitter that comes out of bars in center visual up
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
            main.maxParticles = 50;
            main.startSize = startSize;
            main.startColor = newcol;
            //shape.angle = 0;
            p.transform.parent = BarParent.transform;
            p.transform.localPosition = _instanceSampleCube.transform.localPosition;
            p.transform.Rotate(-90, 0, 0);
            p.enableEmission = false;
            //p.transform.localRotation = go.transform.localRotation;
            emphasisTransformLeftUp[i] = p.transform;

            ////create emitter that comes out of bars in center visual down
            var pgod = new GameObject("PSDown" + i);
            pgod.AddComponent<ParticleSystem>();
            ParticleSystem pd = pgod.GetComponent<ParticleSystem>();

            //set material
            pgod.GetComponent<ParticleSystemRenderer>().material = particleMat;

            //change shape
            var shaped = pd.shape;
            ParticleSystemShapeType meshd = ParticleSystemShapeType.Mesh;
            shaped.shapeType = meshd;
            var maind = pd.main;
            maind.maxParticles = 50;
            maind.startSize = startSize;
            maind.startColor = newcol;
            //shape.angle = 0;
            pd.transform.parent = BarParent.transform;
            pd.transform.localPosition = _instanceSampleCube.transform.localPosition;
            pd.transform.Rotate(90, 0, 0);
            pd.enableEmission = false;
            //p.transform.localRotation = go.transform.localRotation;
            emphasisTransformLeftDown[i] = pd.transform;
        }

        //create right side
        //range is number of samples
        for (int i = 0; i < numVisObjects; i++)
        {
            //instantiate a gameobject called instanceSampleCube
            GameObject _instanceSampleCube = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            _instanceSampleCube.transform.position = this.transform.position; //place in center of where it is spawned
            _instanceSampleCube.transform.parent = BarParent.transform;
            _instanceSampleCube.transform.localScale = new Vector3(cubeWidth, 0.1f, 0.1f);
            Color newcol = _audioVisual.lerp((float)(numVisObjects - i) / numVisObjects);
            _instanceSampleCube.GetComponent<Renderer>().material.color = newcol;
            _instanceSampleCube.name = "SampleCube" + i;

            _instanceSampleCube.transform.position = new Vector3(i + 2, 0, 0);
            _cubesRight[i] = _instanceSampleCube;


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
            main.maxParticles = 50;
            main.startSize = startSize;
            main.startColor = newcol;
            //shape.angle = 0;
            p.transform.parent = BarParent.transform;
            p.transform.localPosition = _instanceSampleCube.transform.localPosition;
            p.transform.Rotate(-90, 0, 0);
            p.enableEmission = false;
            //p.transform.localRotation = go.transform.localRotation;
            emphasisTransformRightUp[i] = p.transform;


            ////create emitter that comes out of bars in center visual down
            var pgod = new GameObject("PSRightDown" + i);
            pgod.AddComponent<ParticleSystem>();
            ParticleSystem pd = pgod.GetComponent<ParticleSystem>();

            //set material
            pgod.GetComponent<ParticleSystemRenderer>().material = particleMat;

            //change shape
            var shaped = pd.shape;
            ParticleSystemShapeType meshd = ParticleSystemShapeType.Mesh;
            shaped.shapeType = meshd;
            var maind = pd.main;
            maind.maxParticles = 50;
            maind.startSize = startSize;
            maind.startColor = newcol;
            //shape.angle = 0;
            pd.transform.parent = BarParent.transform;
            pd.transform.localPosition = _instanceSampleCube.transform.localPosition;
            pd.transform.Rotate(90, 0, 0);
            pd.enableEmission = false;
            //p.transform.localRotation = go.transform.localRotation;
            emphasisTransformRightDown[i] = pd.transform;
        }
    }

    public void setColors()
    {
        for (int i = 0; i < numVisObjects; i++)
        {
            Color newcol = _audioVisual.lerp((float)i / numVisObjects);
            _cubesLeft[i].GetComponent<Renderer>().material.color = newcol;
            ParticleSystem ps = emphasisTransformLeftUp[i].gameObject.GetComponent<ParticleSystem>();
            var p = ps.main;
            p.startColor = newcol;
            ParticleSystem psd = emphasisTransformLeftDown[i].gameObject.GetComponent<ParticleSystem>();
            var pd = psd.main;
            pd.startColor = newcol;
        }
        for (int i = 0; i < numVisObjects; i++)
        {
            Color newcol = _audioVisual.lerp((float)(numVisObjects - i) / numVisObjects);
            _cubesRight[i].GetComponent<Renderer>().material.color = newcol;
            ParticleSystem ps = emphasisTransformRightUp[i].gameObject.GetComponent<ParticleSystem>();
            var p = ps.main;
            p.startColor = newcol;
            ParticleSystem psd = emphasisTransformRightDown[i].gameObject.GetComponent<ParticleSystem>();
            var pd = psd.main;
            pd.startColor = newcol;
        }

    }
    // Update is called once per frame
    void Update()
    {
        int count = 0;
        for (int i = start; i < start + 32; i++)
        {
            if (_cubesLeft != null && _cubesRight != null)
            {
                _cubesLeft[count].transform.localScale = new Vector3(cubeWidth, (_audioAnalyzer._audioBandBuffer64[i] * _maxScale) + 2 * 0.1f, 0.1f);
                _cubesRight[_cubesRight.Length-1-count].transform.localScale = new Vector3(cubeWidth, (_audioAnalyzer._audioBandBuffer64[i] * _maxScale) + 2 * 0.1f, 0.1f);

                var a = _cubesLeft[count].transform.localScale;
                if (a.y > 7.5f && BarParent.activeSelf == true)
                {
                    ParticleSystem ps = emphasisTransformLeftUp[count].gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem psR = emphasisTransformRightUp[_cubesRight.Length - 1 - count].gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem psd = emphasisTransformLeftDown[count].gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem psRd = emphasisTransformRightDown[_cubesRight.Length - 1 - count].gameObject.GetComponent<ParticleSystem>();
                    //ps.enableEmission = true;
                    ps.Emit(1);
                    psd.Emit(1);
                    psR.Emit(1);
                    psRd.Emit(1);
                    //ps.enableEmission = false;
                }

            }
            count++;
        }
    }
}
