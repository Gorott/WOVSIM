using Discord;
using WOVSIM.Bot.Extensions;
using WOVSIM.Bot.Models;
using WOVSIM.CodeRunner;
using WOVSIM.CodeRunner.Interfaces;
using WOVSIM.CodeRunner.Models;
using WOVSIM.Domain.Enums;

namespace WOVSIM.Bot.Builders;

public class ChallengeBuilder
{
    private readonly ICodeRunnerService _codeRunnerService;

    public ChallengeBuilder(ICodeRunnerService codeRunnerService)
    {
        _codeRunnerService = codeRunnerService;
    }

    public async Task<ResponseModel> SubmitCodeAsync(ProgrammingLanguage language, string code)
    {
        var response = new ResponseModel { ResponseType = ResponseType.ComponentsV2 };
        var result = await _codeRunnerService.RunCodeAsync(language, code);

        var content = result.IsSuccess ? result.Output : result.Error;
        var (message, fileContent) = content.HandleLongMessage();

        if (fileContent != null)
        {
            response.FileName = "output.txt";
        }

        var container = new ContainerBuilder()
            .WithTextDisplay($"## **Submission Result**\n```\n{message}\n```")
            .WithAccentColor(result.IsSuccess ? Color.Green : Color.Red);


        if (response.FileName != null)
        {
            container.WithFile(new FileComponentBuilder()
                .WithFile(new UnfurledMediaItemProperties
                {
                    Url = $"attachment://{response.FileName}"
                }));
        }
        
        var builder = new ComponentBuilderV2()
            .WithContainer(container);

        response.ComponentsV2 = builder;
        return response;
    }

}
