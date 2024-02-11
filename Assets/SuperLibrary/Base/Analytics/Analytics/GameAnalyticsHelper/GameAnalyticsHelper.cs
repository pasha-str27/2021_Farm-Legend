using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_GA
using GameAnalyticsSDK;
#endif

public class GameAnalyticsHelper : MonoBehaviour
{
    [SerializeField] GameObject GA_GameObject = null;

    private void Awake()
    {
        bool active = false;
#if USE_GA
        active = true;
#else
        active = false;
#endif

        GA_GameObject.SetActive(active);
    }
    void Start()
    {
#if USE_GA
        GameAnalytics.Initialize();
#endif
    }
}
