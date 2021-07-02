using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IQI.Intuition.Web.Models.Authentication
{
    public class SignInForm
    {
        public virtual string Login { get; set; }

        public virtual string Password { get; set; }

        public virtual string ReturnUrl { get; set; }
    }
}