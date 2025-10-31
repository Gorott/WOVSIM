namespace WOVSIM.CodeRunner.Models;

public class CodeRunResult
{
    public bool IsSuccess { get; set; }
    public string Output { get; set; }
    public string Error { get; set; }
    public int ExitCode { get; set; }
}