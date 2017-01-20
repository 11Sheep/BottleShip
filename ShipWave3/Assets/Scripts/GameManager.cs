using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private static float MIN_SCALE_X = 1f;
    private static float MAX_SCALE_X = 3f;
    private static float MIN_SCALE_Y = 1f;
    private static float MAX_SCALE_Y = 3f;

    private float mShipSpeed = 2f;

    private static int NUM_OF_OBJECTS = 20;

    private GameObject mShip;

    float currentX = 0;
    float currentY = 0;
    float lastScaleX = 0;
    float lastScaleY = 0;
    bool lastOneWasUP = false;

    private PathPartInfo[] mParts;

    // Use this for initialization
    void Start () {

        mShip = GameObject.Find("TheShip");

        mParts = new PathPartInfo[NUM_OF_OBJECTS];

        for (int index = 0; index < NUM_OF_OBJECTS; index++)
        {
            GameObject container = new GameObject();
            GameObject triangle = Instantiate(Resources.Load("triangle1", typeof(GameObject))) as GameObject;
            GameObject box = Instantiate(Resources.Load("black_square", typeof(GameObject))) as GameObject;

            triangle.transform.parent = container.transform;
            box.transform.parent = container.transform;

            mParts[index] = new PathPartInfo();
            mParts[index].CreateRandomPart();
            mParts[index].triangle = triangle;
            mParts[index].box = box;
            mParts[index].container = container;
        }

        CreatePath();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // AddToPath();
            mShip.transform.localPosition = new Vector2(mShip.transform.localPosition.x, GetShipHeightAccordingToXPosition());
        }

        mShip.transform.Translate(Vector2.right * Time.deltaTime * mShipSpeed);
        mShip.transform.localPosition = new Vector2(mShip.transform.localPosition.x, GetShipHeightAccordingToXPosition());
    }

    private void AddToPath()
    {
        // Take the first part and create another in the end
        PathPartInfo firstOne = mParts[0];

        for (int index = 1; index < NUM_OF_OBJECTS; index++)
        {
            mParts[index - 1] = mParts[index];
        }

        mParts[NUM_OF_OBJECTS - 1] = firstOne;

        AddOnePath(NUM_OF_OBJECTS - 1);
    }

    private void CreatePath()
    {
        for (int index = 0; index < NUM_OF_OBJECTS; index++)
        {
            AddOnePath(index);
        }
    }

    private void AddOnePath(int index)
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
        float scaleOfBox = 15;
        mParts[index].box.transform.localScale = new Vector2(mParts[index].triangle.transform.localScale.x, scaleOfBox);
        mParts[index].box.transform.localPosition = new Vector2(mParts[index].triangle.transform.localPosition.x, mParts[index].triangle.transform.localPosition.y - mParts[index].triangle.transform.localScale.y / 2 - scaleOfBox / 2);

        lastScaleX = mParts[index].scaleX;
        lastScaleY = mParts[index].scaleY;
        lastOneWasUP = goUp;
    }

    private void MoveObjectToFuturePosition()
    {

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
            scaleY = Random.Range(MIN_SCALE_Y, MAX_SCALE_Y);
            //scaleX = (Random.Range(0, 2) == 0) ? scaleX : -scaleX;
        }
    }

    private float GetShipHeightAccordingToXPosition()
    {
        float returnHeight = 0;

        float shipCurrentX = mShip.transform.localPosition.x;

        for (int index = 0; index < NUM_OF_OBJECTS; index++)
        {
            if (((mParts[index].triangle.transform.localPosition.x + Mathf.Abs(mParts[index].scaleX /2)) >= shipCurrentX)  &&
                   ((mParts[index].triangle.transform.localPosition.x - Mathf.Abs(mParts[index].scaleX / 2)) <= shipCurrentX)) 

            {
                bool isGoingUP = (mParts[index].scaleX < 0);

                // Found the position, now find the exact hight
                float positionInChunk;
                float relativeXPosition;

             //   if (isGoingUP)
               // {
                //    positionInChunk = mParts[index].triangle.transform.localPosition.x - Mathf.Abs(mParts[index].scaleX) - shipCurrentX;
               //     relativeXPosition = positionInChunk / Mathf.Abs(mParts[index].scaleX);
                //}
               // else
               // {
                    positionInChunk = mParts[index].triangle.transform.localPosition.x + Mathf.Abs(mParts[index].scaleX/2) - shipCurrentX;
                    relativeXPosition = positionInChunk / Mathf.Abs(mParts[index].scaleX);
               // }

                returnHeight = mParts[index].triangle.transform.localPosition.y + (relativeXPosition * mParts[index].scaleY);

                Debug.Log("Index found: " + index + ", height: " + returnHeight);
                break;
            }
        }

        return returnHeight;
    }
}
