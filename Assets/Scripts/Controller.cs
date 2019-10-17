using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
public class Controller : MonoBehaviour
{
    public Slider progressbar;
    public AudioAnalyzer _audioAnalyzer;
    public Canvas fcp1;
    public Canvas fcp2;
    public Canvas menuItems;
    public AudioVisual _audioVisual;
    public BarsCenter _barsCenter;
    AudioSource _audioSource;
    float audioLength;

    public TMPro.TMP_Dropdown centerOptions;
    public TMPro.TMP_Dropdown freqOptions;
    public Button colorOne;
    public Button colorTwo;
    public Button Menu;


    public GameObject av;
    public GameObject pt;
    public GameObject bc;
    public Transform[] children;
    public string a = "AudioVisual";
    public string p = "Phyllotaxis";
    // Start is called before the first frame update
    void Start()
    {
        //set audio for progress bar
        _audioSource = _audioAnalyzer._audioSource;
        audioLength = _audioSource.clip.length;

        //add center options
        List<string> center_options = new List<string>() { "Circle", "Travel", "Bars" };
        centerOptions.AddOptions(center_options);
        //add frequency options
        List<string> freq_options = new List<string>() { "Low", "Medium", "High" };
        freqOptions.AddOptions(freq_options);

        //pt = GameObject.Find("Phyllotaxis - Center");
        GameObject phylloParent = pt.transform.GetChild(0).gameObject;
        phylloParent.SetActive(false);

        //bc = GameObject.Find("Bars - Center");
        GameObject barParent = bc.transform.GetChild(0).gameObject;
        barParent.SetActive(false);

        //av = GameObject.Find("AudioVisual - Center");
        GameObject circleParent = av.transform.GetChild(0).gameObject;
        circleParent.SetActive(true);

        children = circleParent.GetComponentsInChildren<Transform>();

        fcp1.gameObject.SetActive(false);
        fcp2.gameObject.SetActive(false);
        menuItems.gameObject.SetActive(false);

    }

    public void centerOption_changed(int index)
    {
        //av = GameObject.Find("AudioVisual - Center");
        GameObject circleParent = av.transform.GetChild(0).gameObject;
        //pt = GameObject.Find("Phyllotaxis - Center");
        GameObject phylloParent = pt.transform.GetChild(0).gameObject;
        //bc = GameObject.Find("Bars - Center");
        GameObject barParent = bc.transform.GetChild(0).gameObject;

        if (index == 1)
        {
            circleParent.SetActive(false);
            foreach (Transform child in children)
            {
                var name = child.gameObject.name;
                if (name.Substring(0, 2) == "PS")
                {
                    child.gameObject.SetActive(false);
                    ParticleSystem ps = child.gameObject.GetComponent<ParticleSystem>();
                    ps.enableEmission = false;
                    ps.Stop(true);
                }

            }
            phylloParent.SetActive(true);
            barParent.SetActive(false);
        }
        else if (index == 2)
        {
            barParent.SetActive(true);
            phylloParent.SetActive(false);
            circleParent.SetActive(false);
            foreach (Transform child in children)
            {
                var name = child.gameObject.name;
                if (name.Substring(0, 2) == "PS")
                {
                    child.gameObject.SetActive(false);
                    ParticleSystem ps = child.gameObject.GetComponent<ParticleSystem>();
                    ps.enableEmission = false;
                    ps.Stop(true);
                }
            }
        }
        else if (index == 0)
        {
            circleParent.SetActive(true);
            foreach (Transform child in children)
            {
                var name = child.gameObject.name;
                if (name.Substring(0, 2) == "PS")
                {
                    child.gameObject.SetActive(true);
                    ParticleSystem ps = child.gameObject.GetComponent<ParticleSystem>();
                    ps.enableEmission = false;
                }
            }
            phylloParent.SetActive(false);
            barParent.SetActive(false);
        }
        //if ((av.GetComponent(a) as MonoBehaviour).enabled == true) {
        //    name = (av.GetComponent(a) as MonoBehaviour).name;
        //    Debug.Log(string.Format("{0}", name));
        //    (av.GetComponent(a) as MonoBehaviour).enabled = false;
        //    //(pt.GetComponent(p) as MonoBehaviour).enabled = true;
        //} else
        //{
        //    Debug.Log(string.Format("REACHED"));
        //    (av.GetComponent(a) as MonoBehaviour).enabled = true;
        //    //(pt.GetComponent(p) as MonoBehaviour).enabled = false;
        //}

    }

    public void freqOption_changed(int index)
    {
        if (index == 0) //sub bass
        {
            _audioVisual.keep = 0.1f;
            _barsCenter.start = 0;
        }
        else if (index == 1) //bass
        {
            _audioVisual.keep = 0.3f;
            _barsCenter.start = 16;
        }
        else if (index == 2) //upper bass
        {
            _audioVisual.keep = 0.5f;
            _barsCenter.start = 32;
        }
        //else if (index == 3) //midrange
        //{
        //    _audioVisual.keep = 0.4f;
        //} else if (index == 4) // uppder midrange
        //{
        //    _audioVisual.keep = 0.5f;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        UpdateProgress();
       
    }

    void UpdateProgress()
    {
        float currTime = _audioSource.time;
        progressbar.value = currTime / audioLength;
    }

    public void colorOne_changed()
    {
        if (fcp1.gameObject.activeSelf == true)
        {
            fcp1.gameObject.SetActive(false);
        } else
        {
            fcp1.gameObject.SetActive(true);
            fcp2.gameObject.SetActive(false);
        }
    }

    public void colorTwo_changed()
    {
        if (fcp2.gameObject.activeSelf == true)
        {
            fcp2.gameObject.SetActive(false);
        }
        else
        {
            fcp2.gameObject.SetActive(true);
            fcp1.gameObject.SetActive(false);
        }
    }

    public void onMenuClick()
    {
        if (menuItems.gameObject.activeSelf == false)
        {
            menuItems.gameObject.SetActive(true);
        } else
        {
            menuItems.gameObject.SetActive(false);
        }
       
    }

}
