using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EndlessGenerator))]
public class GeneratorTester : MonoBehaviour
{
    [SerializeField]
    public bool test = false;

    private EndlessGenerator generator;
    // Start is called before the first frame update
    void Start()
    {
        generator = GetComponent<EndlessGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (test)
        {

        }
    }
}
