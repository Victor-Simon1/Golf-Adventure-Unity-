using System.Collections;
using TMPro;
using UnityEngine;

public class VictoryPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float pause = 2f;
    [SerializeField] private float stepX = -800f;
    [SerializeField] private float stepY = -40f;
    public void ChangeMessage(string str)
    {
        text.text = str;
    }

    public void Pop()
    {
        //gameObject.SetActive(true);
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float t = 0;
        var startPosition = transform.position;
        RectTransform rt = GetComponent<RectTransform>();

        while (t < duration)
        {
            t = Mathf.Min(duration, t + Time.deltaTime / duration);
            Vector3 offset = (new Vector3 (-1* stepX, -1* stepY, 0))*t /* (step * t)*/;
            rt.position = startPosition + offset;
            yield return null;
        }
        
        yield return new WaitForSeconds(pause);
/*
        t = 0f;
        startPosition = transform.position;

        while (t < 1f)
        {
            t = Mathf.Min(1f, t + Time.deltaTime / duration);
            Vector3 offset = (new Vector3(0, 1, 0)) * (step * t);
            rt.position = startPosition + offset;
            yield return null;
        }*/
        //gameObject.SetActive(false);
    }
}
