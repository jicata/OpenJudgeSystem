﻿namespace OJS.Workers.ExecutionStrategies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    public class JsonExecutionResult
    {
        private const string InvalidJsonReplace = "]}[},!^@,Invalid,!^@,{]{[";

        public IList<string> TestsErrors { get; set; }

        public string Error { get; set; }

        public int TotalPasses { get; set; }

        public List<int> PassingIndexes { get; set; }

        public int TotalTests { get; set; }

        public static JsonExecutionResult Parse(string result, bool forceErrorExtracting = false, bool getTestIndexes = false)
        {
            JObject jsonTestResult = null;
            var totalPasses = 0;
            var totalTests = 0;
            var errors = new List<string>();
            var error = string.Empty;
            try
            {
                jsonTestResult = JObject.Parse(result.Trim().Replace("/*", InvalidJsonReplace).Replace("*/", InvalidJsonReplace));
                totalPasses = (int)jsonTestResult["stats"]["passes"];
                totalTests = (int)jsonTestResult["stats"]["tests"];
                var testsJs = (JArray)jsonTestResult["tests"];
                for (int i = 0; i < testsJs.Count; i++)
                {
                    JToken token = jsonTestResult["tests"][i]["err"]["message"];
                    if (token != null)
                    {
                        errors.Add((string)jsonTestResult["tests"][i]["err"]["message"]);
                    }
                    else
                    {
                        errors.Add(string.Empty);
                    }
                }
            }
            catch
            {
                error = "Invalid console output!";
            }

            var testsIndexes = new List<int>();
            if (getTestIndexes)
            {
                try
                {
                    testsIndexes = jsonTestResult["passing"].Values<int>().ToList();
                }
                catch
                {
                    error = "Invalid console output!";
                }
            }

            if (forceErrorExtracting)
            {
                try
                {
                    var failures = (JArray)jsonTestResult["failures"];
                    for (int i = 0; i < failures.Count; i++)
                    {
                        error += (string)jsonTestResult["failures"][i]["err"]["message"];
                        error += Environment.NewLine;
                    }
                }
                catch
                {
                    error = "Invalid console output!";
                }
            }

            return new JsonExecutionResult
            {
                TestsErrors = errors,
                Error = error,
                PassingIndexes = testsIndexes,
                TotalPasses = totalPasses,
                TotalTests = totalTests
            };
        }
    }
}
