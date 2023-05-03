using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightController : MonoBehaviour
{
    public GameObject FlightObject => this.gameObject;
    public Rigidbody FlightBody => FlightObject.GetComponent<Rigidbody>();
    public bool FlightControl;
    public float Acceleration;

    public void CursorToggle(bool toggle)
    {
        if (toggle)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BOOP")
            other.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        CursorToggle(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt) && FlightControl)
        {
            FlightControl = false;
            CursorToggle(true);
        }

        if (!Input.GetKey(KeyCode.LeftAlt) && !FlightControl)
        {
            FlightControl = true;
            CursorToggle(false);
        }

        if (FlightControl)
        {
            FlightObject.transform.Rotate(
                Input.GetAxis("Mouse Y"),
                Input.GetAxis("Mouse X"),
                Input.GetAxis("Roll"),
                Space.Self);

            if (Input.GetButtonDown("Stop"))
                FlightBody.velocity = Vector3.zero;
            else
            {
                FlightBody.AddForce(FlightObject.transform.forward * Input.GetAxis("Linear") * Acceleration);
                FlightBody.AddForce(FlightObject.transform.right * Input.GetAxis("Lateral") * Acceleration);
                FlightBody.AddForce(FlightObject.transform.up * Input.GetAxis("Vertical") * Acceleration);
            }
                
        }
            
    }
}
