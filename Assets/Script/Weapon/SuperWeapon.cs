using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperWeapon : MonoBehaviour, IWeapon
{
    [SerializeField]
    private MeshRenderer meshRenderer;

    [SerializeField]
    private UnityEngine.Video.VideoPlayer superWeapon;

    public void Fire()
    {
        if(!meshRenderer.enabled)
        {
            StartCoroutine(FireImplementation());
        }
    }

    private IEnumerator FireImplementation()
    {
        if(superWeapon.isPlaying)
        {
            superWeapon.Stop();
        }

        meshRenderer.enabled = true;

        superWeapon.Play();

        yield return new WaitForSeconds((float) superWeapon.clip.length);

        meshRenderer.enabled = false;
    }
}
