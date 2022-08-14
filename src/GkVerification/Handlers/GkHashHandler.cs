using System.Security.Cryptography;
using System.Text;

namespace gatekeeper.GkVerification.Handlers; 

public class GkHashHandler : IGkHashHandler {
    private readonly HashAlgorithm _alg;
    private readonly string _salt;

    public GkHashHandler(HashAlgorithm alg, string salt) => (_alg, _salt) = (alg, salt);
    
    public string ComputeHashFromUkco(string ukco) {
        var bytes = _alg.ComputeHash(Encoding.UTF8.GetBytes(ukco + _salt));
        return Convert.ToHexString(bytes).ToLower();

    }
}