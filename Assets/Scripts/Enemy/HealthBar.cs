using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image foregroundImage;

    void Start()
    {
        // position it below the enemy (parent object, tag "Enemy")
        transform.parent.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);
        transform.parent.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 1, 0);

    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(Camera.main.transform);
        //transform.Rotate(0, 180, 0);
    }

    public void SetHealth(float healthNormalized)
    {
        foregroundImage.fillAmount = healthNormalized;
    }
}
