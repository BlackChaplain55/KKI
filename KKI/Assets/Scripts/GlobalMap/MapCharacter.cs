using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class MapCharacter : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject _target;
    [SerializeField] private NavMeshAgent _navAgent;
    [SerializeField] private Animator _anim;
    [SerializeField] private LayerMask _layerMask;
    public void OnPointerClick(PointerEventData eventData)
    {
        _target.transform.position = eventData.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        _target.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            {
                if(Physics.Raycast(ray, out hitInfo, 100,_layerMask))
                {
                    _target.gameObject.SetActive(true);
                    _target.transform.position = hitInfo.point;
                    _target.transform.position = new Vector3(_target.transform.position.x, _target.transform.position.y+1, _target.transform.position.z);
                    _navAgent.SetDestination(_target.transform.position);
                    _anim.SetBool("Run", true);
                }
            }
        }

        if((transform.position - _target.transform.position).magnitude<1f)
        {
            _target.gameObject.SetActive(false);
            _anim.SetBool("Run", false);
        }
    }
}
