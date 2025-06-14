using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMove : MonoBehaviour {
	public float speed = 25f;

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
