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

    public float bpm; 

    private AudioSource source;
    private float[] samples;
    private float[] spectrum;
    private float[] curSpectrum;
    private float[] prevSpectrum; //PREV SPECTRUM IN ORDER TO GET SPECTRAL FLUX, COMPARE
    private List<SpectralFluxInfo> spectralFluxSamples;
    private int thresholdWindowSize = 30;
    private int thresholdMultiplier = 10;
    private float sampleRate;

    private Transform[] cubeTransform; //contains transforms of cubes
    private float[] scaleFactor;
    private int numVisObjects = 64; //amount of objects

    private Transform[] rmsTransform; //transform for rms
    private Transform[] dbTransform;
    private Transform[] pitchTransform;

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
        cameraTransform = GameObject.Find("Main Camera").transform;

        source = GetComponent<AudioSource>();
        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];
        curSpectrum = new float[SAMPLE_SIZE];
        prevSpectrum = new float[SAMPLE_SIZE];
        spectralFluxSamples = new List<SpectralFluxInfo>();
        sampleRate = AudioSettings.outputSampleRate;

        InstantiateCircle(); //creates circle at center of screen
        InstantiateRMSDBCube();
        //InstantiateFlying();

        bpm = BPMAnalyzer.finalBPM;
        
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

        GameObject rmsgo = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
        rmsgo.transform.position = new Vector3(-2, 0, 0);
        rmsTransform[0] = rmsgo.transform;

        GameObject dbgo = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
        dbgo.transform.position = new Vector3(0, 0, 0);
        dbTransform[0] = dbgo.transform;

        GameObject pitchgo = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
        pitchgo.transform.position = new Vector3(2, 0, 0);
        pitchTransform[0] = pitchgo.transform;
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
        AnalyzeSound();
        UpdateVisual();
        UpdateRMS();
        analyzeSpectrum(spectrum, source.time);
        //UpdateFlying();
    }

    void UpdateVisual() //modify scale of objects
    {
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
                scaleFactor[index] = maxScale;

            cubeTransform[index].localScale = Vector3.one + Vector3.up * scaleFactor[index];
            index++;
        }
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

    void UpdateFlying()
    {
        float c = 100;
        for (int i = 0; i < numFlying; i++)
        {
            flyingObjects[i].localPosition = Vector3.MoveTowards(flyingObjects[i].position, finalPos[i], flyingSpeed * Time.deltaTime);
        }
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
        if(maxN > 0 && maxN < SAMPLE_SIZE - 1)
        {
            var dL = spectrum[maxN - 1] / spectrum[maxN];
            var dR = spectrum[maxN + 1] / spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        PITCH = freqN * (sampleRate / 2) / SAMPLE_SIZE;

    }

    //**************************************************************************

    public class SpectralFluxInfo
    {
        public float time;
        public float spectralFlux;
        public float threshold;
        public float prunedSpectralFlux;
        public bool isPeak;
    }
    public void setCurSpectrum(float[] spectrum)
    {
        curSpectrum.CopyTo(prevSpectrum, 0);
        spectrum.CopyTo(curSpectrum, 0);
    }

    float calculateRectifiedSpectralFlux()
    {
        float sum = 0f;

        // Aggregate positive changes in spectrum data
        for (int i = 0; i < SAMPLE_SIZE; i++)
        {
            sum += Mathf.Max(0f, curSpectrum[i] - prevSpectrum[i]);
        }
        return sum;
    }

    float getFluxThreshold(int spectralFluxIndex)
    {
        // How many samples in the past and future we include in our average
        int windowStartIndex = Mathf.Max(0, spectralFluxIndex - thresholdWindowSize / 2);
        int windowEndIndex = Mathf.Min(spectralFluxSamples.Count - 1, spectralFluxIndex + thresholdWindowSize / 2);

        // Add up our spectral flux over the window
        float sum = 0f;
        for (int i = windowStartIndex; i < windowEndIndex; i++)
        {
            sum += spectralFluxSamples[i].spectralFlux;
        }

        // Return the average multiplied by our sensitivity multiplier
        float avg = sum / (windowEndIndex - windowStartIndex);
        return avg * thresholdMultiplier;
    }


    float getPrunedSpectralFlux(int spectralFluxIndex)
    {
        return Mathf.Max(0f, spectralFluxSamples[spectralFluxIndex].spectralFlux - spectralFluxSamples[spectralFluxIndex].threshold);
    }
    bool isPeak(int spectralFluxIndex)
    {
        if (spectralFluxSamples[spectralFluxIndex].prunedSpectralFlux > spectralFluxSamples[spectralFluxIndex + 1].prunedSpectralFlux &&
            spectralFluxSamples[spectralFluxIndex].prunedSpectralFlux > spectralFluxSamples[spectralFluxIndex - 1].prunedSpectralFlux)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void analyzeSpectrum(float[] spectrum, float time)
    {
        // Set spectrum
        setCurSpectrum(spectrum);

        // Get current spectral flux from spectrum
        SpectralFluxInfo curInfo = new SpectralFluxInfo();
        curInfo.time = time;
        curInfo.spectralFlux = calculateRectifiedSpectralFlux();
        spectralFluxSamples.Add(curInfo);

        // We have enough samples to detect a peak
        if (spectralFluxSamples.Count >= thresholdWindowSize)
        {
            int indexToProcess = thresholdWindowSize / 2;
            // Get Flux threshold of time window surrounding index to process
            spectralFluxSamples[indexToProcess].threshold = getFluxThreshold(indexToProcess);

            // Only keep amp amount above threshold to allow peak filtering
            spectralFluxSamples[indexToProcess].prunedSpectralFlux = getPrunedSpectralFlux(indexToProcess);

            // Now that we are processed at n, n-1 has neighbors (n-2, n) to determine peak
            int indexToDetectPeak = indexToProcess - 1;

            bool curPeak = isPeak(indexToDetectPeak);

            if (curPeak)
            {
                spectralFluxSamples[indexToDetectPeak].isPeak = true;
            }
            indexToProcess++;
        }
        else
        {
            Debug.Log(string.Format("Not ready yet.  At spectral flux sample size of {0} growing to {1}", spectralFluxSamples.Count, thresholdWindowSize));
        }
    }

}
