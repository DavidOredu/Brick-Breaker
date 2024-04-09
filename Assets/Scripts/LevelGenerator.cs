using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public List<GameObject> brickPrefabs;

    public int rows = 5;
    public int cols = 10;

    public Vector2 spawnOffset;
    public Vector2 perlinOffset;

    public float perlinOffsetLimit = 9999f;
    public float perlinScale = 10f;
    [Range(0f, 1f)]
    public float perlinThreshold = .5f;

    public int smoothenCheckSize = 2;
    public bool useSmoothing;
    public bool getRandomLevel;

    private float width;
    private float height;
    private float left;
    private float top;


    public Vector2 brickSpacing;

    private Probability<GameObject> brickProbability;


    [SerializeField]
    private ProbabilityData brickProbabilityData;

    void Start()
    {
        // define necessary array map to create the levels.

        // The perlin map creates smooth differing values for the level bricks for a seeming randomness
        float[,] perlinMap = new float[rows, cols];

        // The smoothed map averages the perlin map, the give more of a symmetrical and organised feel
        float[,] smoothedMap = new float[rows, cols];

        // The symmetry map creates a symmetrical reflection along the y - axis to increase organisation
        float[,] symmetryMap = new float[rows, cols];

        // The binary map thresholds the above map to a binary value to determine where to spawn a brick
        bool[,] binaryMap = new bool[rows, cols];

        // probability for a certain kind of brick to spawn
        brickProbability = new Probability<GameObject>(brickProbabilityData, brickPrefabs);

        // determine width and height
        width = (cols - 1) * brickSpacing.x;
        height = rows * brickSpacing.y;

        // get leftmost and topmost points to begin spawning
        left = transform.position.x + spawnOffset.x - width / 2;
        top = transform.position.y + spawnOffset.y + height / 2;

        // if we need a random level...
        if (getRandomLevel)
        {
            // ...scroll the perlin map a certain amount, determined by a random value
            perlinOffset.x = Random.Range(0, perlinOffsetLimit);
            perlinOffset.y = Random.Range(0, perlinOffsetLimit);
        }

        // generate the perlin map
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                perlinMap[x, y] = Mathf.PerlinNoise(perlinOffset.x + (float)x / rows * perlinScale, perlinOffset.y + (float)y / cols * perlinScale);
                Debug.Log("Perlin Map: " + $"({x}, {y}): {perlinMap[x, y]}");
            }
        }

        // generate the smoothed map, if required
        if (useSmoothing)
        {
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    // the following for loop creates a square that averages every pixel around the current pixel in the perlin map to get an average value of a position relative to the surrounding pixels
                    float sum = 0;
                    for (int i = -smoothenCheckSize; i <= smoothenCheckSize; i++)
                    {
                        for (int j = -smoothenCheckSize; j <= smoothenCheckSize; j++)
                        {
                            //  if ((x == 0 && j < 0) || (y == 0 && i < 0) || (x == rows - 1 && j > 0) || (y == cols - 1 && i > 0)) { continue; }
                            // if the current pixel is within the perlin map bound...
                            if (x + i >= 0 && y + j >= 0 && x + i < rows && y + j < cols)
                            {
                                // get the maximum value of pixel coordinates to determine what order the pixel is at, to obtain the weight of the order of the pixel relative to the main pixel
                                sum += perlinMap[x + i, y + j] * (1-((float)(Mathf.Max(Mathf.Abs(i), Mathf.Abs(j))) / (smoothenCheckSize + 1)));
                            }
                        }
                    }

                    // divide the sum by the iterations to get the average. Since the check size goes from negative to positive e.g -2 to 2, we multiply by the check size by 2 and add 1 to get the full length, and since it's an array, we square it.
                    smoothedMap[x, y] = sum / (((smoothenCheckSize * 2) + 1) * ((smoothenCheckSize * 2) + 1));
                    Debug.Log("Smoothed Map: " + $"({x}, {y}): {smoothedMap[x, y]}");
                }
            }
        }

        // generate the symmetry map
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                if (useSmoothing)
                {
                    // if we have reached the half of the column size...
                    if(y >= cols / 2)
                    // we use the position of the mirrored column position as the current y position
                    symmetryMap[x, y] = smoothedMap[x, cols - 1 - y];
                    else
                    // Use the regular position as the current y position
                    symmetryMap[x, y] = smoothedMap[x, y];
                }
                else
                {
                    if (y >= cols / 2)
                        symmetryMap[x, y] = perlinMap[x, cols - 1 - y];
                    else
                        symmetryMap[x, y] = perlinMap[x, y];
                }
            }
        }

        // generate the binary map
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                binaryMap[x, y] = symmetryMap[x, y] > perlinThreshold;
            }
        }

        // spawn bricks
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                if (binaryMap[x, y])
                {
                    Vector2 position = new Vector2(left + y * brickSpacing.x, top - x * brickSpacing.y);
                    Instantiate(brickProbability.ProbabilityGenerator(), position, Quaternion.identity);
                }
            }
        }
    }
}
