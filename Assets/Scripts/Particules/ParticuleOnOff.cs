using UnityEngine;

public class ParticuleOnOff : MonoBehaviour
{
    public ParticleSystem partSys;
    public bool loop = true;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (partSys == null)
            partSys = GetComponent<ParticleSystem>();
    }

    public void LoopingOnOff()
    {
        partSys.Stop();
        ParticleSystem.MainModule main = partSys.main;
        main.loop ^= true;
        partSys.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(partSys.main.loop != loop)
        {
            LoopingOnOff();
        }
    }
}
