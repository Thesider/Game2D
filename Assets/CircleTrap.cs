using UnityEngine;

public class CircleTrap : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float speedMove = 5f;
    public Transform diemA;
    public Transform diemB;


    private Vector3 diemMuctieu; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // This is now correct: assigning a Vector3 (diemA.position) to a Vector3 (diemMuctieu)
        diemMuctieu = diemA.position;
    }

    // Update is called once per frame
    void Update()
    {
        // These functions now work because diemMuctieu is a Vector3
        transform.position = Vector3.MoveTowards(transform.position, diemMuctieu, speedMove * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, diemMuctieu) < 0.1f)
        {
            // This comparison is now correct (Vector3 == Vector3)
            if (diemMuctieu == diemA.position)
            {
                diemMuctieu = diemB.position;
            }
            else
            {
                diemMuctieu = diemA.position;
            }
        }
    }

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, rotationSpeed);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStatus playerStatus = collision.gameObject.GetComponent<PlayerStatus>();
            if (playerStatus != null)
            {
                playerStatus.ForceKill();
                Debug.Log("Player hit Circle Trap -> Health = 0 (Dead)");
            }
        }
    }
}