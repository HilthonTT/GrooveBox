namespace GrooveBoxDesktop.Helpers;
public class OidGenerator : IOidGenerator
{
    public async Task<string> GenerateOidAsync()
    {
        return await Task.Run(() =>
        {
            byte[] objectIdBytes = new byte[16]; 
            byte[] timestampBytes = BitConverter.GetBytes(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            byte[] machineIdBytes = BitConverter.GetBytes(Environment.MachineName.GetHashCode()).Take(4).ToArray(); 
            byte[] processIdBytes = BitConverter.GetBytes(Environment.ProcessId).Take(2).ToArray();
            byte[] incrementBytes = BitConverter.GetBytes(new Random().Next(0, 16777216)).Take(5).ToArray(); 

            Array.Copy(timestampBytes, 0, objectIdBytes, 0, 4);
            Array.Copy(machineIdBytes, 0, objectIdBytes, 4, 4); 
            Array.Copy(processIdBytes, 0, objectIdBytes, 8, 2);
            Array.Copy(incrementBytes, 0, objectIdBytes, 10, 5); 

            return Convert.ToBase64String(objectIdBytes);
        });
    }
}
