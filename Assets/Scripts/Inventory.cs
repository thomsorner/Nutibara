using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    [Header("Inventario")]
    public List<Item> items = new List<Item>();
    public int selectedIndex = 0; // 0 = manos vacías

    [Header("Item inicial")]
    public Item linternaDefault;   // Asignar en el inspector

    [Header("Referencias")]
    public Transform handTransform;
    private GameObject currentItemInstance;

    // ================= LIFECYCLE =================

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Agregar linterna por defecto
        if (linternaDefault != null)
        {
            AddItem(linternaDefault);
            SelectItem(1);
        }
        else
        {
            Debug.LogWarning("Inventory: No se asignó una linterna por defecto.");
        }
    }

    private void Update()
    {
        HandleScrollInput();
        HandleUseInput();
    }

    // ================= INPUT (NEW INPUT SYSTEM) =================

    private void HandleScrollInput()
    {
        if (Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            ScrollInventory(scroll);
        }
    }

    private void HandleUseInput()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            UseCurrentItem();
        }
    }

    // ================= INVENTORY LOGIC =================

    public void AddItem(Item item)
    {
        items.Add(item);
        Debug.Log($"Añadido {item.itemName} al inventario.");

        if (selectedIndex == 0 && items.Count == 1)
            SelectItem(1);
    }

    public void SelectItem(int index)
    {
        if (index < 0 || index > items.Count) return;

        if (currentItemInstance != null)
            Destroy(currentItemInstance);

        selectedIndex = index;

        if (selectedIndex == 0)
        {
            currentItemInstance = null;
            Debug.Log("Manos vacías");
            return;
        }

        Item item = items[selectedIndex - 1];

        currentItemInstance = Instantiate(item.itemPrefab, handTransform);
        currentItemInstance.transform.localPosition = Vector3.zero;
        currentItemInstance.transform.localRotation = Quaternion.identity;

        if (currentItemInstance.TryGetComponent(out Collider col))
            col.enabled = false;

        if (currentItemInstance.TryGetComponent(out Rigidbody rb))
            rb.isKinematic = true;

        Debug.Log($"Equipado: {item.itemName}");
    }

    public void ScrollInventory(float scroll)
    {
        if (items.Count == 0)
        {
            SelectItem(0);
            return;
        }

        if (scroll > 0)
            selectedIndex++;
        else if (scroll < 0)
            selectedIndex--;

        if (selectedIndex > items.Count)
            selectedIndex = 0;

        if (selectedIndex < 0)
            selectedIndex = items.Count;

        SelectItem(selectedIndex);
    }

    public void UseCurrentItem()
    {
        if (selectedIndex == 0)
        {
            Debug.Log("Intentaste usar las manos vacías.");
            return;
        }

        items[selectedIndex - 1].Use();
    }

    public Item GetCurrentItem()
    {
        if (selectedIndex == 0) return null;
        return items[selectedIndex - 1];
    }
}
