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
    [SerializeField] private GameObject playerList;

    private void Start()
    {
        Vector3 scale = transform.parent.parent.localScale;
        decalX *= scale.x;
        decalY *= scale.y;

        playerList = transform.GetChild(2).GetChild(1).gameObject;
    }

    public void ChangeMessage(string str)
    {
        text.text = str;
    }

    public void Pop(float _duration = 2f)
    {
        Vector3 startPosition = GetComponent<RectTransform>().position;
        Vector3 endPosition;
        duration = _duration;
        if (isRetracted)
        {
            text.text = "<";
            scoreboard.SetActive(true);
         
            endPosition = startPosition + new Vector3(decalX, decalY, 0);
        }
        else
        {
            text.text = ">";
            endPosition = startPosition - new Vector3(decalX, decalY, 0);
        }
        if (isMoving)
            return;
        StartCoroutine(Animate(startPosition, endPosition, duration));

    }

    IEnumerator Animate(Vector3 startPosition,Vector3 endPosition,float duration)
    {
        var timeElapsed = 0f;
        scoreboard.GetComponent<Scoreboard>().TriPlayer();
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
        }
          

    }
}
