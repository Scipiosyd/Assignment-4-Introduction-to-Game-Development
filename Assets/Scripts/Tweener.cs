using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
public class Tweener : MonoBehaviour
{
    //private Tween activeTween;
    private List<Tween> activeTweens = new List<Tween>();
    public bool TweenExists(Transform target)
    {
        foreach (Tween t in activeTweens)
        {
            if (t.Target == target) return true;
        }
        return false;
    }

    public bool AddTween(Transform targetObject, Vector3 startPos, Vector3 endPos, float duration)
    {
        if (TweenExists(targetObject))
        {
            
            return false;
        }
        
        Tween newTween = new Tween(targetObject, startPos, endPos, Time.time, duration);
        activeTweens.Add(newTween);
        return true;
    }


    public void CancelTween(Transform target)
    {
        for (int i = activeTweens.Count - 1; i >= 0; i--)
        {
            if (activeTweens[i].Target == target)
            {
                activeTweens.RemoveAt(i);
            }
        }
    }



    void Update()
    {
        for (int i = activeTweens.Count - 1; i >= 0; i--)
        {
            Tween t = activeTweens[i];

            float elapsed = Time.time - t.StartTime;
            float fraction = elapsed / t.Duration;

            if (fraction >= 1.0f)
            {
                // Tween complete
                t.Target.position = t.EndPos;
                activeTweens.RemoveAt(i);
            }
            else
            {
                // Still tweening
                t.Target.position = Vector3.Lerp(t.StartPos, t.EndPos, fraction);
            }
        }
    }
}