using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateOpener : MonoBehaviour {


 private Animator anim;
 private int characterNearbyHash = Animator.StringToHash("character_nearby");

 void Start()
    {
        anim = GetComponent<Animator>();
    }


    void Update()
    {
       /* 
       
        var distance = Vector3.Distance(transform.position, character.transform.position);

        if (distanceToOpen >= distance)
        {
            anim.SetBool(characterNearbyHash, true);
        }
        else
        {
            anim.SetBool(characterNearbyHash, false);
        }
        */
    }

    public void OpenClose()
    {

        if (!anim.GetBool(characterNearbyHash))
        {
            anim.SetBool(characterNearbyHash, true);
            GetComponent<MeshCollider>().convex = false;
        }
        else
        {
            anim.SetBool(characterNearbyHash, false);
            GetComponent<MeshCollider>().convex = true;
        }
    }
}
