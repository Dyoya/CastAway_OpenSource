using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    private List<Collider> colliderList = new List<Collider>(); // �浹�� ������Ʈ�� ������ ����Ʈ

    [SerializeField]
    private int layerGround; // ���� ���̾� (�����ϰ� �� ��)
    private const int IGNORE_RAYCAST_LAYER = 2;  // ignore_raycast (�����ϰ� �� ��)

    [SerializeField]
    private Material green;
    [SerializeField]
    private Material red;

    // ȸ�� �ӵ�
    [SerializeField]
    private float rotationSpeed = 200.0f;


    void Update()
    {
        HandleRotation();
        ChangeColor();
    }

    private void HandleRotation()
    {
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

        // ���콺 ���� ���� �� Y���� �������� ȸ��
        transform.Rotate(Vector3.up, rotationSpeed * scrollWheel * 50.0f);
    }


    private void ChangeColor()
    {
        if (colliderList.Count > 0)
            SetColor(red);
        else
            SetColor(green);
    }

    private void SetColor(Material mat)
    {
        foreach (Transform tf_Child in this.transform)
        {
            Material[] newMaterials = new Material[tf_Child.GetComponent<Renderer>().materials.Length];

            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = mat;
            }

            tf_Child.GetComponent<Renderer>().materials = newMaterials;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER) || other.CompareTag("Tree") || other.CompareTag("Rock"))
            colliderList.Add(other);
    }
   
    private void OnTriggerExit(Collider other)
    {
        if ((other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER) || other.CompareTag("Tree") || other.CompareTag("Rock"))
            colliderList.Remove(other);
    }

    public bool isBuildable()
    {
        return colliderList.Count == 0;
    }
}