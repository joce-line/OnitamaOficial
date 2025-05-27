using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class ScriptCriarSalas : MonoBehaviourPunCallbacks
{
	public InputField nomeSalaInput;
	public UnityEngine.UI.Text feedbackText;

	public void CriarSala()
	{
		if (!string.IsNullOrEmpty(nomeSalaInput.text))
		{
			RoomOptions opcoes = new RoomOptions();
			opcoes.MaxPlayers = 2; // Exemplo: 2 jogadores por sala
			PhotonNetwork.CreateRoom(nomeSalaInput.text, opcoes, null);
		}
		else
		{
			feedbackText.text = "Digite um nome para a sala!";
		}
	}

	public override void OnCreatedRoom()
	{
		feedbackText.text = "Sala criada com sucesso!";
		// Aqui você pode carregar a cena do jogo, se desejar
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		feedbackText.text = "Falha ao criar sala: " + message;
	}
}
