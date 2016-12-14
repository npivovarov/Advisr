using Advisr.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Advisr.Web.Models
{

    public class UpdatePolicyModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int PolicyTypeId { get; set; }

        [Required]
        public int InsurerId { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        [Required]
        public DateTime? EndDate { get; set; }

        [Required]
        public string PolicyNumber { get; set; }

        [Required]
        public decimal? PolicyPremium { get; set; }

        [Required]
        public PolicyPaymentFrequency PolicyPaymentFrequency { get; set; }

        [Required]
        public decimal? PolicyPaymentAmount { get; set; }

        [Required]
        public decimal? PolicyExcess { get; set; }

        //------------------------------------
        //------------------------------------
        //Properties for Policy Types
        //------------------------------------
        //------------------------------------
        

        //public VehiclePolicyModel VehiclePolicyModel { get; set; }

        //public LifePolicyModel LifePolicyModel { get; set; }

        //public HomePolicyModel HomePolicyModel { get; set; }

        //------------------------------------
        //------------------------------------
        //------------------------------------
        //------------------------------------

        /// <summary>
        /// Additional Properties from PolicyTypeFields
        /// </summary>
        public List<PolicyPropertyModel> AdditionalProperties { get; set; }

        /// <summary>
        /// Properties for Policy Group
        /// </summary>
        [Required]
        public List<PolicyPropertyModel> PolicyProperties { get; set; }

        /// <summary>
        /// Coverages for the policy
        /// </summary>
        public List<PolicyCoverageModel> Coverages { get; set; }

        [Required]
        public bool IsConfirmed { get; set; }
    }

    public class PolicyPropertyModel
    {
        [Required]
        public int PropertyId { get; set; }

        [Required]
        public dynamic Value { get; set; }

        public PolicyTypeFieldType FieldType { get; set; }
        
        public string GetValueAsString(PolicyTypeFieldType type)
        {
            string value = null;

            if (type == PolicyTypeFieldType.List)
            {
                value = string.Format("{0}", this.Value.value);
            }
            else if (type == PolicyTypeFieldType.Bool)
            {
                value = "No";

                try
                {
                    value = this.Value.id == true ? "Yes" : "No";
                }
                catch (Exception)
                {
                }
            }
            else
            {
                value = string.Format("{0}", this.Value);
            }
            
            return value;
        }
    }


    public class PolicyCoverageModel
    {
        [Required]
        public int CoverageId { get; set; }

        //[Required]
        public bool? IsActive { get; set; }

        //[Required]
        public decimal? Excess { get; set; }

        //[Required]
        public string Limit { get; set; }
    }

    
    //public class VehiclePolicyModel
    //{
    //    [Required]
    //    public int? Year { get; set; }

    //    [Required]
    //    public string Make { get; set; }

    //    [Required]
    //    public string Model { get; set; }
        
    //    [Required]
    //    public string RegistredDriverName { get; set; }

    //    [Required]
    //    public string RegistrationNumber { get; set; }
        
    //    [Required]
    //    public bool? IsCommercial { get; set; }
    //}

    //public class LifePolicyModel
    //{
    //    [Required]
    //    public string Medication { get; set; }

    //    [Required]
    //    public string MedicationCondition { get; set; }
    //}

    //public class HomePolicyModel
    //{
    //    [Required]
    //    public string Address { get; set; }

    //    [Required]
    //    public DateTime? BuildDate { get; set; }
    //}
}