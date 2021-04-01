using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteFlower : Seed
{
    public GameObject stalk;
    public float pitchChange;

    public AudioSource sound;
    // Start is called before the first frame update

    protected override void Start()
    {
        base.Start();
        sound = gameObject.GetComponent<AudioSource>();

    }
    // Update is called once per frame
    void Update()
    {

    }

    public override void Grow(string name)
    {
        if (GameManager.instance.GetTileUp(x, y).IsEmpty())
        {
            //move up top
            gameObject.transform.SetPositionAndRotation(new Vector3(x + 0.5f, (y + 1) + 0.5f, 0), Quaternion.identity);

            sound.pitch += pitchChange;


            base.Start();
            //Create plant and init
            Plant newStalk = Instantiate(stalk, new Vector3(x + 0.5f, (y - 1) + 0.5f, 0), Quaternion.identity).GetComponent<Plant>();
            newStalk.Init();
            hasBeenWatered = false;
        }

    }

    public override void Remove()
    {
        base.Remove();
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger!");
        if (other.CompareTag("Player"))
        {
            sound.Play();
        }
    }

}
