using UnityEngine;

public abstract class LevelScript : MonoBehaviour
{
    [SerializeField]protected Interactable[] items;
    [SerializeField]protected string[] tasks;

    protected GrandpaAI grandpa;
   

    protected void Start()
    {
        grandpa = MainScript.instance.grandPa;
        MainScript.instance.taskPanel.UpdateTask(tasks[0]);
    }

    public abstract void TaskCompleted(int taskNumber);
    public abstract void MiniGameEnded();

    //public void TaskCompleted(int taskNumber)
    //{
    //    items[taskNumber].EnableForInteraction(true);
    //    MainScript.instance.taskPanel.UpdateTask(tasks[taskNumber]);

    //    if(taskNumber==2)   //when bury book
    //        MakeGrandpaWatchTV();

    //    if(taskNumber==3)    // when trun tv off
    //        grandpa.StartTheChase();
    //}



}
