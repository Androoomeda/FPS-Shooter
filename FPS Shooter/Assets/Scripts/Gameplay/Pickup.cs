using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private Item Item;
    [SerializeField] private float VerticalBobFrequency = 1f;
    [SerializeField] private float BobbingAmount = 1f;
    [SerializeField] private float RotatingSpeed = 360f;

    public Rigidbody Rigidbody {get; private set;}

    private Collider collider;
    private Vector3 startPosition;

    protected virtual void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        collider.isTrigger = true;
        Rigidbody.isKinematic = true;

        startPosition = transform.position;
    }

    void Update()
    {
        float bobbingAnimationPhase = ((Mathf.Sin(Time.time * VerticalBobFrequency) * 0.5f) + 0.5f) * BobbingAmount;
        transform.position = startPosition + Vector3.up * bobbingAnimationPhase;

        transform.Rotate(Vector3.up, RotatingSpeed * Time.deltaTime, Space.Self);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerController>(out PlayerController player))
            OnPicked();
    }

    protected virtual void OnPicked()
    {
        InventoryManager.Instance.AddItem(Item);
        Destroy(gameObject);
    }
}
