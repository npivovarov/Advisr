using Advisr.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Advisr.Web.Controllers
{
    public class BaseApiController : ApiController
    {
        protected IHttpActionResult JsonError(HttpStatusCode httpCode, int serverErrorCode, string message, object details = null)
        {
            if (details != null && details is ModelStateDictionary)
            {
                List<object> detailsList = new List<object>();
                foreach (var item in ModelState)
                {
                    if (item.Key != "model")
                    {
                        var fieldW = FirstCharacterToLower(item.Key.Replace("model.", ""));

                        if (fieldW.Contains("["))
                        {
                            Regex regexCollectionWithSubValue = new Regex(@"\b(?<field>\w+)\[(?<index>[0-9]+)\].(?<subField>\w+)");
                            Regex regexCollection = new Regex(@"\b(?<field>\w+)\[(?<index>[0-9]+)\]");

                            GroupCollection groups = regexCollectionWithSubValue.Match(fieldW).Groups;
                            if (groups.Count == 0)
                            {
                                groups = regexCollection.Match(fieldW).Groups;
                            }

                            string field = FirstCharacterToLower(groups["field"].Value);
                            int index = int.Parse(groups["index"].Value);
                            string subField = groups.Count > 3 ? FirstCharacterToLower(groups["subField"].Value) : string.Empty;

                            var modelError = new
                            {
                                field = field,
                                index = index,
                                subField = subField,
                                errors = item.Value.Errors.Select(a =>
                                {
                                    if (a.Exception != null)
                                    {
                                        return a.Exception.Message;
                                    }
                                    else
                                    {
                                        return a.ErrorMessage;
                                    };
                                }).ToList()
                            };
                            detailsList.Add(modelError);
                        }
                        else
                        {
                            var modelError = new
                            {
                                field = fieldW,
                                value = item.Value.Value,
                                errors = item.Value.Errors.Select(a =>
                                {
                                    if (a.Exception != null)
                                    {
                                        return a.Exception.Message;
                                    }
                                    else
                                    {
                                        return a.ErrorMessage;
                                    };
                                }).ToList()
                            };

                            if (item.Value.Errors.Any())
                            {
                                detailsList.Add(modelError);
                            }
                        }
                    }
                }

                details = detailsList;
            }

            Error error = new Error();
            error.HttpStatusCode = httpCode;
            error.ServerErrorCode = serverErrorCode;
            error.Message = message;
            error.Details = details;

            return this.Content(httpCode, error);
        }

        public static string FirstCharacterToLower(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return str;
            }

            //var fields = str.Split('.');

            //for (int i = 0; i < fields.Length; i++)
            //{
            //    var field = fields[i];
            //    fields[i] = Char.ToLowerInvariant(field[0]) + field.Substring(1);
            //}

            //return string.Join(".", fields);
            
            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
    }
}