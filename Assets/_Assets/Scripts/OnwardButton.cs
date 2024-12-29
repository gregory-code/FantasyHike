using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnwardButton : MonoBehaviour
{
    [SerializeField] Vector3 shownPos;

    bool alraedyPressed = false;

    public delegate void OnOnward();
    public event OnOnward onOnward;

    public void Onward()
    {
        if (alraedyPressed)
            return;

        alraedyPressed = true;
        onOnward?.Invoke();
    }

    public IEnumerator Show()
    {
        while (Vector3.Distance(transform.localPosition, shownPos) >= 0.1f)
        {
            yield return new WaitForEndOfFrame();
            Vector3 newMovePos = Vector3.Lerp(transform.localPosition, shownPos, 6 * Time.deltaTime);
            transform.localPosition = newMovePos;
        }
    }
}
