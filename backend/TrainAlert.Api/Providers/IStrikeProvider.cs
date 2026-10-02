public interface IStrikeProvider
{
  Task<Strike?> GetCurrentStrikeAsync();
}