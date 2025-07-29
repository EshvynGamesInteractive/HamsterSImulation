using UnityEngine;

public enum CushionType
{
    Soft,
    Firm,
    Springy
}

public class Cushion : Pickable
{
    public CushionType type;

    public float BounceForce
    {
        get
        {
            return type switch
            {
                CushionType.Soft => 12f,
                CushionType.Firm => 5f,
                CushionType.Springy => 8f,
                _ => 0f
            };
        }
    }

    public float Height
    {
        get
        {
            return type switch
            {
                CushionType.Soft => 0.25f,
                CushionType.Firm => 0.15f,
                CushionType.Springy => 0.2f,
                _ => 0.2f
            };
        }
    }

    private void Start()
    {
        GetComponent<Rigidbody>().mass = 1f;
    }

    public void OnBounce()
    {
        Debug.Log($"Bounced on {type} cushion");
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
    }
}
