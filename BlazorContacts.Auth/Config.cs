// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace BlazorContacts.Auth
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            { 
                new IdentityResources.OpenId()
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[] 
            {
                new ApiResource("blazorcontacts-api")
             };
        
        public static IEnumerable<Client> Clients =>
            new Client[] 
            { 
                new Client 
                {
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientId = "blazorcontacts-web",
                    ClientSecrets = {
                        new Secret("thisismysecrets".Sha256())
                    },
                    AllowedScopes = { "blazorcontacts-api" }
                }
            };
        
    }
}