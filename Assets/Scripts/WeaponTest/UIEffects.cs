using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
public class UIEffects : MonoBehaviour
{
    [SerializeField] private Text brrrtText;
    [SerializeField] private float closure = 0.00000000000005f;

    private float veloc1;
    private bool fading = false;

    public void BrrrtEffect()
    {
        StartCoroutine(FadeInOut(brrrtText, 0.7f, 0.4f, 1.7f));
    }

    private IEnumerator FadeInOut(Text objToFade, float fadein, float fadeout, float spacetime)
    {
        if (!fading)
        {
            fading = true;

            Color newCol = new Color(objToFade.color.r, objToFade.color.g, objToFade.color.b, 1f);
            float t = fadein;

            while (true)
            {
                objToFade.color = new Color(newCol.r, newCol.g, newCol.b, Mathf.SmoothDamp(objToFade.color.a, newCol.a, ref veloc1, t));
                yield return new WaitForEndOfFrame();

                if (Mathf.Abs(1f - objToFade.color.a) <= 0.05f)
                {
                    newCol = new Color(objToFade.color.r, objToFade.color.g, objToFade.color.b, 0f);
                    t = fadeout;
                    yield return new WaitForSeconds(spacetime);
                }

                if (objToFade.color.a - closure <= 0f && newCol.a - Mathf.Epsilon <= 0f)
                {
                    break;
                }
            }

            fading = false;
            yield break;
        }
    }
    private IEnumerator FadeInOut(Image objToFade, float fadein, float fadeout, float spacetime)
    {
        if (!fading)
        {
            fading = true;

            Color newCol = new Color(objToFade.color.r, objToFade.color.g, objToFade.color.b, 1f);
            float t = fadein;

            while (true)
            {
                objToFade.color = new Color(newCol.r, newCol.g, newCol.b, Mathf.SmoothDamp(objToFade.color.a, newCol.a, ref veloc1, t));
                yield return new WaitForEndOfFrame();

                if (Mathf.Abs(1f - objToFade.color.a) <= 0.05f)
                {
                    newCol = new Color(objToFade.color.r, objToFade.color.g, objToFade.color.b, 0f);
                    t = fadeout;
                    yield return new WaitForSeconds(spacetime);
                }

                if (objToFade.color.a - closure <= 0f && newCol.a - Mathf.Epsilon <= 0f)
                {
                    break;
                }
            }

            fading = false;
            yield break;
        }
    }
}
#pragma warning restore 0649