using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlay : MonoBehaviour
{
    AudioSource _audioSource;
    public static int numSamples = 256;
    public static int numBands = 8;
    public static float[] _samples = new float[numSamples];
    public static float[] _freqBand = new float[numBands];
    // Start is called before the first frame update
    void Start()
    {
        //_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        //MakeFrequencyBands();
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    //void MakeFrequencyBands()
    //{
    //    //22050 / 512 = 43 hertz per sample
    //    //make 8 frequency bands
    //    /* 0 - 2 = 86hz
    //     * 1 - 4 = 172hz - 87-258
    //     * 2 - 8 = 344hz - 259-602
    //     * 3 - 16 = 688hz 603-1290
    //     * 4 - 32 = 1376hz
    //     * 5 - 64 = 2752hz
    //     * 6 - 128 = 5504hz
    //     * 7 - 256 = 11008hz
    //     * */
    //    int count = 0;
    //    for (int i = 0; i < numBands; i++)
    //    {
    //        float avg = 0; //average of amplitude of all samples combined
    //        int sampleCount = (int)Mathf.Pow(2, i) * 2; // gets 2,4,8,16,etc
    //        if (i == 7)
    //        {
    //            sampleCount += 2;
    //        }
    //        for (int j = 0; j < sampleCount; j++)
    //        {
    //            avg += _samples[count] * (count + 1);
    //                count++;
    //        }
    //        avg /= count;
    //        _freqBand[i] = avg;

    //    }

    //}
}
