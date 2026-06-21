using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class FaceCheck : MonoBehaviour
{
    public ItemInterface item;
    // Start is called before the first frame update
    void Start()
    {
        item = null;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            return;
        Debug.Log(other);
        if (other.GetComponent<ItemInterface>() != null)
        {
            item = other.GetComponent<ItemInterface>();
            
        }
    }
}
