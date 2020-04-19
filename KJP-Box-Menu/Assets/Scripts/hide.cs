using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hide : MonoBehaviour
{

    public GameObject VisbilityToggle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void RefreshVisibility()
    {
        bool sichtbarkeit;
        sichtbarkeit = !VisbilityToggle.activeSelf;
        VisbilityToggle.SetActive(sichtbarkeit);

    }
}
