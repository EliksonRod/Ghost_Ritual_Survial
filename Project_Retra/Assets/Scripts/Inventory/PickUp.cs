using Unity.VisualScripting;
using UnityEngine;

public class PickUp : MonoBehaviour, IInteractable
{
    [SerializeField] float highlightItemDistance = 5f;

    MeshRenderer meshRenderer;
    GameObject PlayerObject;

    bool LookingAt = false;
    
    void Start()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();
        PlayerObject = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        HighlightInteractable();
    }

    void HighlightInteractable()
    {
        float distance = Vector3.Distance(transform.position, PlayerObject.transform.position);

        if (LookingAt)
        {
            //Looking at the item - set to outline layer
            meshRenderer.renderingLayerMask = 8; // Light Layer 3
        }
        else if (distance < highlightItemDistance)
        {
            //Player is close enough to item - set to highlight layer
            meshRenderer.renderingLayerMask = 2; // Light Layer 1
        }
        else
        {
            //Too far to highlight - set to default
            meshRenderer.renderingLayerMask = 1; // Light Layer 2
        }
    }

    public void Interact()
    {
        Debug.Log("Picked up " + gameObject.name);
        Destroy(gameObject);
    }

    public void HoldInteract() { }
    public void CannotInteract() { }
    public void HighlightItem()
    {
        LookingAt = true;
    }
    public void UnHighlightItem()
    {
        LookingAt = false;
    }
}
