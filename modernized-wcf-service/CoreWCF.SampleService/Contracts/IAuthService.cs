// Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: MIT-0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CoreWCF;

namespace WCF.SampleService.Contracts
{
    [ServiceContract]
    public interface IAuthService
    {
        [OperationContract]
        bool Authenticate(LoginInfo loginInfo);
    }

    [DataContract]
    public class LoginInfo
    {
        [DataMember]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [DataMember]
        [Required]
        public string Password { get; set; } = "";
    }
}