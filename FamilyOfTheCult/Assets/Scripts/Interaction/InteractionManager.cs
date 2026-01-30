using TMPro;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.UI;
using FpsHorrorKit;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }
    [Header("Interaction Settings")]
    public float interactRange = 2;
    public bool sendRaycast;
    public LayerMask interactableLayers;

    //public AudioProfile defaultInteractAudio;

    //public Audio interactAudio = new Audio();
    //public Inventory Inventory { get; private set; }

    [Header("Highlight Settings")]
    public GameObject interactionUI;
    public TextMeshProUGUI interactTextUI;
    public Image interactImageUI;

    public bool showHighlight;

    private FpsAssetsInputs _input;
    private IInteractable currentInteractable;

    private GameObject defaultHighlightObj;
    private string defaultInteractText;
    [SerializeField] private bool canDragDoor;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _input = FindAnyObjectByType<FpsAssetsInputs>();

        showHighlight = true;
        sendRaycast = true;

        //defaultInteractText = "Press [E] to interact";
        //interactTextUI.text = defaultInteractText;

        //defaultHighlightObj = highlightObject;
    }

    void Update()
    {
       /* if (currentInteractable != null)
        {
            if (Input.GetMouseButton(0) && canDragDoor)
            {
                highlightObject.SetActive(false); // Highlight'i kaldırın
                currentInteractable.HoldInteract();
                sendRaycast = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                UnHighlight();
                currentInteractable.UnHighlight();

                canDragDoor = false;
                sendRaycast = true;
                currentInteractable = null;
            }
        }*/
        if (sendRaycast)
        {
            showHighlight = true;
            SendRaycast();
        }
        else
        {
            showHighlight = false;
        }
    }

    private void SendRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayers))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                currentInteractable = interactable;
                //canDragDoor = true;
                Debug.Log("Interactable found: " + hit.collider.name);
                Highlight();

                if (_input.interact)// && interactionUI.activeSelf)
                {
                    currentInteractable.Interact(); 
                    UnHighlight();
                    _input.interact = false;
                }
            }
            else
            {
                UnHighlight();
            }
        }
        else
        {
            UnHighlight();
        }
    }

    private void Highlight()
    {
        if (currentInteractable != null)
            currentInteractable.HighlightItem();
        
        //highlightObject.SetActive(showHighlight);
    }
    
    private void UnHighlight()
    {
        if (currentInteractable != null)
            currentInteractable.UnHighlightItem();
        //canDragDoor = false;

        //highlightObject.SetActive(false);
        //highlightObject = defaultHighlightObj;
        //interactTextUI.text = defaultInteractText;
    }
    /*


    public void ChangeInteractText(string interactText)
    {
        interactTextUI.text = interactText;
        highlightObject = interactTextUI.gameObject;
    }
    public void ChangeInteractImage(Sprite interactImage)
    {
        interactImageUI.sprite = interactImage;
        highlightObject = interactImageUI.gameObject;
    }*/
}
