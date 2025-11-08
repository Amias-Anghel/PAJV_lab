using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStateController : NetworkBehaviour
{
    enum GameState
    {
        Waiting,
        Starting,
        Running,
        Ending
    }

    [SerializeField] private TMP_Text endScreenText;
    [SerializeField] private Button startGameButton;

    [Networked] private GameState _gameState { get; set; }

    [Networked] private NetworkBehaviourId _winner { get; set; }
    [Networked] private TickTimer _timer { get; set; }

    private List<NetworkBehaviourId> _playerDataNetworkedIds = new List<NetworkBehaviourId>();

    public override void Spawned()
    {
        endScreenText.text = string.Empty;
        startGameButton.gameObject.SetActive(false);

        if (_gameState != GameState.Starting)
        {
            foreach (var player in Runner.ActivePlayers)
            {
                if (Runner.TryGetPlayerObject(player, out var playerObject) == false) continue;
                TrackNewPlayer(playerObject.GetComponent<PlayerDataNetworked>().Id);
            }
        }

        Runner.SetIsSimulated(Object, true);

        if (!Object.HasStateAuthority) return;
        _gameState = GameState.Waiting;
        startGameButton.gameObject.SetActive(true);
        startGameButton.interactable = false;
    }

    public override void FixedUpdateNetwork()
    {
        switch (_gameState)
        {
            case GameState.Waiting:
                UpdateWaitingDisplay();
                break;
            case GameState.Starting:
                UpdateStartingDisplay();
                break;
            case GameState.Running:
                endScreenText.text = string.Empty;
                break;
            case GameState.Ending:
                UpdateEndingDisplay();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateWaitingDisplay()
    {
        int players = Runner.ActivePlayers.Count();
        if (players < 2) {
            endScreenText.text = $"Waiting for players...\n{players} connected";
        } 
        else {
            endScreenText.text = $"Waiting for host to start...\n{players} connected";
            if (!Object.HasStateAuthority) return;
            
            startGameButton.interactable = true;
        }
    }

    public void StartGame() {
        if (!Object.HasStateAuthority) return;
        startGameButton.gameObject.SetActive(false);
        _gameState = GameState.Starting;
    }

    private void UpdateStartingDisplay()
    {
        endScreenText.text = string.Empty;

        if (Object.HasStateAuthority == false) return;

        FindObjectOfType<PlayerSpawner>().StartPlayerSpawner(this);
        FindObjectOfType<EggSpawner>().StartEggSpawner();
        FindObjectOfType<DamageZoneSpawner>().StartDamageZoneSpawner();

        _gameState = GameState.Running;
    }

    private void UpdateEndingDisplay()
    {
        if (Runner.TryFindBehaviour(_winner, out PlayerDataNetworked playerData)) {
            endScreenText.text = $"{playerData.NickName} won with {playerData.Score} points.\nDisconecting...";
        } else {
            endScreenText.text  ="Disconecting...";
        }

        if (_timer.ExpiredOrNotRunning(Runner) == false) return;

        Runner.Shutdown();        
    }

    // Called from the ShipController when it hits an asteroid
    public void CheckIfGameHasEnded()
    {
        if (Object.HasStateAuthority == false) return;

        int playersAlive = 0;

        for (int i = 0; i < _playerDataNetworkedIds.Count; i++)
        {
            if (Runner.TryFindBehaviour(_playerDataNetworkedIds[i],
                    out PlayerDataNetworked playerDataNetworkedComponent) == false)
            {
                _playerDataNetworkedIds.RemoveAt(i);
                i--;
                continue;
            }

            if (playerDataNetworkedComponent.Life > 0) playersAlive++;
        }

        // If more than 1 player is left alive, the game continues.
        // If only 1 player is left, the game ends immediately.
        if (playersAlive > 1 || (Runner.ActivePlayers.Count() == 1 && playersAlive == 1)) return;

        foreach (var playerDataNetworkedId in _playerDataNetworkedIds)
        {
            if (Runner.TryFindBehaviour(playerDataNetworkedId,
                    out PlayerDataNetworked playerDataNetworkedComponent) ==
                false) continue;

            if (playerDataNetworkedComponent.Life <= 0) continue;

            _winner = playerDataNetworkedId;
        }

        GameHasEnded();
    }

    private void GameHasEnded()
    {
        _timer = TickTimer.CreateFromSeconds(Runner, 2f);
        _gameState = GameState.Ending;
    }

    public void TrackNewPlayer(NetworkBehaviourId playerDataNetworkedId)
    {
        _playerDataNetworkedIds.Add(playerDataNetworkedId);
    }
}
