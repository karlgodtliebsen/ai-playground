﻿using Microsoft.SemanticKernel.Planning;

namespace SemanticKernel.Domain.Utils;


// .NET 8 and the System.Text.Json v8.0.0 nuget package include built-in support for ReadOnlyMemory.
// This is a temporary workaround for .NET 6 and the System.Text.Json v6.0.0 nuget package.
// It should be removed once SK projects upgrade to System.Text.Json v8.0.0.

public static class PlanExtensions
{
    public static string ToPlanString(this Plan originalPlan, string indent = " ")
    {
        var goalHeader = $"{indent}Goal: {originalPlan.Description}\n\n{indent}Steps:\n";

        var stepItems = string.Join("\n", originalPlan.Steps.Select(step =>
        {
            if (step.Steps.Count == 0)
            {
                var skillName = step.SkillName;
                var stepName = step.Name;

                var parameters = string.Join(" ", step.Parameters.Select(param => $"{param.Key}='{param.Value}'"));
                if (!string.IsNullOrEmpty(parameters))
                {
                    parameters = $" {parameters}";
                }

                var outputs = step.Outputs.FirstOrDefault();
                if (!string.IsNullOrEmpty(outputs))
                {
                    outputs = $" => {outputs}";
                }

                return $"{indent}{indent}- {string.Join(".", skillName, stepName)}{parameters}{outputs}";
            }

            return step.ToPlanString(indent + indent);
        }));

        return goalHeader + stepItems;
    }
}
