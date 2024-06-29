using Backend.Model;
using Backend.Utils;
using System.Data.Common;

namespace FrontEnd.Model
{
    /// <summary>
    /// Represents a user entity extending <see cref="AbstractModel{T}"/> and implementing <see cref="IUser"/>.
    /// </summary>
    [Table(nameof(User))]
    public class User : AbstractModel<User>, IUser
    {
        #region backing fields
        private long _userid;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private bool _rememberme = false;
        #endregion

        #region Properties
        [PK]
        public long UserID { get => _userid; set => UpdateProperty(ref value, ref _userid); }
        [Field]
        public string UserName { get => _username; set => UpdateProperty(ref value, ref _username); }
        [Field]
        public string Password { get => _password; set => UpdateProperty(ref value, ref _password); }

        /// <summary>
        /// Gets or sets whether the user wants their access to be remembered.
        /// </summary>
        public bool RememberMe { get => _rememberme; set => UpdateProperty(ref value, ref _rememberme); }

        /// <summary>
        /// Gets or sets the number of login attempts allowed.
        /// </summary>
        public int Attempts { get; protected set; } = 3;

        /// <summary>
        /// Gets the target credential identifier for user login.
        /// </summary>
        public string Target { get; } = SysCredentailTargets.UserLogin;
        #endregion

        /// <summary>
        /// Constructs a new instance of <see cref="User"/>.
        /// Initializes the SQL select query for user retrieval.
        /// </summary>
        public User() => SelectQry = $"SELECT * FROM {nameof(User)} WHERE {nameof(UserName)} = @{nameof(UserName)};";

        /// <summary>
        /// Constructs a <see cref="User"/> instance from a database reader.
        /// </summary>
        /// <param name="reader">The database reader containing user data.</param>
        public User(DbDataReader reader) : this()
        {
            _userid = reader.GetInt64(0);
            _username = reader.GetString(1);
            _password = reader.GetString(2);
        }

        /// <summary>
        /// Attempts to log in the user with the provided password.
        /// </summary>
        /// <param name="pwd">The password to validate.</param>
        /// <returns>True if login is successful; otherwise, false.</returns>
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

        /// <summary>
        /// Logs out the user by deleting stored credentials.
        /// </summary>        
        public virtual void Logout() => CredentialManager.Delete(Target);

        /// <summary>
        /// Saves user credentials securely.
        /// </summary>
        public virtual void SaveCredentials() => CredentialManager.Store(new(Target, UserName, Password));

        /// <summary>
        /// Resets login attempts to the default value.
        /// </summary>
        public void ResetAttempts() => Attempts = 3;

        /// <summary>
        /// Returns the username as the string representation of the user object.
        /// </summary>
        /// <returns>The username of the user.</returns>
        public override string? ToString() => UserName;

    }
}