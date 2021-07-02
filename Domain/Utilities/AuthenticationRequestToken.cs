using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SnyderIS.sCore.Encryption;

namespace IQI.Intuition.Domain.Utilities
{
    public class AuthenticationRequestToken
    {

        public const string AUTHENTICATION_TOKEN_REQUEST_VAR = "authtoken";

        public string Login { get; private set; }
        public string PasswordHash { get; private set; }

        public AuthenticationRequestToken(Models.AccountUser user)
        {
            this.Login = user.Login;
            this.PasswordHash = user.PasswordHash;
        }

        public AuthenticationRequestToken(string data)
        {
            string decrypted = new DESHelper(Constants.SYSTEM_KEY_DES).DecryptString(data);
            string[] segments = decrypted.Split(":".ToCharArray());
            foreach (string chunk in segments)
            {
                string key = chunk.Split("|".ToCharArray())[0];
                string value = chunk.Split("|".ToCharArray())[1];

                if (key == "Login")
                {
                    this.Login = value;
                }
                else if (key == "Password")
                {
                    this.PasswordHash = value;
                }
            }
        }

        public string ToToken()
        {
            var builder = new StringBuilder();

            builder.Append("Login");
            builder.Append("|");
            builder.Append(this.Login);
            builder.Append(":");
            builder.Append("Password");
            builder.Append("|");
            builder.Append(this.PasswordHash);

            string encrypted = new DESHelper(Constants.SYSTEM_KEY_DES).EncryptString(builder.ToString());
            return UrlEncodeHelper.UrlEncode(encrypted);
        }

    }


}
