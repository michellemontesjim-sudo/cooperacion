using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoopLevelInitializer : MonoBehaviour
{
    [Header("Spawns 3D (0:Tiempo, 1:Piromano, 2:Criogenico, 3:Transformador)")]
    [SerializeField] private Transform[] spawnPoints;
    public static CoopLevelInitializer Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (PlayerManager.Instance == null) return;

        List<PlayerInput> players = PlayerManager.Instance.ObtenerJugadores();

        for (int i = 0; i < players.Count; i++)
        {
            PlayerInput player = players[i];
            int index = player.playerIndex + 1;

            // 1. Mover al SpawnPoint 3D del mapa
            if (spawnPoints != null && index < spawnPoints.Length)
            {
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;

                player.transform.position = spawnPoints[index].position;
                player.transform.rotation = spawnPoints[index].rotation;

                if (cc != null) cc.enabled = true;
            }

            // 2. Asignar el cuadrante de UI
            PlayerAlquimia alquimia = player.GetComponent<PlayerAlquimia>();
            if (alquimia != null)
            {
                alquimia.ConfigurarCuadranteUI(index);
            }
        }


    }
}