using Unisave;
using Unisave.Facets;
using Unisave.Facades;
using Unisave.Entities;

[EntityCollectionName("players")]
public class PlayerEntity : Entity
{
    public string Username;
    public string Password;
}

public class AuthFacet : Facet
{
    public bool Register(string username, string password)
    {
        // Проверяем, существует ли пользователь с таким именем
        var existingPlayers = DB.TakeAll<PlayerEntity>()
            .Filter(p => p.Username == username)
            .Get();

        if (existingPlayers.Count > 0)
            return false;

        // Создаем нового игрока
        var player = new PlayerEntity { Username = username, Password = password };
        player.Save();
        return true;
    }

    public string Login(string username, string password)
    {
        // Ищем игрока с указанным логином и паролем
        var players = DB.TakeAll<PlayerEntity>()
            .Filter(p => p.Username == username && p.Password == password)
            .Get();

        if (players.Count == 0)
            return null;

        return players[0].EntityId; // Используем EntityId
    }
}