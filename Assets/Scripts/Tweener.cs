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
        foreach(Tween t in activeTweens)
        {
            if(t.Target == target) return true;
        }
        return false;
    }
    

    public bool AddTween(Transform targetObject, Vector3 startPos, Vector3 endPos, float duration)
    {

        if (TweenExists(targetObject))
        {
            // Already moving → don't add
            return false;
        }

        // Create a new tween and add it to the list
        Tween newTween = new Tween(targetObject, startPos, endPos, Time.time, duration);
        activeTweens.Add(newTween);
        return true;
    }




// Start is called before the first frame update
void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
        for (int i = activeTweens.Count - 1; i >= 0; i--)
        {
            Tween t = activeTweens[i];

            
            if (Vector3.Distance(t.Target.position, t.EndPos) > 0.1f)
            {
                float elapsed = Time.time - t.StartTime;
                float fraction = Mathf.Clamp01(elapsed / t.Duration);
                t.Target.position = Vector3.Lerp(t.StartPos, t.EndPos, fraction);
            }
            else
            {
               
                t.Target.position = t.EndPos;
                activeTweens.RemoveAt(i);
            }
        }
    }
}
