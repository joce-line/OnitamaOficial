namespace Assets.scripts.InfoPlayer
{
    public static class PlayerInfo
    {
        public static string nomePlayer;
        public static int idPlayer;
        public static int id_Background; // Alterado para int
        public static string caminho_Background;
        public static int SelectedSkinId { get; set; } = -1;
    }

    public static class DadosJogo
    {
        public static int vencedor;
        public static int perdedor;
    }

    public class SkinData
    {
        public int id;
        public string nome;
        public string caminhoPawn;
        public string caminhoKing;
    }
}
