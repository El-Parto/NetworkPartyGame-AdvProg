using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickVisualiser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


}
/* old code
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




public  Material barrierMat;
    public Collider barrierCol;

    private float alphaf;
    // Start is called before the first frame update
    void Start()
    {
        barrierCol = GetComponent<Collider>();
        
        
        barrierCol.enabled = false;
        transform.position += new Vector3(0, 0, 1);
        
        barrierMat.color = new Color(barrierMat.color.r, barrierMat.color.g, barrierMat.color.b, barrierMat.color.a);
        alphaf = barrierMat.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if
	    StartCoroutine(VisualiseKick());
    }


    private IEnumerator VisualiseKick()
    {
        barrierCol.enabled = true;
        
        barrierMat.color = new Color(barrierMat.color.r, barrierMat.color.g, barrierMat.color.b, barrierMat.color.a+1);
        
        yield return new WaitForSeconds(0.4f);
        barrierCol.enabled = true;
        barrierMat.color = new Color(barrierMat.color.r, barrierMat.color.g, barrierMat.color.b, alphaf);

*/