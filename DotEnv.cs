public static class DotEnv
{
    public static void Load(string rootPath)
    {
        string[] filePaths = { ".env.local", ".env" };
				string? filePath = null;
        foreach (string path in filePaths)
        {
            if (!File.Exists((path)))
                continue;

						filePath = path;
						break;
        }

				if (filePath is null) return;
        foreach (var line in File.ReadAllLines((filePath)))
        {
            var parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                continue;

            Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }
    }
}
