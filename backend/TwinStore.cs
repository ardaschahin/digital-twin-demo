// TwinStore.cs
using System.Text.Json;

public static class TwinStore
{
    private static TwinModel _state = new TwinModel();

    public static void Update(TwinModel newState)
    {
        _state = newState;
    }

    public static TwinModel Get()
    {
        return _state;
    }

    public static string GetJson()
    {
        return JsonSerializer.Serialize(_state, new JsonSerializerOptions
        {
            WriteIndented = false
        });
    }
}
