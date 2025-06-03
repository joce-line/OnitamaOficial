using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;

public class ScriptCriarSalas : MonoBehaviourPunCallbacks
{
	public TMP_InputField input_Create;
	public TMP_InputField input_Join;

	public void CreateRoom()
	{
		PhotonNetwork.CreateRoom(input_Create.text, new RoomOptions() { MaxPlayers = 2, IsVisible = true, isOpen = true } typedLobby.Default, null);
	}

	public void JoinRoom()
	{
		PhotonNetwork.JoinRoom(input_Join.text);
	}

	public override void OnJoinedRoom()
	{
		PhotonNetwork.LoadLevel("Game");
	}
	//public void CriarSala()
	//{
	//	if (!string.IsNullOrEmpty(nomeSalaInput.text))
	//	{
	//		RoomOptions opcoes = new RoomOptions();
	//		opcoes.MaxPlayers = 2; // Exemplo: 2 jogadores por sala
	//		PhotonNetwork.CreateRoom(nomeSalaInput.text, opcoes, null);
	//	}
	//	else
	//	{
	//		feedbackText.text = "Digite um nome para a sala!";
	//	}
	//}

	//public override void OnCreatedRoom()
	//{
	//	feedbackText.text = "Sala criada com sucesso!";
	//	// Aqui você pode carregar a cena do jogo, se desejar
	//}

	//public override void OnCreateRoomFailed(short returnCode, string message)
	//{
	//	feedbackText.text = "Falha ao criar sala: " + message;
	//}
}
