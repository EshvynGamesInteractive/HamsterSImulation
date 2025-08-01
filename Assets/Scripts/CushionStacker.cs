using System.Collections.Generic;
using UnityEngine;

public class CushionStacker : Interactable
{
    public Transform stackPoint; // assign in Inspector
    public GameObject[] cushionPrefabs; // Soft, Firm, Springy
    public float cushionHeight = 0.5f;

    private List<GameObject> stackedCushions = new List<GameObject>();

    public void StackCushion(int index) // index of cushion type
    {
        if (index < 0 || index >= cushionPrefabs.Length) return;

        Vector3 position = stackPoint.position + Vector3.up * cushionHeight * stackedCushions.Count;
        GameObject cushion = Instantiate(cushionPrefabs[index], position, Quaternion.identity);
        stackedCushions.Add(cushion);
    }

    public override void Interact(PlayerScript player)
    {
        if (!player.HasPickedObject()) return;

        if (player.pickedObject.TryGetComponent<Cushion>(out Cushion cushion))
        {
            // Stack it at the trampoline point

            Debug.Log(MiniGameManager.Instance);
            //MiniGameManager.Instance.cushionTrampoline.StackCushion(cushion);
            player.PlaceObject(MiniGameManager.Instance.cushionTrampoline.GetStackPos(cushion));
            //cushion.transform.SetParent(transform);
            cushion.DisableForInteraction(true);
            //Destroy(cushion.gameObject);
        }
        else
        {
            MainScript.instance.pnlInfo.ShowInfo("Bring cushions to stack here");
        }
    }

}
