using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    Camera cam;
    SpriteRenderer sr;

    public float parallaxFactor;

    private float currentXPos;
    private float prevXPos;
    private float xOffset;
    private float prevXOffset;
    private Material mat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main;

        mat = sr.material;
        mat.SetTexture("_MainTex", sr.sprite.texture);
    }

    private void Update()
    {
        currentXPos = cam.transform.position.x;
        xOffset = prevXOffset + ((prevXPos - currentXPos) * parallaxFactor * -1f);
        mat.SetFloat("_XOffset", xOffset);

        prevXPos = currentXPos;
        prevXOffset = xOffset;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector2(cam.transform.position.x, cam.transform.position.y);
    }
}
