using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Authentication
{
    public class SignInFormMap : ModelMap<SignInForm, AccountUser>
    {
        public SignInFormMap()
        {
            ForProperty(model => model.Login)
                .Required()
                .DisplayName("User Name");

            ForProperty(model => model.Password)
                .Required()
                .Password();
        }
    }
}

