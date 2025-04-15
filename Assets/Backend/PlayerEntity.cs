using Unisave;
using Unisave.Entities;
using Unisave.Facades;
using Unisave.Facets;

namespace Unisave.Backend
{
    [EntityCollectionName("players")]
    public class PlayerEntity : Entity
    {
        public string Username;
        public string Password;
    }

    public enum RegisterStatus
    {
        Success,
        InvalidUsername,
        WeakPassword,
        UsernameTaken
    }

    public class AuthFacet : Facet
    {
        public RegisterStatus Register(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Contains(" "))
                return RegisterStatus.InvalidUsername;

            if (password.Length < 8)
                return RegisterStatus.WeakPassword;

            var existingPlayers = DB.TakeAll<PlayerEntity>()
                .Filter(p => p.Username == username)
                .Get();

            if (existingPlayers.Count > 0)
                return RegisterStatus.UsernameTaken;

            var player = new PlayerEntity { Username = username, Password = password };
            player.Save();
            return RegisterStatus.Success;
        }

        public string Login(string username, string password)
        {
            var players = DB.TakeAll<PlayerEntity>()
                .Filter(p => p.Username == username && p.Password == password)
                .Get();

            if (players.Count == 0)
                return null;

            return players[0].EntityId;
        }
    }
}
