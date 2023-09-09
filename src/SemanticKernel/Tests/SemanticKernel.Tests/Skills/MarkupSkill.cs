using System.ComponentModel;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

using SemanticKernel.Tests.Utils;

namespace SemanticKernel.Tests.Skills;

//public class MarkupSkill
//{
//    private readonly ILogger logger;

//    public MarkupSkill(ILogger logger)
//    {
//        this.logger = logger;
//    }

//    [SKFunction, Description("Run Markup")]
//    public async Task<string> RunMarkupAsync(string docString, SKContext context)
//    {
//        var plan = docString.FromMarkup("Run a piece of xml markup", context, logger);

//        logger.Information("Markup plan:");
//        logger.Information(plan.ToPlanString());
//        logger.Information("");

//        var result = await plan.InvokeAsync();
//        return result.Result;
//    }
//}



// Example Skill that can process XML Markup created by ContextQuery
public class MarkupSkill
{
    private readonly ILogger logger;

    public MarkupSkill(ILogger logger)
    {
        this.logger = logger;
    }
    [SKFunction, Description("Run Markup")]
    public async Task<string> RunMarkupAsync(string docString, SKContext context)
    {
        var plan = docString.FromMarkup("Run a piece of xml markup", context, logger);
        logger.Information("Markup plan:");
        logger.Information(plan.ToPlanString());
        logger.Information("");

        //Console.WriteLine("Markup plan:");
        //Console.WriteLine(plan.ToPlanWithGoalString());
        //Console.WriteLine();

        var result = await plan.InvokeAsync();
        return result.Result;
    }
}
