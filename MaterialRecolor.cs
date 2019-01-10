using UnityEngine;

public class MaterialRecolor : MonoBehaviour
{
    public Color ChangeTo = Color.black;

    private void Awake()
    {
        Renderer rendr = this.GetComponent<Renderer>();
        if(rendr) rendr.material.color = this.ChangeTo;
    }
}