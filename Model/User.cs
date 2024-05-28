using Backend.Model;
using Backend.Utils;
using System.Data.Common;

namespace FrontEnd.Model
{
    [Table(nameof(User))]
    public class User : AbstractModel, IUser
    {
        private long _userid;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private bool _rememberme = false;

        [PK]
        public long UserID { get => _userid; set => UpdateProperty(ref value, ref _userid); }
        [Field]
        public string UserName { get => _username; set => UpdateProperty(ref value, ref _username); }
        [Field]
        public string Password { get => _password; set => UpdateProperty(ref value, ref _password); }
        public bool RememberMe { get => _rememberme; set => UpdateProperty(ref value, ref _rememberme); }
        public int Attempts { get; protected set; } = 3;
        public string Target { get; } = SysCredentailTargets.UserLogin;

        public User(DbDataReader reader) : this()
        {
            _userid = reader.GetInt64(0);
            _username = reader.GetString(1);
            _password = reader.GetString(2);
        }

        public User() => SelectQry = $"SELECT * FROM {nameof(User)} WHERE {nameof(UserName)} = @{nameof(UserName)};";

        public virtual bool Login(string? pwd)
        {
            if (Attempts == 0) throw new Exception("Ran out of login attempts");
            if (string.IsNullOrEmpty(Password)) throw new Exception("Password is empty");
            if (Password.Equals(pwd))
            {
                if (RememberMe)
                    SaveCredentials();
                return true;
            }
            Attempts--;
            return false;
        }
        
        public virtual void Logout() => CredentialManager.Delete(Target);

        public virtual void SaveCredentials() => CredentialManager.Store(new(Target, UserName, Password));

        public override ISQLModel Read(DbDataReader reader) => new User(reader);

        public void ResetAttempts() => Attempts = 3;

        public override string? ToString() => UserName;

    }
}
