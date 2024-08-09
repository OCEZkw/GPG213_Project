using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public LayerMask groundLayer;
    public TextMeshProUGUI interactText;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canInteract;
    private InteractableObject interactableObject;

    public GameObject inventoryMenu;

    private const string MainMenuSceneName = "MainMenu";

    public Quest1 quest;

    void Start()
    {
        List<Quest1> activeQuests = GameManager.Instance.activeQuests;
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on the player!");
        }
        if (interactText == null)
        {
            Debug.LogError("Interact Text (TMP) reference not set in the Inspector!");
        }
        else
        {
            interactText.gameObject.SetActive(false);
        }

        // Load player position if in main menu
        if (SceneManager.GetActiveScene().name == MainMenuSceneName)
        {
            LoadSavedPosition();
        }

        if (inventoryMenu != null)
        {
            inventoryMenu.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Inventory menu is not assigned in the inspector");
        }

    }

    void OnDestroy()
    {
        SaveQuestData();
    }



    private void SaveQuestData()
    {
        // Save the quest data to the GameManager
        foreach (Quest1 quest in GameManager.Instance.activeQuests)
        {
            GameManager.Instance.SaveQuestData(quest);
        }
    }

    private void LoadSavedPosition()
    {
        if (GameManager.Instance.HasSavedPosition(MainMenuSceneName))
        {
            transform.position = GameManager.Instance.GetSavedPlayerPosition(MainMenuSceneName);
        }
    }

    void Update()
    {
        // Check if player is grounded
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);

        // Handle movement
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        Vector2 movement = new Vector2(moveHorizontal * moveSpeed, rb.velocity.y);
        rb.velocity = new Vector2(movement.x, rb.velocity.y);

        // Handle jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        // Handle interaction
        if (Input.GetKeyDown(KeyCode.E) && canInteract)
        {
            Interact();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        InteractableObject interactable = other.GetComponent<InteractableObject>();
        if (interactable != null)
        {
            Debug.Log("Detected interactable object: " + other.gameObject.name);
            canInteract = true;
            interactableObject = interactable;
            if (interactText != null)
            {
                interactText.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<InteractableObject>() != null)
        {
            Debug.Log("Stopped detecting interactable object: " + other.gameObject.name);
            canInteract = false;
            interactableObject = null;
            if (interactText != null)
            {
                interactText.gameObject.SetActive(false);
            }
        }
    }
    private void ToggleInventory()
    {
        if (inventoryMenu != null)
        {
            inventoryMenu.SetActive(!inventoryMenu.activeSelf);
        }
        else
        {
            Debug.LogWarning("Inventory menu is not assigned!");
        }
    }

    void Interact()
    {
        if (interactableObject != null)
        {
            Debug.Log("Interacted with " + interactableObject.name);

            switch (interactableObject.interactionType)
            {
                case InteractionType.LoadScene:
                    if (!string.IsNullOrEmpty(interactableObject.sceneToLoad))
                    {
                        // Save current position before loading new scene
                        GameManager.Instance.SavePlayerPosition(transform.position, SceneManager.GetActiveScene().name);
                        SceneManager.LoadScene(interactableObject.sceneToLoad);
                    }
                    else
                    {
                        Debug.LogWarning("No scene specified for interactable object: " + interactableObject.name);
                    }
                    break;

                case InteractionType.OpenPanel:
                    if (interactableObject.panelToOpen != null)
                    {
                        interactableObject.panelToOpen.SetActive(true);
                    }
                    else
                    {
                        Debug.LogWarning("No panel specified for interactable object: " + interactableObject.name);
                    }
                    break;

                default:
                    Debug.LogWarning("Unknown interaction type for interactable object: " + interactableObject.name);
                    break;
            }
        }
    }
}