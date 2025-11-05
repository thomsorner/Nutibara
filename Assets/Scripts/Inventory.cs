using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    [Header("Inventario")]
    public List<Item> items = new List<Item>();
    public int selectedIndex = 0; // 0 = manos vacías

    [Header("Referencias")]
    public Transform handTransform; // Aquí se instancia el objeto en primera persona
    private GameObject currentItemInstance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        // Detección del scroll del mouse
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            ScrollInventory(scroll);
        }

        // Usar ítem actual con click izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            UseCurrentItem();
        }
    }

    public void AddItem(Item item)
    {
        items.Add(item);

        Debug.Log($"Añadido {item.itemName} al inventario.");

        // Si es el primer ítem y estabas en manos vacías, queda en el slot vacío
        if (selectedIndex == 0 && items.Count == 1)
        {
            SelectItem(0); // manos vacías
        }
    }

    public void SelectItem(int index)
    {
        // index 0 siempre permitido (manos vacías)
        if (index < 0 || index > items.Count) return;

        // Limpia el objeto anterior en la mano
        if (currentItemInstance != null)
            Destroy(currentItemInstance);

        selectedIndex = index;

        if (selectedIndex == 0)
        {
            // Slot vacío → manos vacías
            currentItemInstance = null;
            Debug.Log("Manos vacías");
            return;
        }

        // Instanciamos el ítem correspondiente en la mano
        Item item = items[selectedIndex - 1]; // -1 porque la lista no incluye slot vacío

        if (item.itemPrefab == null)
        {
            Debug.LogWarning($"El item {item.itemName} no tiene prefab asignado.");
            return;
        }

        currentItemInstance = Instantiate(item.itemPrefab, handTransform);
        currentItemInstance.transform.localPosition = Vector3.zero;
        currentItemInstance.transform.localRotation = Quaternion.identity;

        // Deshabilitar físicas en la versión "en mano"
        if (currentItemInstance.TryGetComponent<Collider>(out Collider col))
            col.enabled = false;

        if (currentItemInstance.TryGetComponent<Rigidbody>(out Rigidbody rb))
            rb.isKinematic = true;

        Debug.Log($"Equipado: {item.itemName}");
    }

    public void ScrollInventory(float scroll)
    {
        if (items.Count == 0)
        {
            // Si no hay ítems, solo se muestran manos vacías
            selectedIndex = 0;
            SelectItem(0);
            return;
        }

        if (scroll > 0) selectedIndex++;
        else if (scroll < 0) selectedIndex--;

        // Wrap circular
        if (selectedIndex > items.Count) selectedIndex = 0;
        if (selectedIndex < 0) selectedIndex = items.Count;

        SelectItem(selectedIndex);
    }

    public void UseCurrentItem()
    {
        if (selectedIndex == 0)
        {
            Debug.Log("Intentaste usar las manos vacías.");
            return; // slot vacío
        }

        items[selectedIndex - 1].Use();
    }

    public Item GetCurrentItem()
    {
        if (selectedIndex == 0) return null;
        return items[selectedIndex - 1];
    }
}
