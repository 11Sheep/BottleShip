using UnityEngine;
using System.Collections;

public class CloudsMoveScript : MonoBehaviour
{
    public float MOVE_SPEED = 30f;

   // private float cloudsImageWidth;

    // Use this for initialization
    void Start()
    {
        //cloudsImageWidth = ((RectTransform)gameObject.transform).rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(Vector3.left * MOVE_SPEED * Time.deltaTime);

        if (gameObject.transform.localPosition.x < Camera.main.transform.localPosition.x - (Constants.SCREEN_WIDTH / 50))
        {
            gameObject.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x + (Constants.SCREEN_WIDTH / 50), gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
        }
    }
}
