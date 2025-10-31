using WOVSIM.CodeRunner.Models;

namespace WOVSIM.CodeRunner.Interfaces;

public interface ICodeRunnerService
{
    Task<CodeRunResult> RunCodeAsync(
        ProgrammingLanguage language,
        string? code,
        string? input = null);
}