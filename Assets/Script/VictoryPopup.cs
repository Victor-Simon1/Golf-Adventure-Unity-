using System.Collections;
using TMPro;
using UnityEngine;

public class VictoryPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float duration = 2f;
    [SerializeField] private float decalX = 800f;
    [SerializeField] private float decalY = 40f;
    [SerializeField] private bool isRetracted= true;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private GameObject scoreboard;
    private void Start()
    {
        Vector3 scale = transform.parent.parent.localScale;
        decalX *= scale.x;
        decalY *= scale.y;
    }
    public void ChangeMessage(string str)
    {
        text.text = str;
    }

    public void Pop()
    {
        Vector3 startPosition = GetComponent<RectTransform>().position;
        Vector3 endPosition;
    
        if (isRetracted)
            endPosition = startPosition + new Vector3(decalX, decalY , 0);
        else
            endPosition = startPosition - new Vector3(decalX , decalY , 0);
        if (isMoving)
            return;
        StartCoroutine(Animate(startPosition, endPosition, duration));

    }

    IEnumerator Animate(Vector3 startPosition,Vector3 endPosition,float duration)
    {
        var timeElapsed = 0f;
        isMoving = true;
        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, timeElapsed / duration);
            yield return new WaitForEndOfFrame();
            timeElapsed += Time.deltaTime;
        }
     
        transform.position = Vector3.Lerp(startPosition, endPosition, timeElapsed / duration);
        isRetracted = !isRetracted;
        isMoving = false;
        yield return null;//new WaitForSeconds(pause);
        if (isRetracted)
        {
            scoreboard.SetActive(false);
            text.text = ">";
        }
        else
        {
            scoreboard.SetActive(true);
            text.text = "<";
        }
          

    }
}
