using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private static Color[] mColors = 
        {
        new Color(0.45f, 0.94f, 0.99f),
        new Color(0.91f, 0.55f, 0f),
        new Color(0.46f, 0.03f, 0.03f),
        new Color(0.19f, 0, 0.12f),
        new Color(0.02f, 0, 0.06f),
        new Color(0, 0.1f, 0.32f),
        new Color(0.99f, 0.19f, 0.42f),
        new Color(0.99f, 0.65f, 0.45f),
    };

    private int[] mGameDifficaultyPerSection = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    private static float COLOR_CHANGE_TIME = 20f;
    private static float MIN_SCALE_X = 1f;
    private static float MAX_SCALE_X = 3f;
    private static float MIN_SCALE_Y = 1f;
    private static float MAX_SCALE_Y = 1f;

    private float mShipSpeed = 2f;
    private float mShipTurnSpeed = 20;

    private float mCurrentSlipAngle = 0;
    private float mCurrentShipAngle = 0;
    private float mShipAngleExtra = 0; // The adding of the user pressing the buttons

    private SpriteRenderer mBackgroundSprite;

    private int mCurrentPartIndex = 0;

    private static int NUM_OF_OBJECTS = 40;

    private GameObject mShip;
    private GameObject mAll;
    private GameObject mCamera;

    private GameObject mMainCanvas;
    private GameObject mGameCanvas;
    private GameObject mPauseCanvas;
    private GameObject mGameFailedCanvas;
    private GameObject mClouds;

    private GameObject mAnimal1;
    private GameObject mAnimal2;
    private GameObject mAnimal3;

    float currentX = 0;
    float currentY = 0;
    float lastScaleX = 0;
    float lastScaleY = 0;
    bool lastOneWasUP = false;

    bool mDirectionIsUp;

    private PathPartInfo[] mParts;

    private bool mLeftButtonPressed = false;
    private bool mRightButtonPressed = false;

    private float colorTimer = 0;
    private int colorIndex = 0;

    private bool mIsGamePlaying = false;


    // Use this for initialization
    void Start () {

        mShip = GameObject.Find("TheShip");
        mShip.SetActive(false);
        mAll = GameObject.Find("All");
        mCamera = GameObject.Find("Main Camera");
        mBackgroundSprite = GameObject.Find("Background").GetComponent<SpriteRenderer>();

        mClouds = GameObject.Find("Clouds");
        mClouds.SetActive(false);

        mMainCanvas = GameObject.Find("MainCanvas");
        mGameCanvas = GameObject.Find("GameCanvas");
        mPauseCanvas = GameObject.Find("PauseCanvas");
        mGameFailedCanvas = GameObject.Find("GameFailedCanvas");
        mGameCanvas.SetActive(false);
        mPauseCanvas.SetActive(false);
        mGameFailedCanvas.SetActive(false);
    }

    private void StartGame()
    {
        mParts = new PathPartInfo[NUM_OF_OBJECTS];

        for (int index = 0; index < NUM_OF_OBJECTS; index++)
        {
            GameObject container = new GameObject();
            GameObject triangle = Instantiate(Resources.Load("triangle1", typeof(GameObject))) as GameObject;
            GameObject box = Instantiate(Resources.Load("black_square", typeof(GameObject))) as GameObject;

            container.transform.parent = mAll.transform;

            triangle.transform.parent = container.transform;
            box.transform.parent = container.transform;

            mParts[index] = new PathPartInfo();
            mParts[index].CreateFlatPart();
            mParts[index].triangle = triangle;
            mParts[index].box = box;
            mParts[index].container = container;
        }

        CreatePath();

        // Put the ship in the middle
        mShip.transform.position = new Vector2(mParts[NUM_OF_OBJECTS / 2].triangle.transform.localPosition.x, 0);
        mCurrentShipAngle = 90;

        // Put the cargo on the ship
        mAnimal1 = Instantiate(Resources.Load("Animal1", typeof(GameObject))) as GameObject;
        mAnimal1.transform.position = mShip.transform.position + new Vector3(-4f, 1f, 0);

        mAnimal2 = Instantiate(Resources.Load("Animal2", typeof(GameObject))) as GameObject;
        mAnimal2.transform.position = mShip.transform.position + new Vector3(0, 1f, 0);

        mAnimal3 = Instantiate(Resources.Load("Animal3", typeof(GameObject))) as GameObject;
        mAnimal3.transform.position = mShip.transform.position + new Vector3(4f, 1f, 0);
    }

    void Update()
    {
        if (mIsGamePlaying)
        {
            if (mLeftButtonPressed || Input.GetKey(KeyCode.LeftArrow))
            {
                mShipAngleExtra -= 1;
            }
            else if (mRightButtonPressed || Input.GetKey(KeyCode.RightArrow))
            {
                mShipAngleExtra += 1;
            }

            ///mShip.transform.Translate(Vector2.right * Time.deltaTime * mShipSpeed);
            mAll.transform.Translate(-Vector2.right * Time.deltaTime * mShipSpeed);
            mShip.transform.localPosition = new Vector2(mShip.transform.localPosition.x, GetShipHeightAccordingToXPosition() + 2.5f);

            // Take the ship to the right angle
            if (mCurrentShipAngle > mCurrentSlipAngle)
            {
                mCurrentShipAngle -= Time.deltaTime * mShipTurnSpeed;
            }
            else if (mCurrentShipAngle <= mCurrentSlipAngle)
            {
                mCurrentShipAngle += Time.deltaTime * mShipTurnSpeed;
            }

            if (mDirectionIsUp)
            {
                mShip.transform.localEulerAngles = new Vector3(mShip.transform.localEulerAngles.x, mShip.transform.localEulerAngles.y, mCurrentShipAngle - 90);
            }
            else
            {
                mShip.transform.localEulerAngles = new Vector3(mShip.transform.localEulerAngles.x, mShip.transform.localEulerAngles.y, mCurrentShipAngle - 90);
            }

            mBackgroundSprite.color = Color.Lerp(mColors[colorIndex], mColors[colorIndex + 1], colorTimer);

            if (colorTimer < 1)
            { // while t below the end limit...
              // increment it at the desired rate every update:
                colorTimer += Time.deltaTime / COLOR_CHANGE_TIME;
            }
            else
            {
                colorTimer = 0;
                colorIndex++;

                // move to the next color
                if (colorIndex + 1 == mColors.Length)
                {
                    colorIndex = 0;
                }
            }

            // Check if failed (all animatls are down)
            if ((mAnimal1.transform.position.y < -10) &&
                (mAnimal2.transform.position.y < -10) &&
                (mAnimal3.transform.position.y < -10))
            {
                OnGameFailed();
            }
        }
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

        AddOnePath(NUM_OF_OBJECTS - 1, true);
    }

    private void CreatePath()
    {
        for (int index = 0; index < NUM_OF_OBJECTS; index++)
        {
            AddOnePath(index);
        }
    }

    private void AddOnePath(int index, bool makeRandomParams = false)
    {
        if (makeRandomParams)
        {
            mParts[index].CreateRandomPart();
        }

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
        float scaleOfBox = 30;
        mParts[index].box.transform.localScale = new Vector2(mParts[index].triangle.transform.localScale.x, scaleOfBox);
        mParts[index].box.transform.localPosition = new Vector2(mParts[index].triangle.transform.localPosition.x, mParts[index].triangle.transform.localPosition.y - mParts[index].triangle.transform.localScale.y / 2 - scaleOfBox / 2);

        lastScaleX = mParts[index].scaleX;
        lastScaleY = mParts[index].scaleY;
        lastOneWasUP = goUp;
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
            scaleX = (Random.Range(0, 2) == 0) ? scaleX : -scaleX;
        }

        public void CreateFlatPart()
        {
            direction = DirectionEnum.GoingUp;
            scaleX = Random.Range(MIN_SCALE_X, MAX_SCALE_X);
            scaleY = 0.1f;
            scaleX = (Random.Range(0, 2) == 0) ? scaleX : -scaleX;
        }
    }

    private float GetShipHeightAccordingToXPosition()
    {
        float returnHeight = 0;

        float shipCurrentX = mShip.transform.localPosition.x;

        for (int index = 0; index < NUM_OF_OBJECTS; index++)
        {
            if (((mParts[index].triangle.transform.position.x + Mathf.Abs(mParts[index].scaleX /2)) >= shipCurrentX)  &&
                   ((mParts[index].triangle.transform.position.x - Mathf.Abs(mParts[index].scaleX / 2)) <= shipCurrentX)) 

            {
                mCurrentPartIndex = index;

                mDirectionIsUp = (mParts[index].scaleX < 0);

                // Found the position, now find the exact hight
                float positionInChunk;
                float relativeXPosition;

                if (mDirectionIsUp)
                {
                    positionInChunk = Mathf.Abs(mParts[index].scaleX) - (mParts[index].triangle.transform.position.x + Mathf.Abs(mParts[index].scaleX / 2) - shipCurrentX);
                    relativeXPosition = (positionInChunk / Mathf.Abs(mParts[index].scaleX));
                }
                else
                {
                    positionInChunk = Mathf.Abs(mParts[index].scaleX) - ( mParts[index].triangle.transform.position.x + Mathf.Abs(mParts[index].scaleX/2) - shipCurrentX);
                    relativeXPosition = 1 - (positionInChunk / Mathf.Abs(mParts[index].scaleX));
                }

                returnHeight = mParts[index].triangle.transform.position.y + (relativeXPosition * mParts[index].scaleY) - mParts[index].scaleY/2;

                // Debug.Log("...");
                //Debug.Log("x min: " + (mParts[index].triangle.transform.localPosition.x - Mathf.Abs(mParts[index].scaleX / 2)));
                //Debug.Log("x max: " + (mParts[index].triangle.transform.localPosition.x + Mathf.Abs(mParts[index].scaleX / 2)));
                //Debug.Log("Index found: " + index + ",shipCurrentX: " + shipCurrentX + ", height: " + returnHeight);

                if (mDirectionIsUp)
                {
                    mCurrentSlipAngle = 90 - Mathf.Atan(mParts[index].scaleY / mParts[index].scaleX) * 180 / Mathf.PI;
                }
                else
                {
                    mCurrentSlipAngle = 90 - Mathf.Atan(mParts[index].scaleY / mParts[index].scaleX) * 180 / Mathf.PI;
                }

                // Add the user extra power
                mCurrentShipAngle += mShipAngleExtra;
                mShipAngleExtra = 0;
                //mCurrentSlipAngle = (slipAngle < 0) ? (slipAngle + 360) : slipAngle;
                Debug.Log("angle: " + mCurrentSlipAngle + ", ship angle: " + mCurrentShipAngle);

                // Checj if need to add another block at the end (if we are in the middle)
                if (index == (NUM_OF_OBJECTS / 2))
                {
                    AddToPath();
                }
                break;
            }
        }

        return returnHeight;
    }

    /*
    void OnGUI()
    {
        if (GUI.Button(new Rect(200, 70, 100, 100), "Rotate right"))
        {
            mCamera.transform.Rotate(Vector3.forward, 10);
            mShip.transform.Rotate(Vector3.forward, 10);

            Vector2 dir = (Vector2)(Quaternion.Euler(0, 0, mShip.transform.localEulerAngles.z + 90) * Vector2.right);
            Physics2D.gravity = -dir * 9.8f;

            Debug.Log("gravity: " + Physics2D.gravity);
            //Physics2D.gravity = new Vector2(mParts[mCurrentPartIndex].scaleX, mParts[mCurrentPartIndex].scaleY);
        }

        if (GUI.Button(new Rect(50, 70, 100, 100), "Rotate left"))
        {
            mCamera.transform.Rotate(Vector3.forward, -10);
            mShip.transform.Rotate(Vector3.forward, -10);

            Vector2 dir = (Vector2)(Quaternion.Euler(0, 0, mShip.transform.localEulerAngles.z + 90) * Vector2.right);
            Physics2D.gravity = -dir * 9.8f;

            Debug.Log("gravity: " + Physics2D.gravity);
            //Physics2D.gravity = new Vector2(mParts[mCurrentPartIndex].scaleX, mParts[mCurrentPartIndex].scaleY);
        }


    }
    */

    private void RotateScreen(float angle)
    {
        mCamera.transform.localEulerAngles = new Vector3(0, 0, mCamera.transform.localEulerAngles.z + angle);
        //mCamera.transform.Rotate(Vector3.forward, angle);
        //mShip.transform.Rotate(Vector3.forward, angle);
        mShip.transform.localEulerAngles = new Vector3(0, 0, mShip.transform.localEulerAngles.z + angle);
        //mShip.transform.localEulerAngles = new Vector3(0, 0, angle);

        Vector2 dir = (Vector2)(Quaternion.Euler(0, 0, mShip.transform.localEulerAngles.z + 90) * Vector2.right);
        Physics2D.gravity = -dir * 9.8f;

        //Debug.Log("gravity: " + Physics2D.gravity);
    }

    public void OnUIShipLeftPressed()
    {
        mLeftButtonPressed = true;
    }

    public void OnUIShipLeftReleased()
    {
        mLeftButtonPressed = false;
    }

    public void OnUIShipRightPressed()
    {
        mRightButtonPressed = true;
    }

    public void OnUIShipRightReleased()
    {
        mRightButtonPressed = false;
    }

    public void OnUIPlay()
    {
        mMainCanvas.SetActive(false);
        mPauseCanvas.SetActive(false);
        mGameFailedCanvas.SetActive(false);
        mGameCanvas.SetActive(true);

        mShip.SetActive(true);
        mClouds.SetActive(true);

        StartGame();

        mIsGamePlaying = true;
    }

    public void OnPausePressed()
    {
        mIsGamePlaying = false;

        mPauseCanvas.SetActive(true);
        mGameCanvas.SetActive(false);
    }

    public void OnResumePressed()
    {
        mIsGamePlaying = true;

        mPauseCanvas.SetActive(false);
        mGameCanvas.SetActive(true);
    }

    public void OnPlayAgainPressed()
    {
        // Delete all objects
        for (int index = 0; index < NUM_OF_OBJECTS; index++)
        {
            Destroy(mParts[index].container);
        }

        mParts = null;

        mIsGamePlaying = true;

        Destroy(mAnimal1);
        Destroy(mAnimal2);
        Destroy(mAnimal3);

        mGameFailedCanvas.SetActive(false);
        mPauseCanvas.SetActive(false);
        mGameCanvas.SetActive(true);

        StartGame();
    }

    private void OnGameFailed()
    {
        mGameFailedCanvas.SetActive(true);
        mGameCanvas.SetActive(false);

        mIsGamePlaying = false;
    }
}
