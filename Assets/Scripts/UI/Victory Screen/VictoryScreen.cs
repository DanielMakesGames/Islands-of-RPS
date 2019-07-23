using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScreen : UIScreen
{
    private void OnEnable()
    {
        EnemySquadManager.OnAllEnemiesDefeated += EnableScreen;

        ReturnToTitleButton.OnPressed += DisableScreen;
    }

    private void OnDisable()
    {
        EnemySquadManager.OnAllEnemiesDefeated -= EnableScreen;

        ReturnToTitleButton.OnPressed -= DisableScreen;
    }
}
