using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class Tiling : MonoBehaviour
{
    public int offsetX = 2; // offset so that we dont get any weird errors

    // there are used for checking if we need to instantiate stuff
    public bool hasARightBuddy = false;
    public bool hasALeftBuddy = false;

    public bool reverseScale = false;   // used if the object is not tilable

    private float spriteWidth = 0f; // the width of our element
    private Camera cam;
    private Transform myTransform;

    private void Awake()
    {
        cam = Camera.main;
        myTransform = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
        spriteWidth = sRenderer.sprite.bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        // does it still need buddies? If not, do nothing
        if (!hasALeftBuddy || !hasARightBuddy)
        {
            // calculate the camera's extend (half the width) of what the camera can see in the world coordinates.
            float camHorizontalExtend = cam.orthographicSize * Screen.width / Screen.height;

            // calculate the x position where the camera can see the edge of the sprite(element)
            float edgeVisiblePositionRight = (myTransform.position.x + spriteWidth / 2) - camHorizontalExtend;
            float edgeVisiblePositionLeft = (myTransform.position.x - spriteWidth / 2) + camHorizontalExtend;

            // checking if we can see the edge of the element and calling MakeBuddy if we can
            if (cam.transform.position.x >= edgeVisiblePositionRight - offsetX && hasARightBuddy == false)
            {
                makeNewBuddy(1);
                hasARightBuddy = true;
            }
            else if (cam.transform.position.x <= edgeVisiblePositionLeft + offsetX && hasALeftBuddy == false)
            {
                makeNewBuddy(-1);
                hasALeftBuddy = true;
            }   
        }
    }

    // a function that creates a buddt on the side required
    void makeNewBuddy(int rightOrLeft)
    {
        // calculating the new position of our buddy
        Vector3 newPosition = new Vector3(myTransform.position.x + spriteWidth * rightOrLeft, myTransform.position.y, myTransform.position.z);
        // instantiating our new body and storing it in a variable
        Transform newbuddy = (Transform) Instantiate(myTransform, newPosition, myTransform.rotation); 
        
        // if not tilable let's reverse the x size of our object to get rid of ugle seams
        if (reverseScale == true)
        {
            newbuddy.localScale = new Vector3(newbuddy.localScale.x * -1, newbuddy.localScale.y, newbuddy.localScale.z);
        }

        newbuddy.parent = myTransform.parent;

        if (rightOrLeft > 0)
        {
            newbuddy.GetComponent<Tiling>().hasALeftBuddy = true; 
        }

        else
        {
            newbuddy.GetComponent<Tiling>().hasARightBuddy = true;
        }
    }
}
