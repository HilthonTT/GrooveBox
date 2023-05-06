namespace GrooveBoxDesktop.Helpers;
public class OidGenerator : IOidGenerator
{
    public async Task<string> GenerateOidAsync()
    {
        return await Task.Run(() =>
        {
            byte[] objectIdBytes = new byte[12];
            byte[] timestampBytes = BitConverter.GetBytes(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            byte[] machineIdBytes = BitConverter.GetBytes(Environment.MachineName.GetHashCode()).Take(3).ToArray();
            byte[] processIdBytes = BitConverter.GetBytes(Environment.ProcessId).Take(2).ToArray();
            byte[] incrementBytes = BitConverter.GetBytes(new Random().Next(0, 16777216)).Take(3).ToArray();

            Array.Copy(timestampBytes, 0, objectIdBytes, 0, 4);
            Array.Copy(machineIdBytes, 0, objectIdBytes, 4, 3);
            Array.Copy(processIdBytes, 0, objectIdBytes, 7, 2);
            Array.Copy(incrementBytes, 0, objectIdBytes, 9, 3);

            return Convert.ToBase64String(objectIdBytes);
        });
    }
}
