using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorSpec : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_EDITOR
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.x = 0;
        eulerAngles.z = 180;
        transform.eulerAngles = eulerAngles;
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
