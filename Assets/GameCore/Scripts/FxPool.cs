using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxPool : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] float timeDelay = 1.5f;
    [SerializeField] ParticleSystem particle;
    public void FillData(Sprite spr, Vector3 pos)
    {
        sprite.sprite = spr;
        transform.position = pos;

        if (!particle)
            return;
        pos.y += .2f;
        transform.localEulerAngles = new Vector3(-90, 0, 0);
        Shader shader = Shader.Find("Mobile/Particles/Alpha Blended");
        Material mat = new Material(shader);
        mat.mainTexture = spr.texture;
        particle.GetComponent<Renderer>().material = mat;
    }
    private void OnEnable()
    {
        Invoke("Delay", timeDelay);

    }
    void Delay()
    {
        gameObject.Recycle();
    }
}
