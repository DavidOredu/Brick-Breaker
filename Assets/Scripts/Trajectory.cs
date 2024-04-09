using UnityEngine;

public class Trajectory : MonoBehaviour
{
    [SerializeField] int dotsNumber;
    [SerializeField] GameObject dotsParent;
    [SerializeField] GameObject dotsPrefab;
    [SerializeField] float dotsSpacing;
    [SerializeField] [Range(0.01f, 0.3f)] float dotMinScale;
    [SerializeField] [Range(0.3f, 1f)] float dotMaxScale;

    [SerializeField] float ballCheckSize = 0.5f;
    Vector2 pos;
    Vector2 lastPos;
    Vector2 force;
    float timeStamp;
    Transform[] dotsList;
    SpriteRenderer[] dotsRenderers;

    // Start is called before the first frame update
    void Start()
    {
        HideTrejectory();
        PrepareDots();
        ShowTrejectory();
    }
    public void ShowTrejectory()
    {
        dotsParent.SetActive(true);
    }
    void PrepareDots()
    {
        dotsList = new Transform[dotsNumber];
        dotsRenderers = new SpriteRenderer[dotsNumber];
        dotsPrefab.transform.localScale = Vector3.one * dotMaxScale;

        float scale = dotMaxScale;
        float scaleFactor = scale / dotsNumber;
        for (int i = 0; i < dotsNumber; i++)
        {
            dotsList[i] = Instantiate(dotsPrefab, null).transform;
            dotsList[i].parent = dotsParent.transform;
            dotsRenderers[i] = dotsList[i].GetComponent<SpriteRenderer>();

            dotsList[i].localScale = Vector3.one * scale;
            if (scale > dotMinScale)
                scale -= scaleFactor;
        }
    }
    public void UpdateDots(Vector3 pos, Vector2 force)
    {
        timeStamp = dotsSpacing;
        this.pos = pos;
        lastPos = pos;
        this.force = force;

        for (int i = 0; i < dotsNumber; i++)
        {
            // check if the current iteration is hitting an obstacle
            var lastForce = this.force;
            var hit = Physics2D.CircleCast(this.pos, ballCheckSize, this.force.normalized, .5f, LayerMask.GetMask("Wall", "Block"));
          //  var hit = Physics2D.Raycast(this.pos, this.force.normalized, 2f, LayerMask.GetMask("Wall", "Block"));

            // if it is...
            if (hit)
            {
                // reflect the direction of predict with the hit object's surface
                var newDirection = Vector2.Reflect(lastForce.normalized, hit.normal);
                Debug.Log("Magnitude of velocity, direction of travel:" + this.force.magnitude + ", " + newDirection);
                this.force = newDirection * lastForce.magnitude;
                Debug.DrawRay(this.pos, newDirection * 2f, Color.green);
                lastPos = this.pos;
                timeStamp = 0;
            }
            // set the new position
            this.pos.x = (lastPos.x + this.force.x * timeStamp);
            this.pos.y = (lastPos.y + this.force.y * timeStamp) - (0 * timeStamp * timeStamp) / 2f;

            dotsList[i].position = this.pos;
            timeStamp += dotsSpacing;
        }
    }
    public void HideTrejectory()
    {
        dotsParent.SetActive(false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(pos, ballCheckSize);
    }
}
