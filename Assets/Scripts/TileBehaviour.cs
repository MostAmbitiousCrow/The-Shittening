using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    private MeshRenderer mr;

    private void Awake() => mr = transform.GetChild(0).GetComponent<MeshRenderer>();
    
    public void SetVisible(bool state)
    {
        if (mr.enabled != state)
            mr.enabled = state;
    }
}