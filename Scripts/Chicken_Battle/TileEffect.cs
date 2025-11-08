using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileEffect : MonoBehaviour
{
    private Sprite gridVisual;
    private SpriteRenderer sp;
    public void StartAnimation(Sprite gridVisual) {
        this.gridVisual = gridVisual;
        sp = GetComponent<SpriteRenderer>();

        StartCoroutine(Animate());
    }

    private IEnumerator Animate() {
        sp.color = Color.white;
        sp.sprite = gridVisual;

        yield return new WaitForSeconds(0.1f);
        
        transform.localScale = transform.localScale + new Vector3(2, 2, 0);
        yield return new WaitForSeconds(0.1f);

        transform.localScale = transform.localScale + new Vector3(2, 2, 0);
        yield return new WaitForSeconds(0.1f);
        
        transform.localScale = transform.localScale + new Vector3(2, 2, 0);
        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
    }
}
