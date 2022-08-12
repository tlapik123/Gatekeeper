using System.Security.Cryptography;

namespace gatekeeper.GkDataStructures;

/// <summary>
/// Parsed configuration
/// </summary>
/// <param name="Token">Bot token</param>
/// <param name="EmailServer">Email server to use</param>
/// <param name="EmailUsername">Username of the email</param>
/// <param name="EmailPassword">Password of the email</param>
/// <param name="HashAlg">Hash algorithm to use in hashing</param>
/// <param name="Salt">Salt to used in hashing</param>
/// <param name="SaveFile">File to saved hashed values</param>
public record GkConfig(string Token, string EmailServer, string EmailUsername, string EmailPassword,
    HashAlgorithm HashAlg, string Salt, string SaveFile);