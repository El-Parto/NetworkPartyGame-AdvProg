using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickVisualiser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
	    StartCoroutine(VisualiseKick());
    }


    private IEnumerator VisualiseKick()
    {
        gameObject.transform.localScale += new Vector3(6.5f, 0, 6.5f) * Time.deltaTime;
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);

    }
}
