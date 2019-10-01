using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameObject av;
    public GameObject pt;
    public string a = "AudioVisual";
    public string p = "Phyllotaxis";
    // Start is called before the first frame update
    void Start()
    {
        pt = GameObject.Find("Phyllotaxis - Center");
        GameObject phylloParent = pt.transform.GetChild(0).gameObject;
        phylloParent.SetActive(false);

        //av = GameObject.Find("AudioVisual - Center");
        //GameObject circleParent = av.transform.GetChild(0).gameObject;
        //circleParent.SetActive(true);

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

            if (circleParent.activeSelf)
            {
                circleParent.SetActive(false);
                phylloParent.SetActive(true);
            } else
            {
                circleParent.SetActive(true);
                phylloParent.SetActive(false);
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
