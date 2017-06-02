﻿using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AwsParameterStore
{
    class Program
    {
        static void Main(string[] args)
        {
            Test().Wait();
        }

        static async Task Test()
        {
            // list the parameters to retrieve
            List<String> parameterNames = new List<string>(new string[] { "hello" });

            // retrieve the parameter names and values in a dictionary
            Dictionary<string, string> parameters = await GetParameters(parameterNames, "soconnor", RegionEndpoint.USEast1);
            foreach (var key in parameters.Keys)
            {
                Console.WriteLine($"{key} {parameters[key]}");
            }
        }

        static async Task<Dictionary<string, string>> GetParameters(List<string> parameterNames,  string profile, RegionEndpoint endpoint)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            var chain = new CredentialProfileStoreChain();
            AWSCredentials awsCredentials;
            if (chain.TryGetAWSCredentials(profile, out awsCredentials))
            {
                AmazonSimpleSystemsManagementClient client = new AmazonSimpleSystemsManagementClient(awsCredentials, endpoint);

                GetParametersRequest req = new GetParametersRequest();
                req.Names = parameterNames;
                req.WithDecryption = true;

                GetParametersResponse resp = await client.GetParametersAsync(req);
                foreach (var parameter in resp.Parameters)
                {
                    parameters.Add(parameter.Name, parameter.Value);
                }
            }

            return parameters;
        }
    }
}
