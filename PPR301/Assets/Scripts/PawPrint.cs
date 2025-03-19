using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawPrint : MonoBehaviour
{
    public GameObject pawPrefab;
    public GameObject newPaw;
    private PlayerMovement script;
    
    public float negatePawHeight; //0.24 for crouch
    public float timeBetweenSteps;
    private float timeBetweenSteps2;
    private float mack;
    public float[] pawLocationArray = new float[2];
    public float[] horizontalPawLocationArray = new float[4];
    public int pawIndex;
    public int pawIndex3;
    public List<GameObject> spawnedPaws = new List<GameObject>();//list of paw prints
    public bool horizontalMovement;

    // Start is called before the first frame update
    void Start()
    {
        script = GetComponent<PlayerMovement>();  
    }

    // Update is called once per frame
    void Update()
    {
        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");
        if(((vInput != 0 && hInput == 0) && (script.grounded || script.isCrouching)))
        {
            steps();
        }
        else if(((vInput == 0 && hInput != 0) && (script.grounded || script.isCrouching)))
        {
            sideSteps();
            Debug.Log("side stepping");
        }
        else if(((vInput != 0 && hInput != 0) && (script.grounded || script.isCrouching)))
        {
            sideSteps();
            Debug.Log("diagonal stepping");
        }
        fade();
        if(script.isCrouching)
        {
            negatePawHeight = 0.24f;
        }
        else
        {
            negatePawHeight = 0.49f;
        }
    }
    public void steps()
    {
        timeBetweenSteps2 += Time.deltaTime;
        if(timeBetweenSteps2 >= timeBetweenSteps)
        {
            if(pawIndex < 1){pawIndex++;}
            else{pawIndex = 0;}
            Vector3 offset = new Vector3(pawLocationArray[pawIndex], -negatePawHeight, 0.5f);
            Vector3 spawnPosition = transform.position + transform.rotation * offset;
            GameObject newPaw = Instantiate(pawPrefab, spawnPosition, transform.rotation * Quaternion.Euler(90, 0, 0));
            SpriteRenderer spriteRenderer = newPaw.GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
            spawnedPaws.Add(newPaw);
            timeBetweenSteps2 = 0;
        }
    }
    public void sideSteps()
    {
        mack += Time.deltaTime;
        if(mack >= timeBetweenSteps)
        {
            if(pawIndex3 < 1){pawIndex3++;}
            else{pawIndex3 = 0;}
            Vector3 offset = new Vector3(pawLocationArray[pawIndex], -negatePawHeight, horizontalPawLocationArray[pawIndex3]);
            Vector3 spawnPosition = transform.position + transform.rotation * offset;
            GameObject newPaw = Instantiate(pawPrefab, spawnPosition, transform.rotation * Quaternion.Euler(90, 0, 0));
            SpriteRenderer spriteRenderer = newPaw.GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
            spawnedPaws.Add(newPaw);
            mack = 0;
        }
    }
    public void fade()
    {
        for (int i = spawnedPaws.Count - 1; i >= 0; i--)
    {
        GameObject paw = spawnedPaws[i];

        if (paw.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            Color color = spriteRenderer.color;
            color.a -= Time.deltaTime / 1.1f; // Reduce opacity over time
            spriteRenderer.color = color;

            if (color.a <= 0)
            {
                Destroy(paw);
                spawnedPaws.RemoveAt(i); // Remove from the list
            }
        }
    }
    }
}
