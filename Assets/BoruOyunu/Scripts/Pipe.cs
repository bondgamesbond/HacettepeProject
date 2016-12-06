using UnityEngine;
using System.Collections;

public class Pipe : MonoBehaviour
{
    public AudioSource putPipeSound;
    public AudioSource takePipeSound, trashSound;

    public Vector3 pipePos;
    public Vector3 actualPos;

    public int type;
    public float waterSpeed;

    public bool isOnTrueRotation;
    public bool rotating;
    public bool holding;
    public bool canRotate;

    // For checking appropriate properties of a pipe.
    public bool Filled { get; set; }
    public bool Placed;
    public bool Leak;

    // Vector 3 value for the dragging operation.
    private Vector3 offset;

    // Transforms for putting pipes to correct positions.
    private Transform collidingGrid;
    private GameObject trash;

    // Transform of the water.
    private Transform water;

    private GameObject redGlow;
    private GameObject greenGlow;

    void Start()
    {
        Filled = false;
        Leak = false;

        offset = Vector3.zero;

        collidingGrid = null;
        trash = null;

        water = transform.GetChild(1);
        redGlow = transform.GetChild(2).gameObject;
        greenGlow = transform.GetChild(3).gameObject;

        putPipeSound = GameObject.Find("Sounds").transform.GetChild(0).GetComponent<AudioSource>();
        takePipeSound = GameObject.Find("Sounds").transform.GetChild(1).GetComponent<AudioSource>();
        trashSound = GameObject.Find("Sounds").transform.GetChild(2).GetComponent<AudioSource>();

        transform.GetChild(4).GetChild(0).GetComponent<Renderer>().sortingLayerName = "Default";
        transform.GetChild(4).GetChild(0).GetComponent<Renderer>().sortingOrder = 5;

        transform.GetChild(4).GetChild(1).GetComponent<Renderer>().sortingLayerName = "Default";
        transform.GetChild(4).GetChild(1).GetComponent<Renderer>().sortingOrder = 5;
    }

    /*void Update()
    {
        if (Leak)
        {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch t = Input.GetTouch(i);
                    Vector3 mouse = Camera.main.ScreenToWorldPoint(t.position);
                    RaycastHit2D hit = Physics2D.Raycast(mouse, Vector2.zero);

                    if (hit.collider.GetComponent<Pipe>().Leak)
                    {
                        holding = true;
                        Debug.Log("trilelele");
                    }
                }
            }
            else
            {
                holding = false;
            }
        }
    }*/

    // Hold and drag the pipes.
    void OnMouseDown()
    {
        if (GameManager.Instance.State != GameStateBoru.Paused && GameManager.Instance.State != GameStateBoru.Leaking)
        {
            if ((!Placed || Leak))
            {
                offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                redGlow.SetActive(true);

                if (Leak)
                {
                    takePipeSound.Play();
                    GameManager.Instance.leakingSound.Stop();
                }
            }
        }

        if (Leak)
        {
            holding = true;
        }
    }

    void OnMouseDrag()
    {
        if (GameManager.Instance.State != GameStateBoru.Paused && GameManager.Instance.State != GameStateBoru.Leaking)
        {
            if ((!Placed || Leak))
            {
                Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
                Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

                transform.position = curPosition;

                

                if (collidingGrid != null)
                {
                    // Get the grid position and corresponding grid.
                    int gridX = (int)(collidingGrid.position[0]) / 2;
                    int gridY = (int)(collidingGrid.position[1]) / -2;
                    Grid grid = collidingGrid.GetComponent<Grid>();

                    if (!Leak && GameManager.Instance.State == GameStateBoru.Leaking)
                    {
                        if (GameManager.Instance.CheckGrid(gridY, gridX) == type)
                        {
                            redGlow.SetActive(false);
                            greenGlow.SetActive(true);
                        }
                        else if (type == 0 && GameManager.Instance.CheckGrid(gridY, gridX) == 6 && GameManager.Instance.CurrentPlace(gridY, gridX))
                        {
                            redGlow.SetActive(false);
                            greenGlow.SetActive(true);
                        }
                        else if (GetComponentInParent<PipeRotater>() != null && GameManager.Instance.CurrentPlace(gridY, gridX))
                        {
                            canRotate = false;
                            redGlow.SetActive(false);
                            greenGlow.SetActive(true);
                        }
                    }
                    else if (GetComponentInParent<PipeRotater>() != null && (GameManager.Instance.CurrentPlace(gridY, gridX) || GameManager.Instance.State == GameStateBoru.FillWater))
                    {
                        if ((GameManager.Instance.CheckGrid(gridY, gridX) == 2 ||
                            GameManager.Instance.CheckGrid(gridY, gridX) == 3 ||
                            GameManager.Instance.CheckGrid(gridY, gridX) == 4 ||
                            GameManager.Instance.CheckGrid(gridY, gridX) == 5))
                        {
							if (!grid.Placed)
							{
								redGlow.SetActive(false);
								greenGlow.SetActive(true);
							}
                        }
                    }
                    else if (type == 0 && GameManager.Instance.CheckGrid(gridY, gridX) == 6 && (GameManager.Instance.CurrentPlace(gridY, gridX) || GameManager.Instance.State == GameStateBoru.FillWater))
                    {
                        redGlow.SetActive(false);
                        greenGlow.SetActive(true);
                    }
                    else if (GameManager.Instance.CheckGrid(gridY, gridX) == type && (GameManager.Instance.CurrentPlace(gridY, gridX) || GameManager.Instance.State == GameStateBoru.FillWater))
                    {
                        redGlow.SetActive(false);
                        greenGlow.SetActive(true);
                    }
                    else
                    {
                        redGlow.SetActive(true);
                        greenGlow.SetActive(false);
                    }
                }
                else if (trash != null)
                {
                    redGlow.SetActive(false);
                    greenGlow.SetActive(true);
                }
                else
                {
                    redGlow.SetActive(true);
                    greenGlow.SetActive(false);
                }
            }
        }

        if (Leak)
        {
            holding = true;
        }
    }

    void OnMouseUp()
    {
        if (GameManager.Instance.State != GameStateBoru.Paused && GameManager.Instance.State != GameStateBoru.Leaking)
        {
            if ((!Placed || Leak))
            {
                if (collidingGrid != null) // Colliding with a grid.
                {
                    // Get the grid position and corresponding grid.
                    int gridX = (int)(collidingGrid.position[0]) / 2;
                    int gridY = (int)(collidingGrid.position[1]) / -2;
                    Grid grid = collidingGrid.GetComponent<Grid>();

                    pipePos = new Vector3(gridX, gridY, 0);
                    actualPos = collidingGrid.transform.position;
                    if (!Leak && GameManager.Instance.State == GameStateBoru.Leaking)
                    {
                        if (GameManager.Instance.CheckGrid(gridY, gridX) == type)
                        {
                            // Put to the grid.
                            transform.position = collidingGrid.transform.position;
                            GameManager.Instance.PlaceToGrid(gridY, gridX);
                            GameManager.Instance.Place++;

                            // Get grid place and add the pipe to that place.
                            int place = GameManager.Instance.CheckPlace(gridY, gridX);
                            GameManager.Instance.AddPipe(place, this);
                            grid.Placed = true;
                            this.Placed = true;
                            putPipeSound.Play();

                            greenGlow.SetActive(false);
                        }
                        else if (type == 0 && GameManager.Instance.CheckGrid(gridY, gridX) == 6)
                        {
                            type = 6;
                            transform.position = collidingGrid.transform.position;
                            transform.rotation = Quaternion.Euler(0, 0, 180);
                            GameManager.Instance.PlaceToGrid(gridY, gridX);
                            GameManager.Instance.Place++;

                            // Get grid place and add the pipe to that place.
                            int place = GameManager.Instance.CheckPlace(gridY, gridX);
                            GameManager.Instance.AddPipe(place, this);
                            grid.Placed = true;
                            this.Placed = true;
                            putPipeSound.Play();

                            greenGlow.SetActive(false);
                        }
                        else if (GetComponentInParent<PipeRotater>() != null)
                        {
                            if ((GameManager.Instance.CheckGrid(gridY, gridX) == 2 ||
                                GameManager.Instance.CheckGrid(gridY, gridX) == 3 ||
                                GameManager.Instance.CheckGrid(gridY, gridX) == 4 ||
                                GameManager.Instance.CheckGrid(gridY, gridX) == 5))
                            {
								if (!grid.Placed)
								{
									transform.position = collidingGrid.transform.position;

									if (GameManager.Instance.CheckGrid(gridY, gridX) == 2)
									{
										GameManager.Instance.PlaceToGrid(gridY, gridX);
										GameManager.Instance.Place++;
										isOnTrueRotation = true;
										canRotate = false;
									}
									else
									{
										canRotate = true;
									}

									// Get grid place and add the pipe to that place.
									int place = GameManager.Instance.CheckPlace(gridY, gridX);
									GameManager.Instance.AddPipe(place, this);
									grid.Placed = true;
									this.Placed = true;
									putPipeSound.Play();


									greenGlow.SetActive(false);
								}
                            }
                        }
                    }
                    else if (GetComponentInParent<PipeRotater>() != null && (GameManager.Instance.CurrentPlace(gridY, gridX) || GameManager.Instance.State == GameStateBoru.FillWater))
                    {
                        if ((GameManager.Instance.CheckGrid(gridY, gridX) == 2 ||
                            GameManager.Instance.CheckGrid(gridY, gridX) == 3 ||
                            GameManager.Instance.CheckGrid(gridY, gridX) == 4 ||
                            GameManager.Instance.CheckGrid(gridY, gridX) == 5))
                        {
							if (!grid.Placed)
							{
								transform.position = collidingGrid.transform.position;

								if (GameManager.Instance.CheckGrid(gridY, gridX) == 2)
								{
									GameManager.Instance.PlaceToGrid(gridY, gridX);
									GameManager.Instance.Place++;
									isOnTrueRotation = true;
									canRotate = false;
								}
								else
								{
									canRotate = true;
								}

								// Get grid place and add the pipe to that place.
								int place = GameManager.Instance.CheckPlace(gridY, gridX);
								GameManager.Instance.AddPipe(place, this);
								grid.Placed = true;
								this.Placed = true;
								putPipeSound.Play();

								greenGlow.SetActive(false);
							}
                        }
                        else
                        {
                            transform.position = GameManager.Instance.getPipePosition() + new Vector3(2.5f, 0.0f, 0.0f) * (type);
                            redGlow.SetActive(false);
                            greenGlow.SetActive(false);
                        }
                    }
                    else if (GameManager.Instance.CheckGrid(gridY, gridX) == 6 && type == 0 && (GameManager.Instance.CurrentPlace(gridY, gridX) || GameManager.Instance.State == GameStateBoru.FillWater))
                    {
                        type = 6;
                        transform.position = collidingGrid.transform.position;
                        transform.rotation = Quaternion.Euler(0, 0, 180);
                        GameManager.Instance.PlaceToGrid(gridY, gridX);
                        GameManager.Instance.Place++;

                        // Get grid place and add the pipe to that place.
                        int place = GameManager.Instance.CheckPlace(gridY, gridX);
                        GameManager.Instance.AddPipe(place, this);
                        grid.Placed = true;
                        this.Placed = true;
                        putPipeSound.Play();

                        greenGlow.SetActive(false);
                    }

                    // The pipe is at correct grid
                    else if (GameManager.Instance.CheckGrid(gridY, gridX) == type && (GameManager.Instance.CurrentPlace(gridY, gridX) || GameManager.Instance.State == GameStateBoru.FillWater))
                    {
                        // Put to the grid.
                        transform.position = collidingGrid.transform.position;
                        GameManager.Instance.PlaceToGrid(gridY, gridX);
                        GameManager.Instance.Place++;

                        // Get grid place and add the pipe to that place.
                        int place = GameManager.Instance.CheckPlace(gridY, gridX);
                        GameManager.Instance.AddPipe(place, this);
                        grid.Placed = true;
                        this.Placed = true;
                        putPipeSound.Play();

                        greenGlow.SetActive(false);
                    }
                    else // Not at correct grid.
                    {
                        if (Leak)
                        {
                            transform.position = actualPos;
                        }
                        else if (type == 7 || type == 8)
                        {
                            transform.position = GameManager.Instance.getPipePosition() + new Vector3(2.5f, 0.0f, 0.0f) * (type-1);
                            redGlow.SetActive(false);
                            greenGlow.SetActive(false);
                        }
                        else
                        {
                            transform.position = GameManager.Instance.getPipePosition() + new Vector3(2.5f, 0.0f, 0.0f) * (type);
                            redGlow.SetActive(false);
                            greenGlow.SetActive(false);
                        }
                    }
                }
                else // Not colliding.
                {
                    if (trash != null)
                    {
                        gameObject.SetActive(false);
                        trash.transform.GetChild(0).gameObject.SetActive(true);
                        trash.transform.GetChild(1).gameObject.SetActive(false);
                        trash = null;
                        trashSound.Play();
                        transform.GetChild(4).gameObject.SetActive(false);
                    }
                    else if (Leak)
                    {
                        transform.position = actualPos;
                    }
                    else if (type == 7 || type == 8)
                    {
                        transform.position = GameManager.Instance.getPipePosition() + new Vector3(2.5f, 0.0f, 0.0f) * (type - 1);
                        redGlow.SetActive(false);
                        greenGlow.SetActive(false);
                    }
                    else
                    {
                        transform.position = GameManager.Instance.getPipePosition() + new Vector3(2.5f, 0.0f, 0.0f) * (type);
                        redGlow.SetActive(false);
                    }
                }
            }
        }

        if (Leak)
        {
            holding = false;
        }

        redGlow.SetActive(false);
        greenGlow.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Grid"))
        {
            if (Leak)
            {
                return;
            }
            collidingGrid = other.transform;
        }
        else if (other.CompareTag("Trash"))
        {
            if (Leak)
            {
                trash = other.gameObject;
                trash.transform.GetChild(0).gameObject.SetActive(false);
                trash.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Grid"))
        {
            if (collidingGrid == other.transform)
            {
                // Get grid position.
                int gridX = (int)(collidingGrid.position[0]) / 2;
                int gridY = (int)(collidingGrid.position[1]) / -2;
              
                collidingGrid = null;

                // Remove from the path.
                if (Placed && !rotating)
                {
                    GameManager.Instance.MoveFromGrid(gridY, gridX, type);
                    Placed = false;
                }
            }
        }
        else if (other.CompareTag("Trash"))
        {
            if (Leak)
            {
                trash = other.gameObject;
                trash.transform.GetChild(0).gameObject.SetActive(true);
                trash.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    // Fill the water to the pipe.
    public bool FillWater()
    {
        if (type == 1 || type == 0 || type == 6)
        {
            if (water.localScale.y < 4)
            {
                water.Translate(Vector3.up * Time.deltaTime * waterSpeed / 4);
                water.localScale += Vector3.up * Time.deltaTime * waterSpeed;
                return false;
            }
            return true;
        }
        else if (type == 7)
        {
            Transform water0, water1, water2;
            water0 = water.GetChild(0);
            water1 = water.GetChild(1);
            water2 = water.GetChild(2);

            if (water0.localScale.y < 1.25)
            {
                water0.Translate(Vector3.up * Time.deltaTime * waterSpeed / 4);
                water0.localScale += Vector3.up * Time.deltaTime * waterSpeed;
                water1.Translate(Vector3.up * Time.deltaTime * waterSpeed / 4);
                water1.localScale += Vector3.up * Time.deltaTime * waterSpeed;
                return false;
            }
            else if (water2.localScale.y < 2.5)
            {
                water2.Translate(Vector3.up * Time.deltaTime * waterSpeed / 4);
                water2.localScale += Vector3.up * Time.deltaTime * waterSpeed;
                return false;
            }

            return true;
        }
        else if (type == 8)
        {
            Transform water0, water1, water2;
            water0 = water.GetChild(0);
            water1 = water.GetChild(1);
            water2 = water.GetChild(2);

            
            if (water2.localScale.y < 2.5)
            {
                water2.Translate(Vector3.up * Time.deltaTime * waterSpeed / 4);
                water2.localScale += Vector3.up * Time.deltaTime * waterSpeed;
                return false;
            }
            else if (water0.localScale.y < 1.4)
            {
                water0.Translate(Vector3.up * Time.deltaTime * waterSpeed / 4);
                water0.localScale += Vector3.up * Time.deltaTime * waterSpeed;
                water1.Translate(Vector3.up * Time.deltaTime * waterSpeed / 4);
                water1.localScale += Vector3.up * Time.deltaTime * waterSpeed;
                return false;
            }

            return true;
        }
        else
        {
            Transform water0, water1, waterEdge;
            water0 = water.GetChild(0);
            water1 = water.GetChild(1);
            waterEdge = water.GetChild(2);
            
            if (water0.localScale.y < 1.5)
            {
                water0.Translate(Vector3.up * Time.deltaTime * waterSpeed / 4);
                water0.localScale += Vector3.up * Time.deltaTime * waterSpeed;
                return false;
            }
            else if ((type == 2 || type == 5) && waterEdge.localRotation.z > 0)
            {
                waterEdge.gameObject.SetActive(true);
                waterEdge.Rotate(Vector3.forward * Time.deltaTime * waterSpeed * -100);
                return false;
            }
            else if ((type == 3 || type == 4) && waterEdge.localRotation.z < 0)
            {
                waterEdge.gameObject.SetActive(true);
                waterEdge.Rotate(Vector3.forward * Time.deltaTime * waterSpeed * 100);
                return false;
            }
            else if (water1.localScale.y < 1.5)
            {
                water1.Translate(Vector3.up * Time.deltaTime * waterSpeed / 4);
                water1.localScale += Vector3.up * Time.deltaTime * waterSpeed;
                return false;
            }
            

            return true;
        }
    }
}
