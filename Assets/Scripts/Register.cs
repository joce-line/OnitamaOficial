// using UnityEngine;
// using UnityEngine.SceneManagement;
// using TMPro;
// using System;
// using Assets.scripts.Classes;
// using System.Security.Cryptography;
// using System.Collections.Generic;

// public class Register : MonoBehaviour
// {
//     public TMP_InputField nicknameInput;
//     public TMP_InputField cpfInput;
//     public TMP_InputField emailInput;
//     public TMP_InputField senhaInput;

//     private string cpf;
//     private string email;
//     //private int idade;
//     private string nickname;
//     private string senha;

//     public void Registrar()
//     {
//         try
//         {
//             if (DatabaseManager.Instance == null || string.IsNullOrEmpty(DatabaseManager.ConnectionString))
//             {
//                 Debug.LogError("DatabaseManager não inicializado.");
//                 return;
//             }

//             nickname = nicknameInput.text;
//             cpf = cpfInput.text;
//             email = emailInput.text;
//             senha = senhaInput.text;

//             if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(cpf) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
//             {
//                 Debug.LogError("Todos os campos são obrigatórios. Algum campo esta nulo.");
//                 //TODO: quando adicionar texto visivel para usuario, adicionar aqui a atualização da variavel de feedback.
//                 return;
//             }

//             HashSenha encriptografar = new HashSenha(SHA512.Create());

//             #region email disponivel
//             // Consulta para verificar se o email esta disponivel
//             string selectEmailQuery = "SELECT COUNT(*) FROM usuarios WHERE email = @email";
//             var emailParams = new Dictionary<string, object> { { "@email", email } };
//             //pegando função direta do DatabaseManager
//             long countEmail = Convert.ToInt64(DatabaseManager.Instance.ExecuteScalar(selectEmailQuery, emailParams));
//             if (countEmail > 0)
//             {
//                 Debug.Log("Email indisponível.");
//                 //TODO: adicionar aqui variavel de feedback para usuario
//                 return;
//             }
//             #endregion

//             #region nickname disponivel
//             // Consulta para verificar se o nickname esta disponivel
//             string selectNicknameQuery = "SELECT COUNT(*) FROM usuarios WHERE nickname = @nickname";
//             var nickParams = new Dictionary<string, object> { { "@nickname", nickname } };
//             long countNickname = Convert.ToInt64(DatabaseManager.Instance.ExecuteScalar(selectNicknameQuery, nickParams));

//             if (countNickname > 0)
//             {
//                 Debug.Log("Nickname indisponível.");
//                 //TODO: adicionar aqui variavel de feedback para usuario
//                 return;
//             }
//             #endregion

//             #region insert tabela
//             // Inserir valores na tabela
//             string insertQuery = "INSERT INTO usuarios (nickname, cpf, email, senha) VALUES (@nickname, @cpf, @email, @senha);";
//             var insertParams = new Dictionary<string, object>
//         {
//             { "@nickname", nickname },
//             { "@cpf", cpf },
//             { "@email", email },
//             { "@senha", encriptografar.CriptografarSenha(senha) }
//         };

//             if (DatabaseManager.Instance.ExecuteNonQuery(insertQuery, insertParams))
//             {
//                 Debug.Log("Usuário cadastrado!");
//                 UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
//             }
//             else
//             {
//                 Debug.LogError("Falha ao cadastrar usuário.");
//                 //TODO: adicionar aqui variavel de feedback para usuario
//             }

//             #endregion

//         }
//         catch (Exception e)
//         {

//             Debug.LogError($"Erro ao realizar cadastro: {e.Message}");
//         }
//     }
// }
//CODIGO ANTIGO

// using UnityEngine;
// using UnityEngine.SceneManagement;
// using TMPro;
// using System;
// using Assets.scripts.Classes;
// using System.Security.Cryptography;
// using System.Collections.Generic;

// public class Register : MonoBehaviour
// {
//     public TMP_InputField nicknameInput;
//     public TMP_InputField cpfInput;
//     public TMP_InputField emailInput;
//     public TMP_InputField senhaInput;

//     private string cpf;
//     private string email;
//     private string nickname;
//     private string senha;

//     public void Registrar()
//     {
//         try
//         {
//             if (DatabaseManager.Instance == null || string.IsNullOrEmpty(DatabaseManager.ConnectionString))
//             {
//                 Debug.LogError("DatabaseManager não inicializado.");
//                 return;
//             }

//             nickname = nicknameInput.text;
//             cpf = cpfInput.text;
//             email = emailInput.text;
//             senha = senhaInput.text;

//             if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(cpf) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
//             {
//                 Debug.LogError("Todos os campos são obrigatórios. Algum campo esta nulo.");
//                 //TODO: quando adicionar texto visível para usuário, adicionar aqui a atualização da variável de feedback.
//                 return;
//             }

//             // Verificação de senha mínima
//             if (senha.Length < 8)
//             {
//                 Debug.LogError("A senha deve ter pelo menos 8 caracteres.");
//                 //TODO: adicionar aqui variável de feedback para usuário
//                 return;
//             }

//             // Verificação de formato de email
//             if (!email.Contains("@"))
//             {
//                 Debug.LogError("Formato de email inválido.");
//                 //TODO: adicionar aqui variável de feedback para usuário
//                 return;
//             }

//             // Verificação se CPF é válido (estrutura correta)
//             if (!ValidarCPF(cpf))
//             {
//                 Debug.LogError("CPF inválido.");
//                 //TODO: adicionar aqui variável de feedback para usuário
//                 return;
//             }

//             // Verificação se CPF já está cadastrado
//             string selectCpfQuery = "SELECT COUNT(*) FROM usuarios WHERE cpf = @cpf";
//             var cpfParams = new Dictionary<string, object> { { "@cpf", cpf } };
//             long countCpf = Convert.ToInt64(DatabaseManager.Instance.ExecuteScalar(selectCpfQuery, cpfParams));

//             if (countCpf > 0)
//             {
//                 Debug.Log("CPF já cadastrado.");
//                 //TODO: adicionar aqui variável de feedback para usuário
//                 return;
//             }

//             HashSenha encriptografar = new HashSenha(SHA512.Create());

//             // Verificação se email já está em uso
//             string selectEmailQuery = "SELECT COUNT(*) FROM usuarios WHERE email = @email";
//             var emailParams = new Dictionary<string, object> { { "@email", email } };
//             long countEmail = Convert.ToInt64(DatabaseManager.Instance.ExecuteScalar(selectEmailQuery, emailParams));
//             if (countEmail > 0)
//             {
//                 Debug.Log("Email indisponível.");
//                 //TODO: adicionar aqui variável de feedback para usuário
//                 return;
//             }

//             // Verificação se nickname está disponível
//             string selectNicknameQuery = "SELECT COUNT(*) FROM usuarios WHERE nickname = @nickname";
//             var nickParams = new Dictionary<string, object> { { "@nickname", nickname } };
//             long countNickname = Convert.ToInt64(DatabaseManager.Instance.ExecuteScalar(selectNicknameQuery, nickParams));
//             if (countNickname > 0)
//             {
//                 Debug.Log("Nickname indisponível.");
//                 //TODO: adicionar aqui variável de feedback para usuário
//                 return;
//             }

//             // Inserção no banco de dados
//             string insertQuery = "INSERT INTO usuarios (nickname, cpf, email, senha) VALUES (@nickname, @cpf, @email, @senha);";
//             var insertParams = new Dictionary<string, object>
//             {
//                 { "@nickname", nickname },
//                 { "@cpf", cpf },
//                 { "@email", email },
//                 { "@senha", encriptografar.CriptografarSenha(senha) }
//             };

//             if (DatabaseManager.Instance.ExecuteNonQuery(insertQuery, insertParams))
//             {
//                 Debug.Log("Usuário cadastrado!");
//                 SceneManager.LoadScene("Login");
//             }
//             else
//             {
//                 Debug.LogError("Falha ao cadastrar usuário.");
//                 //TODO: adicionar aqui variável de feedback para usuário
//             }
//         }
//         catch (Exception e)
//         {
//             Debug.LogError($"Erro ao realizar cadastro: {e.Message}");
//         }
//     }

//     // Função para validar estrutura do CPF
//     private bool ValidarCPF(string cpf)
//     {
//         cpf = cpf.Replace(".", "").Replace("-", "");

//         if (cpf.Length != 11 || !long.TryParse(cpf, out _)) return false;

//         string[] invalidos = {
//             "00000000000", "11111111111", "22222222222", "33333333333",
//             "44444444444", "55555555555", "66666666666", "77777777777",
//             "88888888888", "99999999999"
//         };
//         if (Array.Exists(invalidos, element => element == cpf)) return false;

//         int soma = 0;
//         for (int i = 0; i < 9; i++)
//             soma += (cpf[i] - '0') * (10 - i);

//         int digito1 = soma % 11;
//         digito1 = digito1 < 2 ? 0 : 11 - digito1;

//         soma = 0;
//         for (int i = 0; i < 10; i++)
//             soma += (cpf[i] - '0') * (11 - i);

//         int digito2 = soma % 11;
//         digito2 = digito2 < 2 ? 0 : 11 - digito2;

//         return cpf[9] - '0' == digito1 && cpf[10] - '0' == digito2;
//     }
// }

//CODIGO FINAL ABAIXO
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Text.RegularExpressions;
using Assets.scripts.Classes;
using System.Security.Cryptography;
using System.Collections.Generic;

public class Register : MonoBehaviour
{
    public TMP_InputField nicknameInput;
    public TMP_InputField cpfInput;
    public TMP_InputField emailInput;
    public TMP_InputField senhaInput;

    private string cpf;
    private string email;
    private string nickname;
    private string senha;

    public void Registrar()
    {
        try
        {
            if (DatabaseManager.Instance == null || string.IsNullOrEmpty(DatabaseManager.ConnectionString))
            {
                Debug.LogError("DatabaseManager não inicializado.");
                return;
            }

            nickname = nicknameInput.text;
            cpf = cpfInput.text;
            email = emailInput.text;
            senha = senhaInput.text;

            if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(cpf) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                Debug.LogError("Todos os campos são obrigatórios. Algum campo esta nulo.");
                //TODO: quando adicionar texto visível para usuário, adicionar aqui a atualização da variável de feedback.
                return;
            }

            // Verificação de senha mínima
            if (senha.Length < 8)
            {
                Debug.LogError("A senha deve ter pelo menos 8 caracteres.");
                //TODO: adicionar aqui variável de feedback para usuário
                return;
            }

            // Verificação de complexidade da senha
            if (!Regex.IsMatch(senha, @"[A-Za-z]")) // Tem letra?
            {
                Debug.LogError("A senha deve conter pelo menos uma letra.");
                //TODO: adicionar aqui variável de feedback para usuário
                return;
            }

            if (!Regex.IsMatch(senha, @"\d")) // Tem número?
            {
                Debug.LogError("A senha deve conter pelo menos um número.");
                //TODO: adicionar aqui variável de feedback para usuário
                return;
            }

            if (!Regex.IsMatch(senha, @"[\W_]")) // Tem caractere especial?
            {
                Debug.LogError("A senha deve conter pelo menos um caractere especial.");
                //TODO: adicionar aqui variável de feedback para usuário
                return;
            }

            // Verificação de formato de email
            if (!email.Contains("@"))
            {
                Debug.LogError("Formato de email inválido.");
                //TODO: adicionar aqui variável de feedback para usuário
                return;
            }

            // Verificação se CPF é válido (estrutura correta)
            if (!ValidarCPF(cpf))
            {
                Debug.LogError("CPF inválido.");
                //TODO: adicionar aqui variável de feedback para usuário
                return;
            }

            // Verificação se CPF já está cadastrado
            string selectCpfQuery = "SELECT COUNT(*) FROM usuarios WHERE cpf = @cpf";
            var cpfParams = new Dictionary<string, object> { { "@cpf", cpf } };
            long countCpf = Convert.ToInt64(DatabaseManager.Instance.ExecuteScalar(selectCpfQuery, cpfParams));

            if (countCpf > 0)
            {
                Debug.Log("CPF já cadastrado.");
                //TODO: adicionar aqui variável de feedback para usuário
                return;
            }

            HashSenha encriptografar = new HashSenha(SHA512.Create());

            // Verificação se email já está em uso
            string selectEmailQuery = "SELECT COUNT(*) FROM usuarios WHERE email = @email";
            var emailParams = new Dictionary<string, object> { { "@email", email } };
            long countEmail = Convert.ToInt64(DatabaseManager.Instance.ExecuteScalar(selectEmailQuery, emailParams));
            if (countEmail > 0)
            {
                Debug.Log("Email indisponível.");
                //TODO: adicionar aqui variável de feedback para usuário
                return;
            }

            // Verificação se nickname está disponível
            string selectNicknameQuery = "SELECT COUNT(*) FROM usuarios WHERE nickname = @nickname";
            var nickParams = new Dictionary<string, object> { { "@nickname", nickname } };
            long countNickname = Convert.ToInt64(DatabaseManager.Instance.ExecuteScalar(selectNicknameQuery, nickParams));
            if (countNickname > 0)
            {
                Debug.Log("Nickname indisponível.");
                //TODO: adicionar aqui variável de feedback para usuário
                return;
            }

            // Inserção no banco de dados
            string insertQuery = "INSERT INTO usuarios (nickname, cpf, email, senha) VALUES (@nickname, @cpf, @email, @senha);";
            var insertParams = new Dictionary<string, object>
            {
                { "@nickname", nickname },
                { "@cpf", cpf },
                { "@email", email },
                { "@senha", encriptografar.CriptografarSenha(senha) }
            };

            if (DatabaseManager.Instance.ExecuteNonQuery(insertQuery, insertParams))
            {
                Debug.Log("Usuário cadastrado!");
                SceneManager.LoadScene("Login");
            }
            else
            {
                Debug.LogError("Falha ao cadastrar usuário.");
                //TODO: adicionar aqui variável de feedback para usuário
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Erro ao realizar cadastro: {e.Message}");
        }
    }

    // Função para validar estrutura do CPF
    private bool ValidarCPF(string cpf)
    {
        cpf = cpf.Replace(".", "").Replace("-", "");

        if (cpf.Length != 11 || !long.TryParse(cpf, out _)) return false;

        string[] invalidos = {
            "00000000000", "11111111111", "22222222222", "33333333333",
            "44444444444", "55555555555", "66666666666", "77777777777",
            "88888888888", "99999999999"
        };
        if (Array.Exists(invalidos, element => element == cpf)) return false;

        int soma = 0;
        for (int i = 0; i < 9; i++)
            soma += (cpf[i] - '0') * (10 - i);

        int digito1 = soma % 11;
        digito1 = digito1 < 2 ? 0 : 11 - digito1;

        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += (cpf[i] - '0') * (11 - i);

        int digito2 = soma % 11;
        digito2 = digito2 < 2 ? 0 : 11 - digito2;

        return cpf[9] - '0' == digito1 && cpf[10] - '0' == digito2;
    }
public void OpenLoginScene()
    {
        SceneManager.LoadScene("Login");
    }
}
