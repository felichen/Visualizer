using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class AudioAnalyzer : MonoBehaviour
{

    //8 OR 64; THE FREQUENCY BANDS REPRESENTS SPECIFIC SUBSET OF FREQUENCIES, LIKE BASS, SUB-BASS, ETC
    public AudioSource _audioSource;
    public float[] _samplesLeft = new float[1024];
    public float[] _samplesRight = new float[1024];


    public float RMS; //avg output of sound
    public float DB; //sound volume at frame
    public float PITCH; //pitch of note
    private float[] samples;
    public float[] spectrum;
    private float sampleRate;
    private const int SAMPLE_SIZE = 1024;

    public float[] _freqBand = new float[8];
    public float[] _bandBuffer = new float[8];
    private float[] _bufferDecrease = new float[8];
    private float[] _freqBandHighest = new float[8];

    //audio64
    private float[] _freqBand64 = new float[64];
    public float[] _bandBuffer64 = new float[64];
    private float[] _bufferDecrease64 = new float[64];
    private float[] _freqBandHighest64 = new float[64];

    [HideInInspector]
    public float[] _audioBand, _audioBandBuffer;

    [HideInInspector]
    public float[] _audioBand64, _audioBandBuffer64;

    [HideInInspector]
    public float _Amplitude, _AmplitudeBuffer;
    private float _AmplitudeHighest;
    public float _audioProfile = 5;

    public enum _channel {Stereo, Left, Right}
    public _channel channel = new _channel();

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Main Camera").transform.position = new Vector3(0, 0, -65);
        GameObject ps = GameObject.Find("Particle System");
        if (ps != null)
        {
            ps.transform.position = new Vector3(0, 0, -30);
        }
        _audioBand = new float[8];
        _audioBandBuffer = new float[8];
        _audioBand64 = new float[64];
        _audioBandBuffer64 = new float[64];
        _audioSource = GetComponent<AudioSource>();
        AudioProfile(_audioProfile);

        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];
        sampleRate = AudioSettings.outputSampleRate;
    }

    // Update is called once per frame
    void Update()
    {
        AnalyzeSound();
        MakeFrequencyBands();
        MakeFrequencyBands64();
        BandBuffer();
        BandBuffer64();
        CreateAudioBands();
        CreateAudioBands64();
        GetAmplitude();
    }

    void AudioProfile(float audioProfile)
    {
        for (int i = 0; i < 8; i++)
        {
            _freqBandHighest[i] = audioProfile;

        }
    }

    void GetAmplitude()
    {
        float _CurrentAmplitude = 0;
        float _CurrentAmplitudeBuffer = 0;
        for (int i = 0; i < 8; i++)
        {
            _CurrentAmplitude += _audioBand[i];
            _CurrentAmplitudeBuffer += _audioBandBuffer[i];
        }
        if (_CurrentAmplitude > _AmplitudeHighest)
        {
            _AmplitudeHighest = _CurrentAmplitude;
        }
        _Amplitude = _CurrentAmplitude / _AmplitudeHighest;
        _AmplitudeBuffer = _CurrentAmplitudeBuffer / _AmplitudeHighest;

    }
    void CreateAudioBands()
    {
        for (int i = 0; i < 8; i++)
        {
            if (_freqBand[i] > _freqBandHighest[i])
            {
                _freqBandHighest [i] = _freqBand[i];
            }
            _audioBand[i] = (_freqBand[i] / _freqBandHighest[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHighest[i]);
        }
    }

    void CreateAudioBands64()
    {
        for (int i = 0; i < 64; i++)
        {
            if (_freqBand64[i] > _freqBandHighest64[i])
            {
                _freqBandHighest64[i] = _freqBand64[i];
            }
            _audioBand64[i] = (_freqBand64[i] / _freqBandHighest64[i]);
            _audioBandBuffer64[i] = (_bandBuffer64[i] / _freqBandHighest64[i]);
        }
    }

    void AnalyzeSound()
    {
        _audioSource.GetSpectrumData(_samplesLeft, 0, FFTWindow.BlackmanHarris);
        _audioSource.GetSpectrumData(_samplesRight, 1, FFTWindow.BlackmanHarris);

        _audioSource.GetOutputData(samples, 0); //samples array is modified
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

    void BandBuffer()
    {
        for (int g = 0; g < 8; ++g)
        {
            if (_freqBand[g] > _bandBuffer[g])
            {
                _bandBuffer[g] = _freqBand[g];
                _bufferDecrease[g] = 0.005f;
            }
            if (_freqBand[g] < _bandBuffer[g])
            {
                _bandBuffer[g] -= _bufferDecrease[g];
                _bufferDecrease[g] *= 1.2f;
            }
        }
    }

    void BandBuffer64()
    {
        for (int g = 0; g < 64; ++g)
        {
            if (_freqBand64[g] > _bandBuffer64[g])
            {
                _bandBuffer64[g] = _freqBand64[g];
                _bufferDecrease64[g] = 0.005f;
            }
            if (_freqBand64[g] < _bandBuffer64[g])
            {
                _bandBuffer64[g] -= _bufferDecrease64[g];
                _bufferDecrease64[g] *= 1.2f;
            }
        }
    }

    void MakeFrequencyBands()
    {
        /*
         * 22050 / 1024 = 21.5
         * 20 - 60
         * 60-250
         * 250-500
         * 500-2000
         * 2000-4000
         * 4000-6000
         * 6000-20000
         * 
         * 0 - 4  = 86 
         * 1 - 8 = 
         * 2 - 16
         * 3 - 32
         * 4 - 64
         * 5 - 128
         * 6 - 256
         * 7 - 512
         */

        Debug.Log(string.Format("arraylength: {0}", _samplesLeft.Length));
        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 4;
            if (i == 7)
            {
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {
                if (channel == _channel.Stereo)
                {
                    average += _samplesLeft[count] + _samplesRight[count] * (count + 1);
                    count++;
                }
                if (channel == _channel.Left)
                {
                    average += _samplesLeft[count] * (count + 1);
                    count++;
                }
                if (channel == _channel.Right)
                {
                    average += _samplesRight[count] * (count + 1);
                    count++;
                }
            }
            average /= count;
            _freqBand[i] = average * 10;
        }
    }

    void MakeFrequencyBands64()
    {
        /*
         * 0-15 = 2 = 32
         * 16-31 = 4 = 64
         * 32-39 = 8 = 64
         * 40-47 = 12 = 96
         * 48-55 = 32 = 256
         * 56-63 = 64 = 512
         */
        int count = 0;
        int sampleCount = 1;
        int power = 0;

        for (int i = 0; i < 64; i++)
        {
            float average = 0;
            if (i == 16 || i == 32 || i == 40 || i ==48 || i == 56)
            {
                power++;
                sampleCount = (int)Mathf.Pow(2, power) * 2;
                if (power == 3)
                {
                    sampleCount -= 2;
                }
            }
            for (int j = 0; j < sampleCount; j++)
            {
                if (channel == _channel.Stereo)
                {
                    average += _samplesLeft[count] + _samplesRight[count] * (count + 1);
                    count++;
                }
                if (channel == _channel.Left)
                {
                    average += _samplesLeft[count] * (count + 1);
                    count++;
                }
                if (channel == _channel.Right)
                {
                    average += _samplesRight[count] * (count + 1);
                    count++;
                }
            }
            average /= count;
            _freqBand64[i] = average * 80;
        }
    }
}
