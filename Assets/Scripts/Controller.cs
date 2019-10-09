using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameObject av;
    public GameObject pt;
    public GameObject bc;
    public Transform[] children;
    public string a = "AudioVisual";
    public string p = "Phyllotaxis";
    // Start is called before the first frame update
    void Start()
    {
        pt = GameObject.Find("Phyllotaxis - Center");
        GameObject phylloParent = pt.transform.GetChild(0).gameObject;
        phylloParent.SetActive(false);

        bc = GameObject.Find("Bars - Center");
        GameObject barParent = bc.transform.GetChild(0).gameObject;
        barParent.SetActive(false);

        av = GameObject.Find("AudioVisual - Center");
        GameObject circleParent = av.transform.GetChild(0).gameObject;
        circleParent.SetActive(true);

        children = circleParent.GetComponentsInChildren<Transform>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            av = GameObject.Find("AudioVisual - Center");
            GameObject circleParent = av.transform.GetChild(0).gameObject;
            pt = GameObject.Find("Phyllotaxis - Center");
            GameObject phylloParent = pt.transform.GetChild(0).gameObject;
            bc = GameObject.Find("Bars - Center");
            GameObject barParent = bc.transform.GetChild(0).gameObject;

            if (circleParent.activeSelf)
            {
                circleParent.SetActive(false);
                foreach (Transform child in children)
                {
                    var name = child.gameObject.name;
                    if (name.Substring(0,2) == "PS")
                    {
                        child.gameObject.SetActive(false);
                        ParticleSystem ps = child.gameObject.GetComponent<ParticleSystem>();
                        ps.enableEmission = false;
                        ps.Stop(true);
                    }
                    
                }
                phylloParent.SetActive(true);
                barParent.SetActive(false);
            } else if (phylloParent.activeSelf)
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
            else if (barParent.activeSelf)
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
        
    }
}
