using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.ModelBinding;

namespace Advisr.Web.Models
{
    public class RegisterGeneralModel
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ContactPhone { get; set; }

        public string UserName { get; set; }

        public string Provider { get; set; }

        public string ExternalAccessToken { get; set; }

        public virtual string Password { get; set; }

    }


    public class RegisterUserResult
    {
        public RegisterUserResult()
        {
            this.ModelState = new ModelStateDictionary();
            this.HttpStatusCode = HttpStatusCode.OK;
        }

        public string UserId { get; set; }

        public ModelStateDictionary ModelState { get; private set; }
        
        public string ErrorMessage { get; private set; }

        public HttpStatusCode HttpStatusCode { get; private set; }

        public int ServerErrorCode { get; private set; }
        
        public bool HasError
        {
            get { return this.HttpStatusCode != HttpStatusCode.OK || this.ModelState.IsValid == false; }
        }
        
        public void AddError(HttpStatusCode code, int serverErrorCode, string error)
        {
            this.HttpStatusCode = code;
            this.ErrorMessage = error;
            this.ServerErrorCode = serverErrorCode;
        }
    }
}


