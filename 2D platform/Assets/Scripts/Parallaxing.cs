using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour
{ 
    public Transform[] backgrounds; // Array of all back and foregrounds to be parallaxed
    private float[] parallaxScales; // the porportion of the camera's movement to move backgounds by
    public float smoothing = 1f;    // how smooth the parallax is going to be. Make sure to set > 0

    private Transform cam; // reference to the main camera's transform
    private Vector3 previousCamPos; // stores the previous frame's camera position

    // is called before start(). It calls all the logic before start and after objects are set
    // great for references
    private void Awake()
    {
        // set up the camera reference
        cam = Camera.main.transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        // the previous frame that had the current frames's camera position
        previousCamPos = cam.position;

        // assigning corresponding parallexScales
        parallaxScales = new float[backgrounds.Length];
        for (int i = 0; i < backgrounds.Length; i++)
        {
            parallaxScales[i] = backgrounds[i].position.z * -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // for each background
        for (int i = 0; i < backgrounds.Length; i++)
        {
            // the parallax is the opposite of the camera movement
            // because the previous frame multiplied by the scale
            float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];

            // set a target x position which is the current postion + parallax
            float backgroundTargetPosX = backgrounds[i].position.x + parallax;

            // create a target position which is the background's current position with it's target x position
            Vector3 backgoundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);


            // fade between current position and the target position using Lerp
            // deltaTime = frame per second
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgoundTargetPos, smoothing * Time.deltaTime);
        }

        // set previos camPos to camera's position at the end of the frame
        previousCamPos = cam.position;
    }
}
