using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverAboveFollow : MonoBehaviour
{
    public Transform target;

    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = this.transform.position - target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = target.transform.position + offset;
        this.transform.LookAt(target.transform);
    }
}
