using System.Collections;
using PrototypeProject;
using UnityEngine;

public class DoorInteracttion : MonoBehaviour
{
    #region References

    private StarterAssetsInputs _inputs;
    #endregion

    #region Variables

    public float openAngle = 90f;
    public float openSpeed = 3.0f;
    public bool isOpen = false;

    private Quaternion _closedRotation;

    private Quaternion _openRotation;

    //More like a state
    public Coroutine _currentCoroutine;

    #endregion

    #region Functions

    public IEnumerator ToggleDoor()
    {
        Quaternion targetRotaion = isOpen ? _closedRotation : _openRotation;
        
        isOpen = !isOpen;

        while (Quaternion.Angle(transform.rotation, targetRotaion) > 0.01f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotaion, Time.deltaTime * openSpeed);
            yield return null;
        }
        transform.rotation = targetRotaion;
    }

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _openRotation = Quaternion.Euler(0f, openAngle, 0f);
        _closedRotation = transform.rotation;
        _currentCoroutine = StartCoroutine(ToggleDoor());
        _inputs = FindFirstObjectByType<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}