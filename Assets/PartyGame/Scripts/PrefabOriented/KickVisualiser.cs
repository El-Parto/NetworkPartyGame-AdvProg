using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickVisualiser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale = new Vector3(1, 1,3);
    }

    // Update is called once per frame
    void Update()
    {
	    StartCoroutine(VisualiseKick());
    }


    private IEnumerator VisualiseKick()
    {
        
        gameObject.transform.localScale += new Vector3(1.5f, 0, 4.5f) * Time.deltaTime;
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);

    }
    
    
}
