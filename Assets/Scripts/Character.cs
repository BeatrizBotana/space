using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected World world;
    protected Core core;
    private GameObject model;

    // Start is called before the first frame update
    public virtual void Start()
    {
        model = transform.Find("Model").gameObject;
        core = GameObject.Find("Core").GetComponent<Core>();
        world = GameObject.Find("World").GetComponent<World>();

        ShowPlayer(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void ShowPlayer(bool show)
    {
        model.SetActive(show);
    }

    public bool IsActive()
    {
        return model.activeSelf;
    }

    public void Die()
    {
        ShowPlayer(false);
    }

    public virtual void Respawn(Vector3 position, float orientation)
    {
        ShowPlayer(true);
    }
}
