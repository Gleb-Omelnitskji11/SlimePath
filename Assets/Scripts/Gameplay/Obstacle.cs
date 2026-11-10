using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class Obstacle : MonoBehaviour
    {
        public float radius;
 
        public void Explode(float delay, Material explodeMaterial)
        {
            StartCoroutine(ExplodeRoutine(delay, explodeMaterial));
        }
 
        private IEnumerator ExplodeRoutine(float delay, Material explodeMaterial)
        {
            yield return new WaitForSeconds(delay);
 
            var rend = GetComponent<Renderer>();
            rend.material = explodeMaterial;
            Vector3 start = transform.localScale;
 
            float t = 0f;
            const float dur = 0.25f;
            while (t < dur)
            {
                t += Time.deltaTime;
                transform.localScale = start * (1f + t / dur * 0.8f);
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}