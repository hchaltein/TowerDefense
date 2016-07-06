using UnityEngine;
using System.Collections;

// Código de movimento do Player de Teste
// Usado para testar colisões, labirinto, torres e A*.
public class TestPlayerBhvr : MonoBehaviour
{
    [SerializeField]
    float MoveSpeed = 1.0f;
	
	void Update ()
    {
        // Movimento ao longo do eixo Z
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.forward *MoveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= Vector3.forward * MoveSpeed * Time.deltaTime;
        }


        // Movimento ao longo do eixo X
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * MoveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= Vector3.right * MoveSpeed * Time.deltaTime;
        }

    }
}
