using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PlayerVFX : MonoBehaviour
{
    public VisualEffect footstep;
    public ParticleSystem Blade_1;
    public ParticleSystem Blade_2;
    public ParticleSystem Blade_3;
    public VisualEffect Slash;
    public VisualEffect HealVFX;
    public void Update_Footstep(bool state)
    {
        if (state)
            footstep.Play();
        else
            footstep.Stop();
    }

    public void PlayBlade1()
    {
        Blade_1.Play();
    }

    public void PlayBlade2()
    {
        Blade_2.Play();
    }

    public void PlayBlade3()
    {
        Blade_3.Play();
    }

    public void StopBlade()
    {
        Blade_1.Simulate(0);
        Blade_1.Stop();

        Blade_2.Simulate(0);
        Blade_2.Stop();

        Blade_3.Simulate(0);
        Blade_3.Stop();
    }

    public void PlaySlash(Vector3 pos)
    {
        Slash.transform.position = pos;
        Slash.Play();
    }

    public void PlayHealVFX()
    {
        HealVFX.Play();
    }
}
