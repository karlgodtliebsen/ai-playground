using System.Xml;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Planning;

namespace SemanticKernel.Tests.Utils;

public static class XmlMarkupPlanParser
{
    private static readonly Dictionary<string, KeyValuePair<string, string>> s_skillMapping = new() {
        { "lookup", new KeyValuePair<string, string>("bing", "SearchAsync") },
    };

    public static Plan FromMarkup(this string markup, string goal, SKContext context, ILogger logger)
    {
        logger.Information("Markup:");
        logger.Information(markup);
        logger.Information("");

        var doc = new XmlMarkup(markup);
        var nodes = doc.SelectElements();
        return nodes.Count == 0 ? new Plan(goal) : NodeListToPlan(nodes, context, goal);
    }

    private static Plan NodeListToPlan(XmlNodeList nodes, SKContext context, string description)
    {
        Plan plan = new(description);
        for (var i = 0; i < nodes.Count; ++i)
        {
            var node = nodes[i];
            var functionName = node!.LocalName;
            var skillName = string.Empty;

            if (s_skillMapping.TryGetValue(node!.LocalName, out var value))
            {
                functionName = value.Value;
                skillName = value.Key;
            }

            var hasChildElements = node.HasChildElements();

            if (hasChildElements)
            {
                plan.AddSteps(NodeListToPlan(node.ChildNodes, context, functionName));
            }
            else
            {
                if (string.IsNullOrEmpty(skillName)
                        ? !context.Skills!.TryGetFunction(functionName, out var _)
                        : !context.Skills!.TryGetFunction(skillName, functionName, out var _))
                {
                    var planStep = new Plan(node.InnerText);
                    planStep.Parameters.Update(node.InnerText);
                    planStep.Outputs.Add($"markup.{functionName}.result");
                    plan.Outputs.Add($"markup.{functionName}.result");
                    plan.AddSteps(planStep);
                }
                else
                {
                    var command = string.IsNullOrEmpty(skillName)
                        ? context.Skills.GetFunction(functionName)
                        : context.Skills.GetFunction(skillName, functionName);
                    var planStep = new Plan(command);
                    planStep.Parameters.Update(node.InnerText);
                    planStep.Outputs.Add($"markup.{functionName}.result");
                    plan.Outputs.Add($"markup.{functionName}.result");
                    plan.AddSteps(planStep);
                }
            }
        }

        return plan;
    }
}
