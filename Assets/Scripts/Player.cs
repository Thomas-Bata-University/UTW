using System;
using Factions;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using JetBrains.Annotations;
using Managers;
using UnityEngine;

namespace DefaultNamespace
{
    public sealed class Player : NetworkBehaviour
    {
        public static Player Instance { get; private set; }

        [SyncVar] public string username;

        [SyncVar] public Guid FactionId;

        [SyncVar] public bool isReady;

        [SyncVar] public Pawn controlledPawn;

        [SyncVar] [CanBeNull] private Faction _faction;


        public override void OnStartServer()
        {
            base.OnStartServer();

            GameManager.Instance.players.Add(this);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            GameManager.Instance.players.Remove(this);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner) return;

            Instance = this;

            /*UIManager.Instance.Initialize();

            UIManager.Instance.Show<LobbyView>();*/
        }

        private void Update()
        {
            if (!IsOwner) return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                ServerSetIsReady(!isReady);
            }
        }

        public void StartGame()
        {
            GameObject
                pawnPrefab =
                    new GameObject(); //TODO?  Addressables.LoadAssetAsync<GameObject>("Pawn").WaitForCompletion();

            GameObject pawnInstance = Instantiate(pawnPrefab);

            Spawn(pawnInstance, Owner);

            controlledPawn = pawnInstance.GetComponent<Pawn>();

            controlledPawn.controllingPlayer = this;

            TargetPawnSpawned(Owner);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ServerSpawnPawn()
        {
            StartGame();
            RetrieveAssetsFromServer();
        }

        public void StopGame()
        {
            if (controlledPawn != null && controlledPawn.IsSpawned) controlledPawn.Despawn();
        }

        [ServerRpc(RequireOwnership = false)]
        public void ServerSetIsReady(bool value)
        {
            isReady = value;
        }

        [TargetRpc]
        private void TargetPawnSpawned(NetworkConnection networkConnection)
        {
            //TODO ? UIManager.Instance.Show<MainView>();
        }

        [TargetRpc]
        public void TargetPawnKilled(NetworkConnection networkConnection)
        {
            //TODO ?   UIManager.Instance.Show<RespawnView>();
        }


        private void RetrieveAssetsFromServer()
        {
            _faction = FactionsManager.Instance.GetFactionById(FactionId);
        }
    }
}