using UnityEngine;
using System.Collections;

public class UIEffectHelper : MonoBehaviour
{
    public CanvasGroup Group;
    public CanvasGroup MainGroup;
    
    public float FadeDuration = 0.5f;

    public GameObject Speed;
    public GameObject Claws;
    public GameObject Balls;
    public GameObject Finish;
    public GameObject MessageTop;

    public GameObject Tutorial1;
    public GameObject Tutorial2;

    private Coroutine currentCoroutine;

    private void HideAll()
    {
        Speed.SetActive(false);
        Claws.SetActive(false);
        Balls.SetActive(false);
    }

    public void ShowSpeed()
    {
        HideAll();
        Speed.SetActive(true);
        Show();
    }

    public void ShowClaws()
    {
        HideAll();
        Claws.SetActive(true);
        Show();
    }

    public void ShowBalls()
    {
        HideAll();
        Balls.SetActive(true);
        Show();
    }

    public void ShowFinish()
    {
        HideAll();
        Finish.SetActive(true);
        Show();
        StartCoroutine(DisableFinish());
    }

    private IEnumerator DisableFinish()
    {
        yield return new WaitForSeconds(10f);
        Finish.SetActive(false);
    }

    private void Start()
    {
        StartTutorialSequence();
    }

    public void StartTutorialSequence()
    {
        StartCoroutine(TutorialSequence());
    }

    private IEnumerator TutorialSequence()
    {
        HideAll();

        Tutorial1.SetActive(true);
        yield return StartCoroutine(FadeCanvasGroup(1f, MainGroup));

        yield return new WaitForSeconds(5f);
        yield return new WaitUntil(() => Input.anyKey);

        yield return StartCoroutine(FadeCanvasGroup(0f,MainGroup));
        Tutorial1.SetActive(false);

        Tutorial2.SetActive(true);
        yield return StartCoroutine(FadeCanvasGroup(1f,MainGroup));
        yield return new WaitForSeconds(10f);
        yield return new WaitUntil(() => Input.anyKey);
        
        yield return StartCoroutine(FadeCanvasGroup(0f,MainGroup));
        Tutorial2.SetActive(false);

        currentCoroutine = null;
    }

    public void Show()
    {
        if (Tutorial1.gameObject.activeSelf || Tutorial2.gameObject.activeSelf)
        {
            MainGroup.alpha = 0;
            Tutorial1.SetActive(false);
            Tutorial2.SetActive(false);
        }
        
        if (Group == null) return;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(FadeAndAutoHide());
    }

    private IEnumerator FadeAndAutoHide()
    {
        yield return StartCoroutine(FadeCanvasGroup(1f, Group));
        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(FadeCanvasGroup(0f, Group));
        currentCoroutine = null;
    }
    
    private IEnumerator FadeCanvasGroup(float targetAlpha, CanvasGroup group)
    {
        float startAlpha = group.alpha;
        float elapsed = 0f;

        if (targetAlpha > 0f)
        {
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        while (elapsed < FadeDuration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / FadeDuration);
            yield return null;
        }

        group.alpha = targetAlpha;

        if (targetAlpha == 0f)
        {
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }
}
