using StarterAssets;
using UnityEngine;

public class SpringPad : MonoBehaviour
{
    public float bounceForce = 20f;

    private void OnTriggerEnter(Collider other)
    {
        PersonController controller = other.GetComponent<PersonController>();
        if (controller != null)
        {
            controller.SpringJump(bounceForce);
            Debug.Log("Spring triggered");
        }
    }

}
