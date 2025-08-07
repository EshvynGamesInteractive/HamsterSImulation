using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BallLauncher : Interactable
{
    [Header("Timer Settings")]
    [SerializeField] private float timer;
    [SerializeField] private float fetchDuration = 10f;
    [SerializeField] Text txtTimer;
    private bool ballLaunched = false;


    [Header("Ball Launch Settings")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private float upwardForce = 4f;
    [SerializeField] private GameObject launcherCamera;
    //[SerializeField] GameObject[] balls;

    [Header("Launch Target Area")]
    [SerializeField] private Vector3 areaCenter;
    [SerializeField] private Vector3 areaSize;

    [Header("Return Zone")]
    [SerializeField] private Transform returnZone;

    private GameObject currentBall;
    private bool waitingForReturn;

    private void Start()
    {
        txtTimer.transform.parent.gameObject.SetActive(false);
    }
    private void Update()
    { 
        if (!ballLaunched) return;

        timer -= Time.deltaTime;
        txtTimer.text = timer.ToString("f1");
        if (timer <= 0f)
        {
            TaskFailed();
        }
    }

    private void TaskFailed()
    {
        ballLaunched = false;
        Debug.Log("failed task");
    }
    public void BeginLaunching()    // only run on start of mini game
    {
        MainScript.instance.pnlInfo.ShowInfo("Retrieve ball in given time to earn points");
        launcherCamera.SetActive(true);

        DOVirtual.DelayedCall(2, () =>
        {
            launcherCamera.SetActive(false);
        });

        Invoke(nameof(LaunchNewBall), 1);
    }

    public void StopLaunching()
    {
        //if (currentBall != null)
        //    Destroy(currentBall);
        txtTimer.transform.parent.gameObject.SetActive(false);
        HideBall();
        waitingForReturn = false;
    }

    private void LaunchNewBall()
    {
        EnableForInteraction(false);
        timer = fetchDuration;
        ballLaunched = true;
        txtTimer.transform.parent.gameObject.SetActive(true);
        Vector3 randomTarget = areaCenter + new Vector3(
            Random.Range(-areaSize.x / 2, areaSize.x / 2),
            0,
            Random.Range(-areaSize.z / 2, areaSize.z / 2)
        );

        currentBall = Instantiate(ballPrefab, launchPoint.position, Quaternion.identity);

        if (currentBall.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            Vector3 launchDir = (randomTarget - launchPoint.position).normalized;
            Vector3 force = launchDir * launchForce + Vector3.up * upwardForce;
            rb.AddForce(force, ForceMode.Impulse);
        }
        SoundManager.instance.PlaySound(SoundManager.instance.ballLaunch);
        //if (currentBall.TryGetComponent<FetchBall>(out FetchBall fetchBall))
        //{
        //    fetchBall.Setup(this, returnZone);
        //}

        waitingForReturn = true;
    }

    public void OnBallReturned()
    {
        if (!waitingForReturn) return;
        DisableForInteraction(true);
        ballLaunched = false;
        txtTimer.transform.parent.gameObject.SetActive(false);

        waitingForReturn = false;

        if (currentBall != null)
        {
            Destroy(currentBall);
            currentBall = null;
        }

        if (timer <= 0)
            MainScript.instance.pnlInfo.ShowInfo("You are late");
        else
            MainScript.instance.PointScored(1);


        Invoke(nameof(LaunchNewBall), 1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(areaCenter, areaSize);

        if (returnZone != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(returnZone.position, 0.5f);
        }
    }

    public override void Interact(PlayerScript player)
    {
        if (player.HasPickedObject() && player.pickedObject.TryGetComponent<FetchBall>(out FetchBall ball))
        {
            player.ThrowObject();
            Destroy(ball.gameObject);
            OnBallReturned();
            DisableForInteraction(true);
        }
    }

    private void HideBall()
    {
        PlayerScript playerr = MainScript.instance.player;
        if (playerr.HasPickedObject() && playerr.pickedObject.TryGetComponent<FetchBall>(out FetchBall ball))
        {
            playerr.ThrowObject();
            Destroy(ball.gameObject);
        }
    }
}
