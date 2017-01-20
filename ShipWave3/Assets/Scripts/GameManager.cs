using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private static float MIN_SCALE_X = 1f;
    private static float MAX_SCALE_X = 3f;
    private static float MIN_SCALE_Y = 1f;
    private static float MAX_SCALE_Y = 3f;

    private PathPartInfo[] mParts;
    //private GameObject[] mObjectPool; 

    // Use this for initialization
    void Start () {
        mParts = new PathPartInfo[10];
       // mObjectPool = new GameObject[10];

       // GameObject go = null;

        for (int index = 0; index < 10; index++)
        {
            GameObject container = new GameObject();
            GameObject triangle = Instantiate(Resources.Load("triangle1", typeof(GameObject))) as GameObject;
            GameObject box = Instantiate(Resources.Load("black_square", typeof(GameObject))) as GameObject;

            triangle.transform.parent = container.transform;
            box.transform.parent = container.transform;

           // mObjectPool[index] = container;
            //mObjectPool[index].SetActive(false);

            mParts[index] = new PathPartInfo();
            mParts[index].CreateRandomPart();
            mParts[index].triangle = triangle;
            mParts[index].box = box;
            mParts[index].container = container;
        }

        CreatePath();
    }


    private void CreatePath()
    {
        float currentX = 0;
        float currentY = 0;
        float lastScaleX = 0;
        float lastScaleY = 0;
        bool lastOneWasUP = false;

        for (int index = 0; index < 10; index++)
        {
            
            
            currentX += (Mathf.Abs(lastScaleX / 2)) + (Mathf.Abs(mParts[index].scaleX) / 2);

            bool goUp = (mParts[index].scaleX < 0);

            if (goUp)
            {
                if (lastOneWasUP)
                {
                    currentY += ((lastScaleY / 2) + (mParts[index].scaleY / 2));
                }
                else
                {
                    currentY += ((mParts[index].scaleY / 2) - (lastScaleY / 2));
                }
            }
            else
            {
                if (lastOneWasUP)
                {
                    currentY -= ((mParts[index].scaleY / 2) - (lastScaleY / 2));
                }
                else
                {
                    currentY -= ((lastScaleY / 2) + (mParts[index].scaleY / 2));
                }
            }

            mParts[index].triangle.transform.localPosition = new Vector2(currentX, currentY);
            mParts[index].triangle.transform.localScale = new Vector2(mParts[index].scaleX, mParts[index].scaleY);

            // Set the box
            float scaleOfBox = 10;
            mParts[index].box.transform.localScale = new Vector2(mParts[index].triangle.transform.localScale.x, scaleOfBox);
            mParts[index].box.transform.localPosition = new Vector2(mParts[index].triangle.transform.localPosition.x, mParts[index].triangle.transform.localPosition.y - mParts[index].triangle.transform.localScale.y/2 - scaleOfBox/2);

            lastScaleX = mParts[index].scaleX;
            lastScaleY = mParts[index].scaleY;
            lastOneWasUP = goUp;
        }
    }

    private class PathPartInfo
    {
        public enum DirectionEnum { GoingUp = 0, GoingDown = 1};
        public DirectionEnum direction;

        public float scaleX;
        public float scaleY;

        public GameObject triangle;
        public GameObject box;
        public GameObject container;

        public void CreateRandomPart()
        {
            direction = (Random.Range((int)0, (int)2) == 0) ? DirectionEnum.GoingUp : DirectionEnum.GoingDown;
            scaleX = Random.Range(MIN_SCALE_X, MAX_SCALE_X);
            scaleY = Random.Range(MIN_SCALE_Y, MIN_SCALE_Y);
            scaleX = (Random.Range(0, 2) == 0) ? scaleX : -scaleX;
        }
    }
}
