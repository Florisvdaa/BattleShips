using MoreMountains.Feedbacks;
using MoreMountains.Feel;
using MoreMountains.Tools;
using MoreMountains.FeedbacksForThirdParty;
using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance { get; private set; }



    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
}
